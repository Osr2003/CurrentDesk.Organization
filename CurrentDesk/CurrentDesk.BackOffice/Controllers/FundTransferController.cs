﻿#region Header Information
/*****************************************************************************
 * File Name     : FundTransferController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th Feb 2013
 * Modified Date : 11th Feb 2013
 * Description   : This file handles all operations related to Transfer page
 * **************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Models.Transfers;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.BackOffice.Utilities;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using Microsoft.ApplicationServer.Caching;
using MT4ManLibraryNETv03;
using MT4Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This controller handles all operations related to Transfers page
    /// </summary>
    [AuthorizeTrader, NoCache]
    public class FundTransferController : Controller
    {
        private const decimal PointMultiplier = 0.00001M;
        private const decimal JpyPointMultiplier = 0.001M;
        
        #region Variables
        private BankAccountInformationBO bankBO = new BankAccountInformationBO();
        private Client_AccountBO clientAccBO = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private L_CountryBO countryBO = new L_CountryBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private TransactionBO transactionBO = new TransactionBO();
        private TransferLogBO transferLogBO = new TransferLogBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private TransferActivityBO transActivityBO = new TransferActivityBO();
        private ConversionActivityBO convActivityBO = new ConversionActivityBO();
        private FundingSourceBO fundSourceBO = new FundingSourceBO();
        private AdminTransactionBO adminTransactionBO = new AdminTransactionBO();
        private ClientBO clientBO = new ClientBO();
        private TransactionSettingBO transactionSettingBO = new TransactionSettingBO();
        private PriceBO priceBO = new PriceBO();
        private DepositOrWithdrawActivityBO depWithDrwActBO = new DepositOrWithdrawActivityBO();
        private FundingSourceAcceptedCurrencyBO fundSrcAccpCurrBO = new FundingSourceAcceptedCurrencyBO();
        #endregion

        /// <summary>
        /// This actions returns FundAccount view in Fund Transfers section
        /// </summary>
        /// <returns></returns>
        public ActionResult FundAccount()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");
                    
                    var model = new TransfersModel();
                    model.BankInformation = new List<BankInformation>();
                    model.LandingAccInformation = new List<LandingAccInformation>();

                    //Get all bank accounts
                    var userBankInfos = bankBO.GetAllBankInfosForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var bank in userBankInfos)
                    {
                        var bankInfo = new BankInformation();
                        bankInfo.BankName = bank.BankName;
                        bankInfo.BankAccNumber = bank.AccountNumber;
                        model.BankInformation.Add(bankInfo);
                    }

                    //Get all landing accounts
                    var landingAccs = clientAccBO.GetAllLandingAccountForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;
                        lAccInfo.LAccCustomName = lAcc.AccountName;

                        lAccInfo.LAccBalance = Utility.FormatCurrencyValue((decimal)lAcc.CurrentBalance, "");

                        model.LandingAccInformation.Add(lAccInfo);
                    }

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action method adds new fund account request in database
        /// </summary>
        /// <param name="fundAccData">fundAccData</param>
        /// <returns></returns>
        public ActionResult AddFundAccountRequest(FundAccountData fundAccData)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int)SessionManagement.OrganizationID;

                    //Get incoming fund admin setting
                    var adminTransactionSetting = transactionSettingBO.GetTransactionSetting((int)AdminTransactionType.IncomingFunds, organizationID);
                    var currId = lCurrValueBO.GetCurrencyIDFromSymbol(fundAccData.Currency);
                    decimal minDepositAmount = 0;

                    if (adminTransactionSetting != null)
                    {
                        minDepositAmount = (decimal)adminTransactionSetting.MinimumDepositAmount;
                    }

                    if (fundAccData.Amount >= minDepositAmount)
                    {
                        LoginInformation loginInfo = SessionManagement.UserInfo;

                        //Assigning property values
                        var newFundRequest = new AdminTransaction();
                        newFundRequest.TransactionDate = DateTime.UtcNow;
                        newFundRequest.FK_UserID = loginInfo.UserID;
                        newFundRequest.AccountNumber = fundAccData.AccountNumber;
                        newFundRequest.FK_FundingSourceID = fundAccData.FundSourceID;
                        newFundRequest.FK_CurrencyID = currId;
                        newFundRequest.TransactionAmount = fundAccData.Amount;
                        newFundRequest.FK_AdminTransactionTypeID = (int)AdminTransactionType.IncomingFunds;
                        newFundRequest.Notes = fundAccData.Notes;
                        newFundRequest.ClientName = clientBO.GetClientName(loginInfo.UserID);
                        newFundRequest.IsApproved = false;
                        newFundRequest.IsDeleted = false;
                        newFundRequest.FK_OrganizationID = organizationID;

                        //Call BO method to add
                        if (adminTransactionBO.AddNewAdminTransactionRequest(newFundRequest))
                        {
                            InsertDepositOrWithdrawActivityDetails(Constants.K_DEPOSIT, currId, fundAccData.Amount, fundAccData.AccountNumber, null, Constants.K_STATUS_PENDING);

                            return Json(new { status = true});
                        }
                        else
                        {
                            return Json(new { status = false, message = "Some error occurred!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Minimum amount to be funded is " + Utility.FormatCurrencyValue(minDepositAmount, "") + "!" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!"});
            }
        }

        /// <summary>
        /// This actions returns WithdrawFund view in Fund Transfers section
        /// </summary>
        /// <returns></returns>
        public ActionResult WithdrawFund()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");
                    
                    var model = new TransfersModel();
                    model.BankInformation = new List<BankInformation>();
                    model.LandingAccInformation = new List<LandingAccInformation>();

                    //Get all bank accounts
                    var userBankInfos = bankBO.GetAllBankInfosForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var bank in userBankInfos)
                    {
                        var bankInfo = new BankInformation();
                        bankInfo.BankID = bank.PK_BankAccountInformationID;
                        bankInfo.BankName = bank.BankName;
                        bankInfo.BankAccNumber = bank.AccountNumber;
                        model.BankInformation.Add(bankInfo);
                    }

                    //Get all landing accounts
                    var landingAccs = clientAccBO.GetAllLandingAccountForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;
                        lAccInfo.LAccCustomName = lAcc.AccountName;

                        lAccInfo.LAccBalance = Utility.FormatCurrencyValue((decimal)lAcc.CurrentBalance, "");

                        model.LandingAccInformation.Add(lAccInfo);
                    }

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action inserts new fund withdraw request in database
        /// </summary>
        /// <param name="withdrawData">withdrawData</param>
        /// <returns></returns>
        public ActionResult AddWithdrawRequest(FundWithdrawData withdrawData)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var currencyID = lCurrValueBO.GetCurrencyIDFromSymbol(withdrawData.Currency);
                    var accountDetails = clientAccBO.GetLandingAccountForCurrencyOfUser(LoginAccountType.LiveAccount, loginInfo.UserID, currencyID);
                    var availableBalance = accountDetails != null ? (decimal)accountDetails.CurrentBalance : 0;

                    //Check if balance is  greater or equal to withdraw request
                    if (availableBalance >= withdrawData.Amount)
                    {
                        //Check if balance greater than pending withdrawal requests
                        if (availableBalance >= (adminTransactionBO.GetPendingWithdrawalAmount(withdrawData.AccountNumber, organizationID) + withdrawData.Amount))
                        {
                            //Assigning property values
                            var newWithdrawRequest = new AdminTransaction();
                            newWithdrawRequest.TransactionDate = DateTime.UtcNow;
                            newWithdrawRequest.FK_UserID = loginInfo.UserID;
                            newWithdrawRequest.AccountNumber = withdrawData.AccountNumber;
                            newWithdrawRequest.FK_BankInfoID = withdrawData.BankInfoID;
                            newWithdrawRequest.FK_CurrencyID = currencyID;
                            newWithdrawRequest.TransactionAmount = withdrawData.Amount;
                            newWithdrawRequest.FK_AdminTransactionTypeID = (int)AdminTransactionType.OutgoingFunds;
                            newWithdrawRequest.Notes = withdrawData.Notes;
                            newWithdrawRequest.ClientName = clientBO.GetClientName(loginInfo.UserID);
                            newWithdrawRequest.IsApproved = false;
                            newWithdrawRequest.IsDeleted = false;
                            newWithdrawRequest.FK_OrganizationID = organizationID;

                            //Call BO method to add
                            if (adminTransactionBO.AddNewAdminTransactionRequest(newWithdrawRequest))
                            {
                                //Log activity details
                                InsertDepositOrWithdrawActivityDetails(Constants.K_WITHDRAW, currencyID, withdrawData.Amount, withdrawData.AccountNumber, withdrawData.BankInfoID, Constants.K_STATUS_PENDING);

                                return Json(new { status = true});
                            }
                            else
                            {
                                return Json(new { status = false, message = "Some error occurred!" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance due to pending withdrawal requests!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Insufficient account balance!"}, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This actions returns InternalTransfer view in Fund Transfers section
        /// </summary>
        /// <returns></returns>
        public ActionResult InternalTransfer()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var model = new TransfersModel();
                    model.TradingAccInformation = new List<TradingAccountGrouped>();

                    //Get all trading accounts
                    var tradingAccs = clientAccBO.GetAllTradingAccountsOfUser(loginInfo.LogAccountType, loginInfo.UserID);
                    var pairedTradingAcct = tradingAccs.GroupBy(o => o.FK_CurrencyID);
                    var tradeList = new List<TradingAccountGrouped>();
                    foreach (var item in pairedTradingAcct)
                    {
                        var groupedTradingAccount = new TradingAccountGrouped();
                        groupedTradingAccount.TradingCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)item.Key);
                        var list = new List<Client_Account>();
                        foreach (var groupedItem in item)
                        {
                            list.Add(groupedItem);
                        }
                        groupedTradingAccount.TradingAccountList = list;
                        tradeList.Add(groupedTradingAccount);
                    }
                    model.TradingAccInformation = tradeList;

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action does internal transfer between accounts and logs in Transactions table
        /// </summary>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="amount">amount</param>
        /// <param name="exchangeRate">exchangeRate</param>
        /// <param name="notes">notes</param>
        /// <returns></returns>
        public ActionResult InternalFundTransfer(string fromAcc, string toAcc, double amount, double exchangeRate, string notes)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    var currID = lCurrValueBO.GetCurrencyIDFromAccountCode(fromAcc.Split('-')[ruleInfo.CurrencyPosition - 1]);
                    var clientName = clientBO.GetClientName(loginInfo.UserID);

                    //Get transaction settings from database
                    var transacSett =
                        transactionSettingBO.GetTransactionSetting((int) AdminTransactionType.InternalTransfers, organizationID);

                    var fromAccDetails = clientAccBO.GetAnyAccountDetails(fromAcc, organizationID);
                    var toAccDetails = clientAccBO.GetAnyAccountDetails(toAcc, organizationID);

                    //Get from acc balance
                    var balance = (decimal)fromAccDetails.CurrentBalance;
                    var pendingAmount = adminTransactionBO.GetPendingTransferAmount(fromAcc, organizationID);

                    var isToSucessful = true;
                    var isFromSucessful = true;

                    if (transacSett != null)
                    {
                        //Check balance
                        if (balance >= (decimal) amount)
                        {
                            //Check pending request balance
                            if (balance >= (pendingAmount + (decimal) amount))
                            {
                                //Check margin restriction
                                if (fromAccDetails.PlatformLogin != null)
                                {
                                    if (!IsMarginRestrictionSatisfied((int) fromAccDetails.PlatformLogin,
                                                                      (decimal) amount, (decimal) fromAccDetails.Equity,
                                                                      (double) transacSett.MarginRestriction))
                                    {
                                        return
                                            Json(
                                                new
                                                    {
                                                        status = false,
                                                        message =
                                                    "Due to open positions the maximum available for transfer is less than your request. If you wish to transfer additional funds please close open positions."
                                                    });
                                    }
                                }

                                //If approval settings is immediate or approval settings is limited and transfer amount less than limit, do immediate transfer
                                if (transacSett.InternalTransferApprovalOptions ==
                                    (int) TransferApprovalOptions.Immediate ||
                                    (transacSett.InternalTransferApprovalOptions ==
                                     (int) TransferApprovalOptions.Limited &&
                                     amount <= (double) transacSett.InternalTransferLimitedAmount))
                                {
                                    if (fromAccDetails.PlatformLogin != null)
                                    {
                                        isFromSucessful = DoPlatformTransaction((int) fromAccDetails.PlatformLogin,
                                                                                -amount,
                                                                                "Debit");
                                    }

                                    if (toAccDetails.PlatformLogin != null && isFromSucessful)
                                    {
                                        isToSucessful = DoPlatformTransaction((int) toAccDetails.PlatformLogin, amount,
                                                                              "Credit");
                                    }

                                    //If platform transactions are successful
                                    if (isToSucessful && isFromSucessful)
                                    {
                                        //If transaction is successful, then log in Transactions table
                                        if (clientAccBO.TransferFundInternal(fromAcc, toAcc, amount, exchangeRate, organizationID))
                                        {
                                            var pkTransactionID = transactionBO.InternalFundTransfer(fromAcc, toAcc,
                                                                                                     currID,
                                                                                                     currID, amount,
                                                                                                     exchangeRate, notes, organizationID);

                                            //Logs fund transfers details(Withdrawal/Deposit) in TransferLogs table
                                            transferLogBO.AddTransferLogForTransaction(pkTransactionID, fromAcc, toAcc,
                                                                                       currID, currID, amount,
                                                                                       exchangeRate, organizationID);

                                            //Log activity details
                                            InsertTransferActivityDetails(currID, amount, fromAcc, toAcc,
                                                                          Constants.K_STATUS_TRANSFERRED);

                                            //Insert into admin transaction table for records
                                            var transferTransac = new AdminTransaction();
                                            transferTransac.TransactionDate = DateTime.UtcNow;
                                            transferTransac.FK_UserID = loginInfo.UserID;
                                            transferTransac.AccountNumber = fromAcc;
                                            transferTransac.FK_CurrencyID = currID;
                                            transferTransac.TransactionAmount = (decimal) amount;
                                            transferTransac.FK_AdminTransactionTypeID =
                                                (int) AdminTransactionType.InternalTransfers;
                                            transferTransac.Notes = notes;
                                            transferTransac.ClientName = clientName;
                                            transferTransac.ApprovedDate = DateTime.UtcNow;
                                            transferTransac.ToAccountNumber = toAcc;
                                            transferTransac.ToClientName = clientName;
                                            transferTransac.IsApproved = true;
                                            transferTransac.IsDeleted = false;
                                            transferTransac.FK_OrganizationID = organizationID;

                                            //Add immediate transaction to Admin Transaction
                                            adminTransactionBO.AddNewAdminTransactionRequest(transferTransac);

                                            return
                                                Json(
                                                    new
                                                        {
                                                            status = true,
                                                            message =
                                                        "Internal transfer has been successfully completed."
                                                        });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new {status = false, message = "Some error occurred in platform!"});
                                    }
                                }


                                    //Make admin transfer request
                                else
                                {

                                    //Register transfer request
                                    var transferTransac = new AdminTransaction();
                                    transferTransac.TransactionDate = DateTime.UtcNow;
                                    transferTransac.FK_UserID = loginInfo.UserID;
                                    transferTransac.AccountNumber = fromAcc;
                                    transferTransac.FK_CurrencyID = currID;
                                    transferTransac.TransactionAmount = (decimal) amount;
                                    transferTransac.FK_AdminTransactionTypeID =
                                        (int) AdminTransactionType.InternalTransfers;
                                    transferTransac.Notes = notes;
                                    transferTransac.ClientName = clientName;
                                    transferTransac.FeeAmount = transacSett.TransferFee;
                                    transferTransac.ToAccountNumber = toAcc;
                                    transferTransac.ToClientName = clientName;
                                    transferTransac.IsApproved = false;
                                    transferTransac.IsDeleted = false;
                                    transferTransac.FK_OrganizationID = organizationID;

                                    //Add request to Admin Transaction
                                    adminTransactionBO.AddNewAdminTransactionRequest(transferTransac);

                                    //Log activity details for pending transaction
                                    InsertTransferActivityDetails(currID, amount, fromAcc, toAcc,
                                                                  Constants.K_STATUS_PENDING);

                                    return
                                        Json(
                                            new
                                                {
                                                    status = true,
                                                    message = "Internal transfer request has been submitted."
                                                });
                                }

                            }
                            else
                            {
                                return
                                    Json(
                                        new
                                            {
                                                status = false,
                                                message = "Insufficient balance due to pending transfer requests!"
                                            });
                            }
                        }
                        else
                        {
                            return Json(new {status = false, message = "Transfer failed due to insufficient balance"});
                        }
                    }
                    return Json(new {status = false, message = "No transaction settings found!"});
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new {status = false, message = "Some error occurred!"});
            }
        }

        /// <summary>
        /// This action does internal conversion transfer between accounts and logs in Transactions table
        /// </summary>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="amount">amount</param>
        /// <param name="exchangeRate">exchangeRate</param>
        /// <param name="notes">notes</param>
        /// <returns></returns>
        public ActionResult ConversionFundTransfer(string fromAcc, string toAcc, double amount, double exchangeRate, string notes)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;
                    var organizationID = (int)SessionManagement.OrganizationID;

                    var fromCurrID = lCurrValueBO.GetCurrencyIDFromAccountCode(fromAcc.Split('-')[ruleInfo.CurrencyPosition - 1]);
                    var toCurrID = lCurrValueBO.GetCurrencyIDFromAccountCode(toAcc.Split('-')[ruleInfo.CurrencyPosition - 1]);

                    var clientName = clientBO.GetClientName(loginInfo.UserID);

                    var fromAccDetails = clientAccBO.GetAnyAccountDetails(fromAcc, organizationID);
                    var toAccDetails = clientAccBO.GetAnyAccountDetails(toAcc, organizationID);

                    //Get from acc balance
                    var balance = (decimal)fromAccDetails.CurrentBalance;
                    var pendingAmount = adminTransactionBO.GetPendingTransferAmount(fromAcc, organizationID);

                    var isToSucessful = true;
                    var isFromSucessful = true;

                    //Get transaction settings from database
                    var transacSett =
                        transactionSettingBO.GetTransactionSetting((int) AdminTransactionType.ConversionsRequests, organizationID);

                    if (transacSett != null)
                    {
                        //Check balance
                        if (balance >= (decimal) amount)
                        {
                            //Check pending request balance
                            if (balance >= (pendingAmount + (decimal) amount))
                            {
                                //Check margin restriction
                                if (fromAccDetails.PlatformLogin != null)
                                {
                                    if (!IsMarginRestrictionSatisfied((int) fromAccDetails.PlatformLogin,
                                                                      (decimal) amount, (decimal) fromAccDetails.Equity,
                                                                      (double) transacSett.MarginRestriction))
                                    {
                                        return
                                            Json(
                                                new
                                                    {
                                                        status = false,
                                                        message =
                                                    "Due to open positions the maximum available for transfer is less than your request. If you wish to transfer additional funds please close open positions."
                                                    });
                                    }
                                }

                                //If approval settings is immediate or approval settings is limited 
                                //and transfer amount less than limit, do immediate transfer
                                if (transacSett.InternalTransferApprovalOptions ==
                                    (int) TransferApprovalOptions.Immediate ||
                                    (transacSett.InternalTransferApprovalOptions ==
                                     (int) TransferApprovalOptions.Limited &&
                                     amount <= (double) transacSett.InternalTransferLimitedAmount))
                                {

                                    if (fromAccDetails.PlatformLogin != null)
                                    {
                                        isFromSucessful = DoPlatformTransaction((int) fromAccDetails.PlatformLogin,
                                                                                -amount,
                                                                                "Debit");
                                    }

                                    if (toAccDetails.PlatformLogin != null && isFromSucessful)
                                    {
                                        isToSucessful = DoPlatformTransaction((int) toAccDetails.PlatformLogin,
                                                                              (amount*exchangeRate), "Credit");
                                    }

                                    //If platform transactions are successful
                                    if (isToSucessful && isFromSucessful)
                                    {
                                        //If transaction is successful, then log in Transactions table
                                        if (clientAccBO.TransferFundInternal(fromAcc, toAcc, amount, exchangeRate, organizationID))
                                        {
                                            var pkTransactionID = transactionBO.InternalFundTransfer(fromAcc, toAcc,
                                                                                                     fromCurrID,
                                                                                                     toCurrID,
                                                                                                     amount,
                                                                                                     exchangeRate,
                                                                                                     notes, organizationID);

                                            //Logs fund transfers details(Withdrawal/Deposit) in TransferLogs table
                                            transferLogBO.AddTransferLogForTransaction(pkTransactionID, fromAcc, toAcc,
                                                                                       fromCurrID, toCurrID, amount,
                                                                                       exchangeRate, organizationID);

                                            //Log activity details
                                            InsertConversionActivityDetails(fromCurrID, toCurrID, amount, exchangeRate,
                                                                            fromAcc, toAcc,
                                                                            Constants.K_STATUS_TRANSFERRED);

                                            //Insert into admin transaction table for records
                                            var convTransac = new AdminTransaction();
                                            convTransac.TransactionDate = DateTime.UtcNow;
                                            convTransac.FK_UserID = loginInfo.UserID;
                                            convTransac.AccountNumber = fromAcc;
                                            convTransac.FK_CurrencyID = fromCurrID;
                                            convTransac.TransactionAmount = (decimal) amount;
                                            convTransac.FK_AdminTransactionTypeID =
                                                (int) AdminTransactionType.ConversionsRequests;
                                            convTransac.IsApproved = true;
                                            convTransac.IsDeleted = false;
                                            convTransac.Notes = notes;
                                            convTransac.ClientName = clientName;
                                            convTransac.ApprovedDate = DateTime.UtcNow;
                                            convTransac.ToAccountNumber = toAcc;
                                            convTransac.ToClientName = clientName;
                                            convTransac.FK_ToUserID = loginInfo.UserID;
                                            convTransac.ExchangeRate = exchangeRate;
                                            convTransac.FK_ToCurrencyID = toCurrID;
                                            convTransac.FK_OrganizationID = organizationID;

                                            //Add conv transaction to Admin Transaction
                                            adminTransactionBO.AddNewAdminTransactionRequest(convTransac);

                                            return
                                                Json(
                                                    new
                                                        {
                                                            status = true,
                                                            message =
                                                        "Conversion transfer has been successfully completed."
                                                        });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new {status = false, message = "Some error occurred in platform!"});
                                    }
                                }
                                    //Make admin conversion request
                                else
                                {
                                    //Register transfer request
                                    var convTransac = new AdminTransaction();
                                    convTransac.TransactionDate = DateTime.UtcNow;
                                    convTransac.FK_UserID = loginInfo.UserID;
                                    convTransac.AccountNumber = fromAcc;
                                    convTransac.FK_CurrencyID = fromCurrID;
                                    convTransac.TransactionAmount = (decimal) amount;
                                    convTransac.FK_AdminTransactionTypeID =
                                        (int) AdminTransactionType.ConversionsRequests;
                                    convTransac.Notes = notes;
                                    convTransac.ClientName = clientName;
                                    convTransac.FeeAmount = transacSett.TransferFee;
                                    convTransac.ToAccountNumber = toAcc;
                                    convTransac.ToClientName = clientName;
                                    convTransac.FK_ToUserID = loginInfo.UserID;
                                    convTransac.ExchangeRate = exchangeRate;
                                    convTransac.FK_ToCurrencyID = toCurrID;
                                    convTransac.IsApproved = false;
                                    convTransac.IsDeleted = false;
                                    convTransac.FK_OrganizationID = organizationID;

                                    //Add request to Admin Transaction
                                    adminTransactionBO.AddNewAdminTransactionRequest(convTransac);

                                    //Log activity details for pending transaction
                                    InsertConversionActivityDetails(fromCurrID, toCurrID, amount, exchangeRate, fromAcc,
                                                                    toAcc, Constants.K_STATUS_PENDING);

                                    return
                                        Json(
                                            new
                                                {
                                                    status = true,
                                                    message = "Internal transfer request has been submitted."
                                                });
                                }
                            }
                            else
                            {
                                return
                                    Json(
                                        new
                                            {
                                                status = false,
                                                message = "Insufficient balance due to pending transfer requests!"
                                            });
                            }
                        }
                        else
                        {
                            return Json(new {status = false, message = "Transfer failed due to insufficient balance!"});
                        }
                    }
                    return Json(new {status = false, message = "No transaction settings found!"});
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new {status = false, message = "Some error occurred!"});
            }
        }

        /// <summary>
        /// This action returns exchange rate
        /// for a pair of currency
        /// </summary>
        /// <param name="fromCurr">fromCurr</param>
        /// <param name="toCurr">toCurr</param>
        /// <returns></returns>
        public decimal GetExchangeRates(string fromCurr, string toCurr)
        {
            try
            {
                var inverse = false;
                var exchangeRate = priceBO.GetExchangeRateForCurrencyPair(fromCurr, toCurr, ref inverse);

                var settings = transactionSettingBO.GetTransactionSetting((int)AdminTransactionType.ConversionsRequests, (int)SessionManagement.OrganizationID);

                //If settings not null
                if (settings != null)
                {
                    var markupType = (int)settings.ConversionMarkupType;
                    var markup = (decimal)settings.ConversionMarkupValue;

                    //Markup in percentage
                    if (markupType == (int)ConversionMarkupType.Percentage)
                    {
                        //Inverse calculation
                        if (inverse)
                        {
                            exchangeRate = Math.Round((1 / (exchangeRate + (exchangeRate * markup / 100))), 5);
                        }
                        else
                        {
                            exchangeRate = exchangeRate - (exchangeRate * markup / 100);
                        }
                        return exchangeRate;
                    }
                    //Markup in points
                    else if (markupType == (int)ConversionMarkupType.Points)
                    {
                        //Inverse calculation
                        if (inverse)
                        {
                            //JPY currency logic
                            if (fromCurr == "JPY" || toCurr == "JPY")
                            {
                                exchangeRate = Math.Round((1 / (exchangeRate + (markup * JpyPointMultiplier))), 5);
                            }
                            else
                            {
                                exchangeRate = Math.Round((1 / (exchangeRate + (markup * PointMultiplier))), 5);
                            }
                        }
                        else
                        {
                            //JPY currency logic
                            if (fromCurr == "JPY" || toCurr == "JPY")
                            {
                                exchangeRate = exchangeRate - (markup * JpyPointMultiplier);
                            }
                            else
                            {
                                exchangeRate = exchangeRate - (markup * PointMultiplier);
                            }
                        }
                        return exchangeRate;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action logs transfer activity details in database
        /// </summary>
        /// <param name="currID">currID</param>
        /// <param name="amount">amount</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertTransferActivityDetails(int currID, double amount, string fromAcc, string toAcc, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Insert in UserActivity
                    int pkActivityID = userActivityBO.InsertUserActivityDetails(loginInfo.UserID, (int)ActivityType.TransferActivity);

                    //Insert in TransferActivity
                    transActivityBO.InsertTransferActivityDetails(pkActivityID, currID, amount, fromAcc, toAcc, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action logs conversion activity details in database
        /// </summary>
        /// <param name="fromCurrID">fromCurrID</param>
        /// <param name="toCurrID">toCurrID</param>
        /// <param name="amount">amount</param>
        /// <param name="exchangeRate">exchangeRate</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertConversionActivityDetails(int fromCurrID, int toCurrID, double amount, double exchangeRate, string fromAcc, string toAcc, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Insert in UserActivity
                    int pkActivityID = userActivityBO.InsertUserActivityDetails(loginInfo.UserID, (int)ActivityType.ConversionActivity);

                    //Insert in TransferActivity
                    convActivityBO.InsertConversionActivityDetails(pkActivityID, fromCurrID, toCurrID, amount, exchangeRate, fromAcc, toAcc, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action logs deposit or withdraw activity details in database
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="currId">currId</param>
        /// <param name="amount">amount</param>
        /// <param name="accNumber">accNumber</param>
        /// <param name="bankId">bankId</param>
        /// <param name="status">status</param>
        public void InsertDepositOrWithdrawActivityDetails(string type, int currId, decimal amount, string accNumber, int? bankId, string status)
        {
            try
            {
                LoginInformation loginInfo = SessionManagement.UserInfo;

                //Insert in UserActivity
                int pkActivityId = userActivityBO.InsertUserActivityDetails(loginInfo.UserID,
                                                                            (int) ActivityType.DepositOrWithdrawActivity);

                //Insert in DepositOrWithdrawActivity
                depWithDrwActBO.InsertDepositOrWithdrawActivityDetails(pkActivityId, type, currId, amount, accNumber, bankId, status);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method credits or debits balance of platform accounts
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="amount">amount</param>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        public bool DoPlatformTransaction(int login, double amount, string comment)
        {
            try
            {
                var newTransac = new TradeTransInfoNET();
                newTransac.cmd = (short)TradeCommands.OP_BALANCE;
                newTransac.comment = comment;
                newTransac.orderby = login;
                newTransac.price = amount;
                newTransac.type = (short)TradeTransTypes.TT_BR_BALANCE;
                newTransac.reserved = 0;

                var manager = new MetaTraderWrapperManager("mtdem01.primexm.com:443", 900, "!FQS123!!");
                if (manager.IsConnected() == 1)
                {
                    if (manager.TradeTransaction(newTransac) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// This action returns fund sources accepting particular currency
        /// </summary>
        /// <param name="currSymbol">currSymbol</param>
        /// <returns></returns>
        public ActionResult GetFundingSourcesAsPerCurrency(string currSymbol)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get currency Id from symbol
                    int currId = lCurrValueBO.GetCurrencyIDFromSymbol(currSymbol);

                    //Get all sources ids accepting this currency
                    var sourceIds = fundSrcAccpCurrBO.GetAllTransferFundingSourceOfParticularCurrency(currId);

                    //Return all fund sources of this currency
                    return Json(fundSourceBO.GetAllClientTransferFundSources(sourceIds, (int)SessionManagement.OrganizationID), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action calculates if margin restriction and returns true
        /// if balance after transfer is greater than margin restriction
        /// </summary>
        /// <param name="loginId">loginId</param>
        /// <param name="amount">amount</param>
        /// <param name="balance">balance</param>
        /// <param name="marginRestriction">marginRestriction</param>
        /// <returns></returns>
        public bool IsMarginRestrictionSatisfied(int loginId, decimal amount, decimal balance, double marginRestriction)
        {
            try
            {
                var marginCache = new DataCache("MarginCache");
                
                //Get margin object for the login
                object objMargin = marginCache.Get(loginId.StringTryParse());
                
                if (objMargin != null)
                {
                    var margin = (Margin)objMargin;

                    //Add margin restriction percent to accountMargin
                    var accountMargin = (decimal)margin.Margin1;
                    accountMargin += ((accountMargin * (decimal)marginRestriction)/100);

                    return ((balance - amount) >= accountMargin) ? true : false;
                }

                return false;
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
