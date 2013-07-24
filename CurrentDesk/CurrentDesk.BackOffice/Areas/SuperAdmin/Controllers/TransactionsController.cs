#region Header Information
/*************************************************************************
 * File Name     : TransactionsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th June 2013
 * Modified Date : 11th June 2013
 * Description   : This file contains Transactions controller and related 
 *                 actions for super admin login
 * **********************************************************************/
#endregion

#region Namespace Used

using System.Linq;
using CurrentDesk.BackOffice.Areas.SuperAdmin.Models.Transactions;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Extension;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using MT4ManLibraryNETv03;
using MT4Wrapper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.ApplicationServer.Caching;
using CurrentDesk.BackOffice.Utilities;

#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Transactions controller and contains action
    /// methods for super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class TransactionsController : Controller
    {
        private const decimal PointMultiplier = 0.00001M;
        private const decimal JpyPointMultiplier = 0.001M;

        #region Variables
        private AdminTransactionBO adminTransactionBO = new AdminTransactionBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private TransactionBO transactionBO = new TransactionBO();
        private TransferLogBO transferlogBO = new TransferLogBO();
        private Client_AccountBO clientAccBO = new Client_AccountBO();
        private FundingSourceBO fundSourceBO = new FundingSourceBO();
        private L_CurrencyValueBO currencyBO = new L_CurrencyValueBO();
        private UserBO userBO = new UserBO();
        private BankAccountInformationBO bankInfoBO = new BankAccountInformationBO();
        private L_CountryBO lCountryBO = new L_CountryBO();
        private L_RecievingBankBO lReceivingBankBO = new L_RecievingBankBO();
        private TransactionSettingBO transactionSettingBO = new TransactionSettingBO();
        private TransferLogBO transferLogBO = new TransferLogBO();
        private PriceBO priceBO = new PriceBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private DepositOrWithdrawActivityBO depWithDrwActBO = new DepositOrWithdrawActivityBO();
        private TransferActivityBO transActivityBO = new TransferActivityBO();
        private ConversionActivityBO convActivityBO = new ConversionActivityBO();
        private FundingSourceAcceptedCurrencyBO fundSrcAccpCurrBO = new FundingSourceAcceptedCurrencyBO();
        #endregion

        #region Incoming Transaction Section
        /// <summary>
        /// This action returns Incoming Funds requests page of super admin
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Get setting from database
                    var incTransactionSett = transactionSettingBO.GetTransactionSetting((int)AdminTransactionType.IncomingFunds, organizationID);

                    if (incTransactionSett != null)
                    {
                        ViewData["SettingCurrency"] = incTransactionSett.FK_CurrencyID;
                        ViewData["MinAmount"] = incTransactionSett.MinimumDepositAmount;
                    }
                    ViewData["FundingSource"] = new SelectList(fundSourceBO.GetAllTransferFundSources(organizationID), "PK_FundingSourceID", "SourceName");
                    ViewData["Currency"] = new SelectList(currencyBO.GetCurrencies(), "PK_CurrencyValueID", "CurrencyValue");
                    ViewData["Clients"] = new SelectList(userBO.GetAllClientsOfBroker(organizationID), "UserID", "DisplayName");

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns list of incoming transactions
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllIncomingFundRequests()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    List<TransactionModel> lstIncomingTransactions = new List<TransactionModel>();

                    //Get all incoming transactions
                    var allIncomingRequests = adminTransactionBO.GetAllIncomingFundRequests((int)SessionManagement.OrganizationID);

                    //Iterate through each incoming transaction
                    foreach (var request in allIncomingRequests)
                    {
                        TransactionModel incTransaction = new TransactionModel();
                        incTransaction.TransactionDate = Convert.ToDateTime(request.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        incTransaction.TransactionID = request.PK_TransactionID;
                        incTransaction.AccountNumber = request.AccountNumber;
                        incTransaction.ClientName = request.ClientName ?? String.Empty;
                        incTransaction.FundingSourceName = request.FundingSource.SourceName;
                        incTransaction.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)request.FK_CurrencyID);
                        incTransaction.TransactionAmount = Utility.FormatCurrencyValue((decimal)request.TransactionAmount, "");
                        incTransaction.TransactionFee = request.FeeAmount == null ? Utility.FormatCurrencyValue((decimal)request.FundingSource.IncomingWireFeeAmount, "") : Utility.FormatCurrencyValue((decimal)request.FeeAmount, "");
                        incTransaction.Actions = "<button class='btn btn-mini' data-modal='modalApprove' onclick='showModalApprove(" + request.PK_TransactionID + ")'>Approve</button><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteTransaction(" + request.PK_TransactionID + ")'>";

                        lstIncomingTransactions.Add(incTransaction);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstIncomingTransactions.Count,
                        rows = lstIncomingTransactions
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns transaction details
        /// </summary>
        /// <param name="pkTransactionID">pkTransactionID</param>
        /// <returns></returns>
        public JsonResult GetIncomingTransactionDetails(int pkTransactionID)
        {
            try
            {
                //Get transaction details
                var transaction = adminTransactionBO.GetTransactionDetails(pkTransactionID);
                TransactionModel transacDetail = new TransactionModel();

                if (transaction != null)
                {
                    transacDetail.PK_TransactionID = transaction.PK_TransactionID;
                    transacDetail.ClientName = transaction.ClientName;
                    transacDetail.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FK_CurrencyID);
                    transacDetail.FeeCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FundingSource.FK_IncomingWireFeeCurrency);
                    transacDetail.Notes = transaction.Notes;
                }

                return Json(transacDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action approves a particular transaction
        /// </summary>
        /// <param name="approveTransaction">approveTransaction</param>
        /// <returns></returns>
        public ActionResult ApproveIncomingTransaction(AdminTransaction approveTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, approveTransaction.ClientName))
                    {
                        //Get final amount after deducting fees
                        decimal amountAfterDeduction = (decimal)(approveTransaction.TransactionAmount - approveTransaction.FeeAmount);

                        //Get transaction request
                        var transaction = adminTransactionBO.GetTransactionDetails(approveTransaction.PK_TransactionID);

                        //Entry in transaction table
                        var pkTransactionID = transactionBO.FundDeposit(transaction.AccountNumber, (int)transaction.FK_CurrencyID, amountAfterDeduction, transaction.Notes, organizationID);

                        //Entry in transfer log table
                        transferlogBO.AddTransferLogForFundDeposit(pkTransactionID, (int)transaction.FK_CurrencyID, amountAfterDeduction, transaction.AccountNumber, organizationID);

                        //Credit amount to account and set IsApproved true
                        if (clientAccBO.CreditLandingAccount(transaction.AccountNumber, amountAfterDeduction, organizationID))
                        {
                            //Set Approve in AdminTransaction table 
                            if (adminTransactionBO.ApproveIncomingTransaction(approveTransaction))
                            {
                                //Log in activity table
                                InsertDepositOrWithdrawActivityDetails((int)transaction.FK_UserID, Constants.K_DEPOSIT, (int)transaction.FK_CurrencyID, (decimal)approveTransaction.TransactionAmount, transaction.AccountNumber, null, Constants.K_STATUS_TRANSFERRED);
                                
                                return Json(true);
                            }
                        }
                    }

                    return Json(false);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action creates new transaction for a client
        /// </summary>
        /// <param name="newTransaction">newTransaction</param>
        /// <returns></returns>
        public ActionResult CreateNewIncomingTransactionForClient(NewTransactionModel newTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, newTransaction.AdminPassword))
                    {
                        AdminTransaction transaction = new AdminTransaction();
                        transaction.TransactionDate = DateTime.UtcNow;
                        transaction.FK_UserID = newTransaction.ClientUserID;
                        transaction.AccountNumber = newTransaction.ClientAccountNumber;
                        transaction.FK_FundingSourceID = newTransaction.FundingSourceID;
                        transaction.FK_CurrencyID = newTransaction.CurrencyID;
                        transaction.TransactionAmount = newTransaction.Amount;
                        transaction.FK_AdminTransactionTypeID = (int)AdminTransactionType.IncomingFunds;
                        transaction.Notes = newTransaction.Notes;
                        transaction.ClientName = newTransaction.ClientName;
                        transaction.FeeAmount = newTransaction.Fee;
                        transaction.FK_OrganizationID = (int) SessionManagement.OrganizationID;
                        transaction.IsApproved = false;
                        transaction.IsDeleted = false;

                        return Json(adminTransactionBO.AddNewAdminTransactionRequest(transaction));
                    }

                    return Json(false);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action adds or updates Incoming Transaction settings in database
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <param name="minDepositAmt">minDepositAmt</param>
        /// <param name="adminPassword">adminPassword</param>
        /// <returns></returns>
        public ActionResult SaveIncomingTransactionSettings(int currencyID, decimal minDepositAmt, string adminPassword)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, adminPassword))
                    {
                        TransactionSetting setting = new TransactionSetting();
                        setting.FK_CurrencyID = currencyID;
                        setting.MinimumDepositAmount = minDepositAmt;
                        setting.FK_AdminTransactionTypeID = (int)AdminTransactionType.IncomingFunds;
                        setting.FK_OrganizationID = (int) SessionManagement.OrganizationID;

                        //Add or update settings
                        return Json(new { status = transactionSettingBO.AddOrUpdateTransactionSetting(setting)});
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!"});
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }
        #endregion

        #region Outgoing Transaction Section
        /// <summary>
        /// This action returns OutgoingFunds view
        /// </summary>
        /// <returns></returns>
        public ActionResult OutgoingFunds()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["Clients"] = new SelectList(userBO.GetAllClientsOfBroker((int)SessionManagement.OrganizationID), "UserID", "DisplayName");
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns list of outgoing transactions
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllOutgoingFundRequests()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    List<TransactionModel> lstIncomingTransactions = new List<TransactionModel>();

                    //Get all outgoing transactions
                    var allOutgoingRequests = adminTransactionBO.GetAllOutgoingFundRequests(organizationID);

                    //Iterate through each outgoing transaction
                    foreach (var request in allOutgoingRequests)
                    {
                        var sourceIds = fundSrcAccpCurrBO.GetAllTransferFundingSourceOfParticularCurrency((int)request.FK_CurrencyID);
                        
                        //Get all funding sources
                        var allFundingSources = fundSourceBO.GetFundingSourcesFromIDs(sourceIds, organizationID);
                        
                        var outTransaction = new TransactionModel();
                        outTransaction.TransactionDate = Convert.ToDateTime(request.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        outTransaction.TransactionID = request.PK_TransactionID;
                        outTransaction.AccountNumber = request.AccountNumber;
                        outTransaction.ClientName = request.ClientName ?? String.Empty;
                        outTransaction.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)request.FK_CurrencyID);
                        outTransaction.TransactionAmount = Utility.FormatCurrencyValue((decimal)request.TransactionAmount, "");
                        outTransaction.WithdrawSource = "<a href='#' data-modal='modalSource' onclick='openWithdrawSource(" + request.FK_BankInfoID + ")'>" + request.BankAccountInformation.BankName + "</a>";
                        outTransaction.Actions = "<button class='btn btn-mini' data-modal='modalApprove' onclick='approveOutgoingTransaction(" + request.PK_TransactionID + ")'>Approve</button><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteTransaction(" + request.PK_TransactionID + ")'>";
                        outTransaction.FundingSourceName = "<select id='drpSource" + request.PK_TransactionID + "' class='chzn-select width-150' onchange='changeFundingSource(" + request.PK_TransactionID + ")'><option></option>";

                        //Add all funding sources in options
                        foreach (var source in allFundingSources)
                        {
                            outTransaction.FundingSourceName += "<option value='" + source.PK_FundingSourceID + "/" + Utility.FormatCurrencyValue((decimal)source.OutgoingWireFeeAmount, "") + "/" + lCurrValueBO.GetCurrencySymbolFromID((int)source.FK_OutgoingWireFeeCurrency) + "'>" + source.SourceName + "</option>";
                        }
                        outTransaction.FundingSourceName += "</select>";

                        lstIncomingTransactions.Add(outTransaction);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstIncomingTransactions.Count,
                        rows = lstIncomingTransactions
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns particular bank info details
        /// </summary>
        /// <param name="pkBankInfoID">pkBankInfoID</param>
        /// <returns></returns>
        public JsonResult GetWithdrawBankInfoDetails(int pkBankInfoID)
        {
            try
            {
                BankInfoDetails details = new BankInfoDetails();

                //Get particular bank details
                var bankDetails = bankInfoBO.GetBankAccountDetails(pkBankInfoID);

                if (bankDetails != null)
                {
                    details.BankName = bankDetails.BankName;
                    details.AccountNumber = bankDetails.AccountNumber;
                    details.BicOrSwiftCode = bankDetails.BicNumberOrSwiftCode;
                    details.ReceivingBankInfo = lReceivingBankBO.GetSelectedRecievingBankInfo((int)bankDetails.FK_ReceivingBankInformationID) + " " + bankDetails.ReceivingBankInfo;
                    details.BankAddress = bankDetails.BankingAddress.Split('@')[0] + ", " + bankDetails.BankingAddress.Split('@')[1];
                    details.BankCity = bankDetails.City;
                    details.BankCountry = lCountryBO.GetSelectedCountry((int)bankDetails.FK_CountryID);
                    details.BankPostalCode = bankDetails.PostalCode;
                }

                return Json(details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns outgoing transaction details
        /// </summary>
        /// <param name="pkTransactionID">pkTransactionID</param>
        /// <param name="amount">amount</param>
        /// <param name="fee">fee</param>
        /// <returns></returns>
        public JsonResult GetOutgoingTransactionDetails(int pkTransactionID, decimal amount, decimal fee)
        {
            try
            {
                //Get transaction details
                var transaction = adminTransactionBO.GetTransactionDetails(pkTransactionID);

                OutgoingTransactionApproveModel transacDetail = new OutgoingTransactionApproveModel();

                if (transaction != null)
                {
                    transacDetail.PK_TransactionID = transaction.PK_TransactionID;
                    transacDetail.ClientName = transaction.ClientName;
                    transacDetail.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FK_CurrencyID);
                    transacDetail.Notes = transaction.Notes;
                    transacDetail.BankName = transaction.BankAccountInformation.BankName;
                    transacDetail.AccountNumber = transaction.BankAccountInformation.AccountNumber;
                    transacDetail.BicOrSwiftCode = transaction.BankAccountInformation.BicNumberOrSwiftCode;
                    transacDetail.ReceivingBankInfo = lReceivingBankBO.GetSelectedRecievingBankInfo((int)transaction.BankAccountInformation.FK_ReceivingBankInformationID) + " " + transaction.BankAccountInformation.ReceivingBankInfo;
                    transacDetail.BankAddress = transaction.BankAccountInformation.BankingAddress.Split('@')[0] + ", " + transaction.BankAccountInformation.BankingAddress.Split('@')[1];
                    transacDetail.BankCity = transaction.BankAccountInformation.City;
                    transacDetail.BankCountry = lCountryBO.GetSelectedCountry((int)transaction.BankAccountInformation.FK_CountryID);
                    transacDetail.BankPostalCode = transaction.BankAccountInformation.PostalCode;
                    transacDetail.TotalAmount = Utility.FormatCurrencyValue((amount - fee), "");
                }

                return Json(transacDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action method approves an outgoing transaction
        /// </summary>
        /// <param name="apprvOutTransaction">apprvOutTransaction</param>
        /// <returns></returns>
        public ActionResult ApproveOutgoingTransaction(OutgoingTransactionApproveModel apprvOutTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, apprvOutTransaction.AdminPassword))
                    {
                        //Get transaction request
                        var transaction = adminTransactionBO.GetTransactionDetails(apprvOutTransaction.PK_TransactionID);

                        //Get acc details
                        var accDetails = clientAccBO.GetAnyAccountDetails(transaction.AccountNumber, organizationID);

                        //Check balance
                        if (accDetails != null && accDetails.CurrentBalance >= (apprvOutTransaction.TransactionAmount + apprvOutTransaction.FeeAmount))
                        {
                            //Debit amount from account, add logs and set IsApproved true
                            if (clientAccBO.DebitLandingAccount(transaction.AccountNumber, apprvOutTransaction.TransactionAmount, organizationID))
                            {
                                AdminTransaction outTransaction = new AdminTransaction();
                                outTransaction.PK_TransactionID = apprvOutTransaction.PK_TransactionID;
                                outTransaction.TransactionAmount = apprvOutTransaction.TransactionAmount;
                                outTransaction.FeeAmount = apprvOutTransaction.FeeAmount;
                                outTransaction.Notes = apprvOutTransaction.Notes;
                                outTransaction.FK_FundingSourceID = apprvOutTransaction.FundSourceID;

                                //Entry in transaction table
                                var pkTransactionID = transactionBO.FundWithdraw(transaction.AccountNumber, (int)transaction.FK_CurrencyID, apprvOutTransaction.TransactionAmount, transaction.Notes, organizationID);

                                //Entry in transfer log table
                                transferlogBO.AddTransferLogForFundWithdraw(pkTransactionID, (int)transaction.FK_CurrencyID, apprvOutTransaction.TransactionAmount, transaction.AccountNumber, organizationID);

                                //Set approve in AdminTransaction table
                                if (adminTransactionBO.ApproveOutgoingTransaction(outTransaction))
                                {
                                    //Log in activity table
                                    InsertDepositOrWithdrawActivityDetails((int)transaction.FK_UserID, Constants.K_WITHDRAW, (int)transaction.FK_CurrencyID, apprvOutTransaction.TransactionAmount, transaction.AccountNumber, transaction.FK_BankInfoID, Constants.K_STATUS_TRANSFERRED);

                                    return Json(new {status = true});
                                }
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance!"});
                        }
                    }

                    return Json(new { status = false, message = "Invalid password!" });
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This method returns list of bank accs of client
        /// </summary>
        /// <param name="pkClientUserID"></param>
        /// <returns></returns>
        public JsonResult GetAllBankAccountsOfClient(int pkClientUserID)
        {
            try
            {
                List<BankAccountDetails> lstClientBanks = new List<BankAccountDetails>();
                List<BankAccountInformation> bankInfos;

                //Get client details
                var clientDetails = userBO.GetUserDetails(pkClientUserID);

                //Live client
                if (clientDetails.FK_UserTypeID == Constants.K_BROKER_LIVE)
                {
                    //Get all bank accounts of client
                    bankInfos = bankInfoBO.GetAllBankInfosForUser(LoginAccountType.LiveAccount, pkClientUserID);
                }
                //Partner client
                else
                {
                    //Get all bank accounts of partner
                    bankInfos = bankInfoBO.GetAllBankInfosForUser(LoginAccountType.PartnerAccount, pkClientUserID);
                }

                //Iterate through each acc
                foreach (var info in bankInfos)
                {
                    BankAccountDetails bankAcc = new BankAccountDetails();
                    bankAcc.BankID = info.PK_BankAccountInformationID;
                    bankAcc.BankName = info.BankName;
                    bankAcc.BankAccount = info.AccountNumber;

                    lstClientBanks.Add(bankAcc);
                }

                return Json(lstClientBanks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action creates new outgoing transaction for client
        /// </summary>
        /// <param name="newOutTransaction">newOutTransaction</param>
        /// <returns></returns>
        public ActionResult CreateNewOutGoingTransactionForClient(NewTransactionModel newOutTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, newOutTransaction.AdminPassword))
                    {
                        AdminTransaction transaction = new AdminTransaction();
                        transaction.TransactionDate = DateTime.UtcNow;
                        transaction.FK_UserID = newOutTransaction.ClientUserID;
                        transaction.AccountNumber = newOutTransaction.ClientAccountNumber;
                        transaction.FK_BankInfoID = newOutTransaction.BankID;
                        transaction.FK_CurrencyID = lCurrValueBO.GetCurrencyIDFromSymbol(newOutTransaction.CurrencySymbol);
                        transaction.TransactionAmount = newOutTransaction.Amount;
                        transaction.FK_AdminTransactionTypeID = (int)AdminTransactionType.OutgoingFunds;
                        transaction.Notes = newOutTransaction.Notes;
                        transaction.ClientName = newOutTransaction.ClientName;
                        transaction.FK_OrganizationID = (int) SessionManagement.OrganizationID;
                        transaction.IsApproved = false;
                        transaction.IsDeleted = false;

                        return Json(adminTransactionBO.AddNewAdminTransactionRequest(transaction));
                    }

                    return Json(false);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }
        #endregion

        #region Internal Transfers Section
        /// <summary>
        /// This action returns InternalTransfers view
        /// </summary>
        /// <returns></returns>
        public ActionResult InternalTransfers()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;
                    InternalTransferModel model = new InternalTransferModel();

                    //Get settings from database
                    var settings = transactionSettingBO.GetTransactionSetting((int)AdminTransactionType.InternalTransfers, organizationID);

                    if (settings != null)
                    {
                        model.TransferCurrencyID = (int)settings.FK_CurrencyID;
                        model.TransferFee = (decimal)settings.TransferFee;
                        model.ApprovalOptionID = (int)settings.InternalTransferApprovalOptions;
                        model.LimitedAmount = settings.InternalTransferLimitedAmount;
                        model.MarginRestriction = (float)settings.MarginRestriction;

                    }

                    ViewData["Currency"] = new SelectList(currencyBO.GetCurrencies(), "PK_CurrencyValueID", "CurrencyValue");
                    ViewData["Approval"] = new SelectList(ExtensionUtility.GetAllApprovalOptions(), "ID", "Value");
                    ViewData["Clients"] = new SelectList(userBO.GetAllClientsOfBroker(organizationID), "UserID", "DisplayName");

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action saves or updates internal transfer settings in database
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <param name="transferFeeAmt">transferFeeAmt</param>
        /// <param name="approvalOption">approvalOption</param>
        /// <param name="limitedAmt">limitedAmt</param>
        /// <param name="marginRestriction">marginRestriction</param>
        /// <param name="adminPassword">adminPassword</param>
        /// <returns></returns>
        public ActionResult SaveInternalTransferSettings(int currencyID, decimal transferFeeAmt, int approvalOption, decimal? limitedAmt, float marginRestriction, string adminPassword)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, adminPassword))
                    {
                        TransactionSetting setting = new TransactionSetting();
                        setting.FK_CurrencyID = currencyID;
                        setting.TransferFee = transferFeeAmt;
                        setting.InternalTransferApprovalOptions = approvalOption;
                        setting.InternalTransferLimitedAmount = limitedAmt;
                        setting.MarginRestriction = marginRestriction;
                        setting.FK_AdminTransactionTypeID = (int)AdminTransactionType.InternalTransfers;
                        setting.FK_OrganizationID = (int) SessionManagement.OrganizationID;

                        //Add or update settings
                        return Json(new { status = transactionSettingBO.AddOrUpdateTransactionSetting(setting) });
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This method returns all pending internal transfer requests
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllInternalTransferRequest()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    List<TransactionModel> lstInternalTransactions = new List<TransactionModel>();

                    //Get all incoming transactions
                    var allInternalTransferRequests = adminTransactionBO.GetAllInternalTransferRequests((int)SessionManagement.OrganizationID);

                    //Iterate through each internal transaction
                    foreach (var request in allInternalTransferRequests)
                    {
                        TransactionModel internalTransaction = new TransactionModel();
                        internalTransaction.TransactionDate = Convert.ToDateTime(request.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        internalTransaction.PK_TransactionID = request.PK_TransactionID;
                        internalTransaction.AccountNumber = request.AccountNumber;
                        internalTransaction.ClientName = request.ClientName ?? String.Empty;
                        internalTransaction.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)request.FK_CurrencyID);
                        internalTransaction.TransactionAmount = Utility.FormatCurrencyValue((decimal)request.TransactionAmount, "");
                        internalTransaction.TransactionFee = request.FeeAmount == null ? Utility.FormatCurrencyValue((decimal)request.FundingSource.IncomingWireFeeAmount, "") : Utility.FormatCurrencyValue((decimal)request.FeeAmount, "");
                        internalTransaction.ToAccount = request.ToAccountNumber;
                        internalTransaction.ToClientName = request.ToClientName;
                        internalTransaction.Actions = "<button class='btn btn-mini' data-modal='modalApprove' onclick='showModalApprove(" + request.PK_TransactionID + ")'>Approve</button><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteTransaction(" + request.PK_TransactionID + ")'>";

                        lstInternalTransactions.Add(internalTransaction);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstInternalTransactions.Count,
                        rows = lstInternalTransactions
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns internal transfer transaction details
        /// </summary>
        /// <param name="pkTransactionID">pkTransactionID</param>
        /// <returns></returns>
        public JsonResult GetInternalTransferDetails(int pkTransactionID)
        {
            try
            {
                //Get transaction details
                var transaction = adminTransactionBO.GetTransactionDetails(pkTransactionID);
                TransactionModel transacDetail = new TransactionModel();

                if (transaction != null)
                {
                    transacDetail.PK_TransactionID = transaction.PK_TransactionID;
                    transacDetail.ClientName = transaction.ClientName;
                    transacDetail.AccountNumber = transaction.AccountNumber;
                    transacDetail.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FK_CurrencyID);
                    transacDetail.ToClientName = transaction.ToClientName;
                    transacDetail.ToAccount = transaction.ToAccountNumber;
                    transacDetail.Notes = transaction.Notes;
                }

                return Json(transacDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action approves a particular internal transfer transaction
        /// </summary>
        /// <param name="approveTransaction">approveTransaction</param>
        /// <returns></returns>
        public ActionResult ApproveInternalTransferTransaction(AdminTransaction approveTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, approveTransaction.ClientName))
                    {
                        bool isToSucessful = true;
                        bool isFromSucessful = true;

                        //Get transaction request details
                        var transaction = adminTransactionBO.GetTransactionDetails(approveTransaction.PK_TransactionID);

                        //From and to account details
                        var fromAccDetails = clientAccBO.GetAnyAccountDetails(transaction.AccountNumber, organizationID);
                        var toAccDetails = clientAccBO.GetAnyAccountDetails(transaction.ToAccountNumber, organizationID);

                        if (transaction != null && fromAccDetails.CurrentBalance >= (approveTransaction.TransactionAmount + approveTransaction.FeeAmount))
                        {
                            if (fromAccDetails.PlatformLogin != null)
                            {
                                isFromSucessful = DoPlatformTransaction((int)fromAccDetails.PlatformLogin, -(double)approveTransaction.TransactionAmount, "Debit");

                                //Debit fee if above transaction is successful
                                if (isFromSucessful && approveTransaction.FeeAmount != 0)
                                {
                                    isFromSucessful = DoPlatformTransaction((int)fromAccDetails.PlatformLogin, -(double)approveTransaction.FeeAmount, "Debit Fee");
                                }
                            }

                            if (toAccDetails.PlatformLogin != null && isFromSucessful)
                            {
                                isToSucessful = DoPlatformTransaction((int)toAccDetails.PlatformLogin, (double)approveTransaction.TransactionAmount, "Credit");
                            }

                            //If platform transactions are successful
                            if (isToSucessful && isFromSucessful)
                            {
                                //Do actual transfer of funds and set IsApproved true
                                if (clientAccBO.TransferUserFund(transaction.AccountNumber, transaction.ToAccountNumber, (double)approveTransaction.TransactionAmount, (double)approveTransaction.FeeAmount, 1, organizationID))
                                {
                                    //Log in transaction table
                                    var pkTransactionID = transactionBO.InternalFundTransfer(transaction.AccountNumber, transaction.ToAccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)approveTransaction.TransactionAmount, 1, approveTransaction.Notes, organizationID);

                                    //Logs fund transfers details(Withdrawal/Deposit) in TransferLogs table
                                    transferLogBO.AddTransferLogForTransaction(pkTransactionID, transaction.AccountNumber, transaction.ToAccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)approveTransaction.TransactionAmount, 1, organizationID);

                                    //Log fee transfer if fee > 0
                                    if (approveTransaction.FeeAmount != 0)
                                    {
                                        //Log fee deduction in transaction table
                                        var pkFeeTransactionID = transactionBO.InternalFeeTransaction(transaction.AccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)approveTransaction.FeeAmount, organizationID);

                                        //Logs fee transfers details in TransferLogs table
                                        transferLogBO.AddTransferLogForFee(pkFeeTransactionID, transaction.AccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)approveTransaction.FeeAmount, organizationID);
                                    }

                                    //Set status to Approve
                                    if (adminTransactionBO.ApproveIncomingTransaction(approveTransaction))
                                    {
                                        //Same client transfer
                                        if (transaction.AccountNumber.Split('-')[2] ==
                                            transaction.ToAccountNumber.Split('-')[2])
                                        {
                                            //Log activity details
                                            InsertTransferActivityDetails((int) transaction.FK_UserID,
                                                                          (int) transaction.FK_CurrencyID,
                                                                          (double) approveTransaction.TransactionAmount,
                                                                          transaction.AccountNumber,
                                                                          transaction.ToAccountNumber,
                                                                          Constants.K_STATUS_TRANSFERRED);
                                        }
                                            //Inter client transfer
                                        else
                                        {
                                            //Log activity details
                                            InsertInterClientTransferActivityDetails((int) transaction.FK_UserID,
                                                                                     (int) transaction.FK_ToUserID,
                                                                                     (int) transaction.FK_CurrencyID,
                                                                                     (double) approveTransaction
                                                                                                  .TransactionAmount,
                                                                                     transaction.AccountNumber,
                                                                                     transaction.ToAccountNumber,
                                                                                     Constants.K_STATUS_TRANSFERRED);
                                        }

                                        return Json(new {status = true});
                                    }
                                }
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance!" });
                        }
                    }

                    return Json(new { status = false, message = "Invalid password!"});
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This action creates new internal transfer transaction
        /// </summary>
        /// <param name="newTransfer">newTransfer</param>
        /// <returns></returns>
        public ActionResult CreateNewInternalTransferTransaction(NewTransferTransaction newTransfer)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, newTransfer.AdminPassword))
                    {
                        //Get balance for accounts
                        decimal accBalance = clientAccBO.GetAccountBalance(newTransfer.FromClientAccount, organizationID);
                        decimal pendingTransactionAmount = adminTransactionBO.GetPendingTransferAmount(newTransfer.FromClientAccount, organizationID);
                        
                        //Check balance
                        if (accBalance >= newTransfer.TransactionAmount)
                        {
                            //Check pending requests balance
                            if (accBalance >= (pendingTransactionAmount + newTransfer.TransactionAmount))
                            {
                                //Create new request
                                AdminTransaction newIntTransac = new AdminTransaction();
                                newIntTransac.TransactionDate = DateTime.UtcNow;
                                newIntTransac.FK_UserID = newTransfer.FromClientUserID;
                                newIntTransac.AccountNumber = newTransfer.FromClientAccount;
                                newIntTransac.FK_CurrencyID = lCurrValueBO.GetCurrencyIDFromSymbol(newTransfer.Currency);
                                newIntTransac.TransactionAmount = newTransfer.TransactionAmount;
                                newIntTransac.FK_AdminTransactionTypeID = (int)AdminTransactionType.InternalTransfers;
                                newIntTransac.Notes = newTransfer.Notes;
                                newIntTransac.ClientName = newTransfer.FromClientName;
                                newIntTransac.FeeAmount = newTransfer.TransactionFee;
                                newIntTransac.ToAccountNumber = newTransfer.ToClientAccount;
                                newIntTransac.ToClientName = newTransfer.ToClientName;
                                newIntTransac.FK_ToUserID = newTransfer.ToClientUserID;
                                newIntTransac.FK_OrganizationID = organizationID;
                                newIntTransac.IsApproved = false;
                                newIntTransac.IsDeleted = false;

                                //Add new request
                                adminTransactionBO.AddNewAdminTransactionRequest(newIntTransac);

                                return Json(new { status = true });
                            }
                            else
                            {
                                return Json(new { status = false, message = "Insufficient balance due to pending transfer requests!" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!"});
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This action logs transfer activity details in database
        /// </summary>
        /// <param name="clientUserId">clientUserId</param>
        /// <param name="currId">currId</param>
        /// <param name="amount">amount</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertTransferActivityDetails(int clientUserId, int currId, double amount, string fromAcc, string toAcc, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Insert in UserActivity
                    int pkActivityId = userActivityBO.InsertUserActivityDetails(clientUserId, (int)ActivityType.TransferActivity);

                    //Insert in TransferActivity
                    transActivityBO.InsertTransferActivityDetails(pkActivityId, currId, amount, fromAcc, toAcc, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action inserts inter client transfer activity log
        /// </summary>
        /// <param name="fromUserId">fromUserId</param>
        /// <param name="toUserId">toUserId</param>
        /// <param name="currId">currId</param>
        /// <param name="amount">amount</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertInterClientTransferActivityDetails(int fromUserId, int toUserId, int currId, double amount,
                                                             string fromAcc, string toAcc, string status)
        {
            try
            {
                //Insert in UserActivity
                int pkActivityFromId = userActivityBO.InsertUserActivityDetails(fromUserId, (int)ActivityType.TransferActivity);
                int pkActivityToId = userActivityBO.InsertUserActivityDetails(toUserId, (int)ActivityType.TransferActivity);

                //Insert in TransferActivity
                transActivityBO.InsertInterClientTransferActivityDetails(pkActivityFromId, null, toUserId, currId, amount, fromAcc, toAcc, status);
                transActivityBO.InsertInterClientTransferActivityDetails(pkActivityToId, fromUserId, null, currId, amount, fromAcc, toAcc, status);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }
        #endregion

        #region Conversion Requests Section
        /// <summary>
        /// This action returns ConversionRequests view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult ConversionsRequests()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;
                    InternalTransferModel model = new InternalTransferModel();

                    //Get settings from database
                    var settings = transactionSettingBO.GetTransactionSetting((int)AdminTransactionType.ConversionsRequests, organizationID);

                    if (settings != null)
                    {
                        model.TransferCurrencyID = (int)settings.FK_CurrencyID;
                        model.TransferFee = (decimal)settings.TransferFee;
                        model.ApprovalOptionID = (int)settings.InternalTransferApprovalOptions;
                        model.LimitedAmount = settings.InternalTransferLimitedAmount;
                        model.ConversionMarkupType = (int)settings.ConversionMarkupType;
                        model.ConversionMarkupValue = (double)settings.ConversionMarkupValue;
                        model.MarginRestriction = (float)settings.MarginRestriction;

                    }

                    ViewData["Currency"] = new SelectList(currencyBO.GetCurrencies(), "PK_CurrencyValueID", "CurrencyValue");
                    ViewData["Approval"] = new SelectList(ExtensionUtility.GetAllApprovalOptions(), "ID", "Value");
                    ViewData["Clients"] = new SelectList(userBO.GetAllClientsOfBroker(organizationID), "UserID", "DisplayName");

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action saves or updates conversion settings in database
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <param name="convFeeAmt">convFeeAmt</param>
        /// <param name="approvalOption">approvalOption</param>
        /// <param name="limitedAmt">limitedAmt</param>
        /// <param name="convMarkupType">convMarkupType</param>
        /// <param name="convMarkupValue">convMarkupValue</param>
        /// <param name="marginRestriction">marginRestriction</param>
        /// <param name="adminPassword">adminPassword</param>
        /// <returns></returns>
        public ActionResult SaveConversionSettings(int currencyID, decimal convFeeAmt, int approvalOption, decimal? limitedAmt, int convMarkupType, double convMarkupValue, float marginRestriction, string adminPassword)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, adminPassword))
                    {
                        TransactionSetting setting = new TransactionSetting();
                        setting.FK_CurrencyID = currencyID;
                        setting.TransferFee = convFeeAmt;
                        setting.InternalTransferApprovalOptions = approvalOption;
                        setting.InternalTransferLimitedAmount = limitedAmt;
                        setting.ConversionMarkupType = convMarkupType;
                        setting.ConversionMarkupValue = convMarkupValue;
                        setting.MarginRestriction = marginRestriction;
                        setting.FK_AdminTransactionTypeID = (int)AdminTransactionType.ConversionsRequests;
                        setting.FK_OrganizationID = (int) SessionManagement.OrganizationID;

                        //Add or update settings
                        return Json(new { status = transactionSettingBO.AddOrUpdateTransactionSetting(setting) });
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This method returns all pending conversion requests
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllConversionRequest()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    List<TransactionModel> lstInternalTransactions = new List<TransactionModel>();

                    //Get all incoming transactions
                    var allConversionRequests = adminTransactionBO.GetAllConversionRequests((int)SessionManagement.OrganizationID);

                    //Iterate through each internal transaction
                    foreach (var request in allConversionRequests)
                    {
                        TransactionModel convTransaction = new TransactionModel();
                        convTransaction.TransactionDate = Convert.ToDateTime(request.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        convTransaction.PK_TransactionID = request.PK_TransactionID;
                        convTransaction.AccountNumber = request.AccountNumber;
                        convTransaction.ClientName = request.ClientName ?? String.Empty;
                        convTransaction.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)request.FK_CurrencyID);
                        convTransaction.TransactionAmount = Utility.FormatCurrencyValue((decimal)request.TransactionAmount, "");
                        convTransaction.TransactionFee = Utility.FormatCurrencyValue((decimal)request.FeeAmount, "");
                        convTransaction.ToAccount = request.ToAccountNumber;
                        convTransaction.ToClientName = request.ToClientName;
                        convTransaction.ExchangeRate = (double)request.ExchangeRate;
                        convTransaction.ExchangedAmount = Utility.FormatCurrencyValue(Math.Round((decimal)(request.TransactionAmount * (decimal)request.ExchangeRate), 2), "");
                        convTransaction.ToCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)request.FK_ToCurrencyID);
                        convTransaction.Actions = "<button class='btn btn-mini' data-modal='modalApprove' onclick='showModalApprove(" + request.PK_TransactionID + ")'>Approve</button><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteTransaction(" + request.PK_TransactionID + ")'>";

                        lstInternalTransactions.Add(convTransaction);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstInternalTransactions.Count,
                        rows = lstInternalTransactions
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns conversion transaction request details
        /// </summary>
        /// <param name="pkTransactionId">pkTransactionId</param>
        /// <param name="amount">amount</param>
        /// <returns></returns>
        public JsonResult GetConversionRequestDetails(int pkTransactionId, decimal amount)
        {
            try
            {
                //Get transaction details
                var transaction = adminTransactionBO.GetTransactionDetails(pkTransactionId);
                TransactionModel transacDetail = new TransactionModel();

                if (transaction != null)
                {
                    transacDetail.PK_TransactionID = transaction.PK_TransactionID;
                    transacDetail.ClientName = transaction.ClientName;
                    transacDetail.AccountNumber = transaction.AccountNumber;
                    transacDetail.Currency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FK_CurrencyID);
                    transacDetail.ToClientName = transaction.ToClientName;
                    transacDetail.ToAccount = transaction.ToAccountNumber;
                    transacDetail.ExchangeRate = (double)transaction.ExchangeRate;
                    transacDetail.ToCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)transaction.FK_ToCurrencyID);
                    transacDetail.ExchangedAmount = Utility.FormatCurrencyValue((amount * (decimal)transaction.ExchangeRate), "");

                    transacDetail.Notes = transaction.Notes;
                }

                return Json(transacDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action approves conversion request transaction
        /// </summary>
        /// <param name="convTransaction">convTransaction</param>
        /// <returns></returns>
        public ActionResult ApproveConversionTransaction(AdminTransaction convTransaction)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, convTransaction.ClientName))
                    {
                        bool isToSucessful = true;
                        bool isFromSucessful = true;

                        //Get transaction request details
                        var transaction = adminTransactionBO.GetTransactionDetails(convTransaction.PK_TransactionID);

                        //From and to account details
                        var fromAccDetails = clientAccBO.GetAnyAccountDetails(transaction.AccountNumber, organizationID);
                        var toAccDetails = clientAccBO.GetAnyAccountDetails(transaction.ToAccountNumber, organizationID);

                        if (transaction != null && fromAccDetails.CurrentBalance >= (convTransaction.TransactionAmount + convTransaction.FeeAmount))
                        {
                            if (fromAccDetails.PlatformLogin != null)
                            {
                                isFromSucessful = DoPlatformTransaction((int)fromAccDetails.PlatformLogin, -(double)convTransaction.TransactionAmount, "Debit");

                                //Debit fee if above transaction is successful
                                if (isFromSucessful && convTransaction.FeeAmount != 0)
                                {
                                    isFromSucessful = DoPlatformTransaction((int)fromAccDetails.PlatformLogin, -(double)convTransaction.FeeAmount, "Debit Fee");
                                }
                            }

                            if (toAccDetails.PlatformLogin != null && isFromSucessful)
                            {
                                isToSucessful = DoPlatformTransaction((int)toAccDetails.PlatformLogin, Math.Round(((double)convTransaction.TransactionAmount * (double)convTransaction.ExchangeRate), 2), "Credit");
                            }

                            //If platform transactions are successful
                            if (isToSucessful && isFromSucessful)
                            {
                                //Do actual transfer of funds and set IsApproved true
                                if (clientAccBO.TransferUserFund(transaction.AccountNumber, transaction.ToAccountNumber, (double)convTransaction.TransactionAmount, (double)convTransaction.FeeAmount, (double)transaction.ExchangeRate, organizationID))
                                {
                                    //Log in transaction table
                                    var pkTransactionId = transactionBO.InternalFundTransfer(transaction.AccountNumber, transaction.ToAccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)convTransaction.TransactionAmount, (double)transaction.ExchangeRate, convTransaction.Notes, organizationID);

                                    //Logs fund transfers details(Withdrawal/Deposit) in TransferLogs table
                                    transferLogBO.AddTransferLogForTransaction(pkTransactionId, transaction.AccountNumber, transaction.ToAccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)convTransaction.TransactionAmount, (double)transaction.ExchangeRate, organizationID);

                                    //Log fee transfer if fee > 0
                                    if (convTransaction.FeeAmount != 0)
                                    {
                                        //Log fee deduction in transaction table
                                        var pkFeeTransactionId = transactionBO.InternalFeeTransaction(transaction.AccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)convTransaction.FeeAmount, organizationID);

                                        //Logs fee transfers details in TransferLogs table
                                        transferLogBO.AddTransferLogForFee(pkFeeTransactionId, transaction.AccountNumber, (int)transaction.FK_CurrencyID, (int)transaction.FK_CurrencyID, (double)convTransaction.FeeAmount, organizationID);
                                    }

                                    //Set status Approve
                                    if (adminTransactionBO.ApproveIncomingTransaction(convTransaction))
                                    {
                                        //Same client
                                        if (transaction.AccountNumber.Split('-')[2] ==
                                            transaction.ToAccountNumber.Split('-')[2])
                                        {
                                            //Log activity details
                                            InsertConversionActivityDetails((int) transaction.FK_UserID,
                                                                            (int) transaction.FK_CurrencyID,
                                                                            (int) transaction.FK_ToCurrencyID,
                                                                            (double) convTransaction.TransactionAmount,
                                                                            (double) transaction.ExchangeRate,
                                                                            transaction.AccountNumber,
                                                                            transaction.ToAccountNumber,
                                                                            Constants.K_STATUS_TRANSFERRED);
                                        }
                                            //Inter client
                                        else
                                        {
                                            //Log activity details
                                            InsertInterClientConversionActivityDetails((int) transaction.FK_UserID,
                                                                                       (int) transaction.FK_ToUserID,
                                                                                       (int) transaction.FK_CurrencyID,
                                                                                       (int) transaction.FK_ToCurrencyID,
                                                                                       (double)
                                                                                       convTransaction.TransactionAmount,
                                                                                       (double) transaction.ExchangeRate,
                                                                                       transaction.AccountNumber,
                                                                                       transaction.ToAccountNumber,
                                                                                       Constants.K_STATUS_TRANSFERRED);
                                        }

                                        return Json(new {status = true});
                                    }
                                    else
                                    {
                                        return Json(new {status = false, message = "Some error occurred!"});
                                    }
                                }
                                else
                                {
                                    return Json(new { status = false, message = "Some error occurred!" });
                                }
                            }
                            else
                            {
                                return Json(new { status = false, message = "Some error in platform!"});
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!"});
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
                if (SessionManagement.UserInfo != null)
                {
                    bool inverse = false;
                    decimal exchangeRate = priceBO.GetExchangeRateForCurrencyPair(fromCurr, toCurr, ref inverse);

                    var settings =
                        transactionSettingBO.GetTransactionSetting((int) AdminTransactionType.ConversionsRequests, (int)SessionManagement.OrganizationID);

                    //If settings not null
                    if (settings != null)
                    {
                        int markupType = (int) settings.ConversionMarkupType;
                        decimal markup = (decimal) settings.ConversionMarkupValue;

                        //Markup in percentage
                        if (markupType == (int) ConversionMarkupType.Percentage)
                        {
                            //Inverse calculation
                            if (inverse)
                            {
                                exchangeRate = Math.Round((1/(exchangeRate + (exchangeRate*markup/100))), 5);
                            }
                            else
                            {
                                exchangeRate = exchangeRate - (exchangeRate*markup/100);
                            }
                            return exchangeRate;
                        }
                            //Markup in points
                        else if (markupType == (int) ConversionMarkupType.Points)
                        {
                            //Inverse calculation
                            if (inverse)
                            {
                                //JPY currency logic
                                if (fromCurr == "JPY" || toCurr == "JPY")
                                {
                                    exchangeRate = Math.Round((1/(exchangeRate + (markup*JpyPointMultiplier))), 5);
                                }
                                else
                                {
                                    exchangeRate = Math.Round((1/(exchangeRate + (markup*PointMultiplier))), 5);
                                }
                            }
                            else
                            {
                                //JPY currency logic
                                if (fromCurr == "JPY" || toCurr == "JPY")
                                {
                                    exchangeRate = exchangeRate - (markup*JpyPointMultiplier);
                                }
                                else
                                {
                                    exchangeRate = exchangeRate - (markup*PointMultiplier);
                                }
                            }
                            return exchangeRate;
                        }
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
        /// This action creates new conversion transaction
        /// </summary>
        /// <param name="newTransfer">newTransfer</param>
        /// <returns></returns>
        public ActionResult CreateNewConversionTransaction(NewTransferTransaction newTransfer)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    //Validate admin password
                    if (LoginVerification.ValidateUser(loginInfo.UserEmail, newTransfer.AdminPassword))
                    {
                        //Get balance for accounts
                        decimal accBalance = clientAccBO.GetAccountBalance(newTransfer.FromClientAccount, organizationID);
                        decimal pendingTransactionAmount = adminTransactionBO.GetPendingTransferAmount(newTransfer.FromClientAccount, organizationID);

                        //Check balance
                        if (accBalance >= newTransfer.TransactionAmount)
                        {
                            //Check pending requests balance
                            if (accBalance >= (pendingTransactionAmount + newTransfer.TransactionAmount))
                            {
                                //Create new request
                                var newConvTransac = new AdminTransaction();
                                newConvTransac.TransactionDate = DateTime.UtcNow;
                                newConvTransac.FK_UserID = newTransfer.FromClientUserID;
                                newConvTransac.AccountNumber = newTransfer.FromClientAccount;
                                newConvTransac.FK_CurrencyID = lCurrValueBO.GetCurrencyIDFromSymbol(newTransfer.Currency);
                                newConvTransac.FK_ToCurrencyID = lCurrValueBO.GetCurrencyIDFromSymbol(newTransfer.ToCurrency);
                                newConvTransac.TransactionAmount = newTransfer.TransactionAmount;
                                newConvTransac.FK_AdminTransactionTypeID = (int)AdminTransactionType.ConversionsRequests;
                                newConvTransac.Notes = newTransfer.Notes;
                                newConvTransac.ClientName = newTransfer.FromClientName;
                                newConvTransac.FeeAmount = newTransfer.TransactionFee;
                                newConvTransac.ToAccountNumber = newTransfer.ToClientAccount;
                                newConvTransac.ToClientName = newTransfer.ToClientName;
                                newConvTransac.FK_ToUserID = newTransfer.ToClientUserID;
                                newConvTransac.ExchangeRate = newTransfer.ExchangeRate;
                                newConvTransac.FK_OrganizationID = organizationID;
                                newConvTransac.IsApproved = false;
                                newConvTransac.IsDeleted = false;

                                //Add new request
                                adminTransactionBO.AddNewAdminTransactionRequest(newConvTransac);

                                return Json(new { status = true });
                            }
                            else
                            {
                                return Json(new { status = false, message = "Insufficient balance due to pending transfer requests!" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, message = "Insufficient balance!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Invalid password!" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(new { status = false, message = "Some error occurred!" });
            }
        }

        /// <summary>
        /// This action logs conversion activity details in database
        /// </summary>
        /// <param name="clientUserId">clientUserId</param>
        /// <param name="fromCurrId">fromCurrId</param>
        /// <param name="toCurrId">toCurrId</param>
        /// <param name="amount">amount</param>
        /// <param name="exchangeRate">exchangeRate</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertConversionActivityDetails(int clientUserId, int fromCurrId, int toCurrId, double amount, double exchangeRate, string fromAcc, string toAcc, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Insert in UserActivity
                    int pkActivityId = userActivityBO.InsertUserActivityDetails(clientUserId, (int)ActivityType.ConversionActivity);

                    //Insert in TransferActivity
                    convActivityBO.InsertConversionActivityDetails(pkActivityId, fromCurrId, toCurrId, amount, exchangeRate, fromAcc, toAcc, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action logs inter client conversion activity details in database
        /// </summary>
        /// <param name="clientFromUserId">clientFromUserId</param>
        /// <param name="clientToUserId">clientToUserId</param>
        /// <param name="fromCurrId">fromCurrId</param>
        /// <param name="toCurrId">toCurrId</param>
        /// <param name="amount">amount</param>
        /// <param name="exchangeRate">exchangeRate</param>
        /// <param name="fromAcc">fromAcc</param>
        /// <param name="toAcc">toAcc</param>
        /// <param name="status">status</param>
        public void InsertInterClientConversionActivityDetails(int clientFromUserId, int clientToUserId, int fromCurrId, int toCurrId, double amount, double exchangeRate, string fromAcc, string toAcc, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Insert in UserActivity
                    int pkActivityFromId = userActivityBO.InsertUserActivityDetails(clientFromUserId, (int)ActivityType.ConversionActivity);
                    int pkActivityToId = userActivityBO.InsertUserActivityDetails(clientToUserId, (int)ActivityType.ConversionActivity);

                    //Insert in ConversionActivity
                    convActivityBO.InsertInterClientConversionActivityDetails(pkActivityFromId, null, clientToUserId, fromCurrId, toCurrId, amount, exchangeRate, fromAcc, toAcc, status);
                    convActivityBO.InsertInterClientConversionActivityDetails(pkActivityToId, clientFromUserId, null, fromCurrId, toCurrId, amount, exchangeRate, fromAcc, toAcc, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }
        #endregion

        #region Common Actions
        /// <summary>
        /// This action returns list of landing accounts of client
        /// </summary>
        /// <param name="pkClientUserID">pkClientUserID</param>
        /// <returns></returns>
        public JsonResult GetUserAllLandingAccounts(int pkClientUserID)
        {
            try
            {
                List<LandingAccountDetails> lstLandingAcc = new List<LandingAccountDetails>();
                List<Client_Account> allLandingAccs;

                //Live
                if (userBO.GetUserDetails(pkClientUserID).Clients != null)
                {
                    allLandingAccs = clientAccBO.GetAllLandingAccountForUser(LoginAccountType.LiveAccount, pkClientUserID);
                }
                //Partner
                else
                {
                    allLandingAccs = clientAccBO.GetAllLandingAccountForUser(LoginAccountType.PartnerAccount, pkClientUserID);
                }

                //Iterate through each account
                foreach (var acc in allLandingAccs)
                {
                    LandingAccountDetails landing = new LandingAccountDetails();
                    landing.LandingAccount = acc.LandingAccount;
                    landing.LandingCurrency = currencyBO.GetCurrencySymbolFromID((int)acc.FK_CurrencyID);
                    landing.LandingBalance = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");

                    lstLandingAcc.Add(landing);
                }

                //Return
                return Json(lstLandingAcc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns list of all accounts of client
        /// </summary>
        /// <param name="pkClientUserId">pkClientUserId</param>
        /// <returns></returns>
        public JsonResult GetAllUserAccountsWithBalance(int pkClientUserId)
        {
            var allAccounts = new List<TradingAccountGrouped>();
            List<Client_Account> tradingAccs;

            //Live
            if (userBO.GetUserDetails(pkClientUserId).Clients != null)
            {
                tradingAccs = clientAccBO.GetAllTradingAccountsOfUser(LoginAccountType.LiveAccount, pkClientUserId);
            }
            else
            {
                tradingAccs = clientAccBO.GetAllTradingAccountsOfUser(LoginAccountType.PartnerAccount, pkClientUserId);
            }

            var pairedTradingAcct = tradingAccs.GroupBy(o => o.FK_CurrencyID);

            //Loop through each currency grouped accounts
            foreach (var item in pairedTradingAcct)
            {
                var groupedTradingAccount = new TradingAccountGrouped();
                groupedTradingAccount.TradingCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)item.Key);
                var list = new List<LandingAccountDetails>();
                foreach (var groupedItem in item)
                {
                    LandingAccountDetails land = new LandingAccountDetails();
                    land.LandingAccount = (bool)groupedItem.IsLandingAccount
                                              ? groupedItem.LandingAccount
                                              : groupedItem.TradingAccount;
                    land.LandingBalance = groupedItem.CurrentBalance.ToString();
                    land.IsLanding = (bool)groupedItem.IsLandingAccount;
                    land.LandingCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)groupedItem.FK_CurrencyID);
                    list.Add(land);
                }
                groupedTradingAccount.TradingAccountList = list;
                allAccounts.Add(groupedTradingAccount);
            }

            return Json(allAccounts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action deleted a transaction
        /// </summary>
        /// <param name="transactionID">transactionID</param>
        /// <returns></returns>
        public ActionResult DeleteTransaction(int transactionID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(adminTransactionBO.DeleteTransaction(transactionID));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
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
                TradeTransInfoNET newTransac = new TradeTransInfoNET();
                newTransac.cmd = (short)TradeCommands.OP_BALANCE;
                newTransac.comment = comment;
                newTransac.orderby = login;
                newTransac.price = amount;
                newTransac.type = (short)TradeTransTypes.TT_BR_BALANCE;
                newTransac.reserved = 0;

                MetaTraderWrapperManager manager = new MetaTraderWrapperManager("mtdem01.primexm.com:443", 900, "!FQS123!!");
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
        /// This action logs deposit or withdraw activity details in database
        /// </summary>
        /// <param name="clientUserId">clientUserId</param>
        /// <param name="type">type</param>
        /// <param name="currId">currId</param>
        /// <param name="amount">amount</param>
        /// <param name="accNumber">accNumber</param>
        /// <param name="bankId">bankId</param>
        /// <param name="status">status</param>
        public void InsertDepositOrWithdrawActivityDetails(int clientUserId, string type, int currId, decimal amount, string accNumber, int? bankId, string status)
        {
            try
            {
                //Insert in UserActivity
                int pkActivityId = userActivityBO.InsertUserActivityDetails(clientUserId,
                                                                            (int)ActivityType.DepositOrWithdrawActivity);

                //Insert in DepositOrWithdrawActivity
                depWithDrwActBO.InsertDepositOrWithdrawActivityDetails(pkActivityId, type, currId, amount, accNumber, bankId, status);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
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
                    accountMargin += ((accountMargin * (decimal)marginRestriction) / 100);

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
        #endregion

    }
}
