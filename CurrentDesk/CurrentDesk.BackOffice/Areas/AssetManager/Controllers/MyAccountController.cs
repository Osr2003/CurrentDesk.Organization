#region Header Information
/***************************************************************************
 * File Name     : MyAccountController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 26th April 2013
 * Modified Date : 26th April 2013
 * Description   : This file MyAccount controller and related actions to
 *                 handle AssetManager accounts functionality
 * *************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients;
using CurrentDesk.BackOffice.Models.MyAccount;
using CurrentDesk.BackOffice.Models.Transfers;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using CurrentDesk.Models;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Utilities;
#endregion

namespace CurrentDesk.BackOffice.Areas.AssetManager.Controllers
{
    /// <summary>
    /// This class represents MyAccount controller and contains
    /// actions to handle AM accounts functionality
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_AM), NoCache]
    public class MyAccountController : Controller
    {
        #region Variables
        private Client_AccountBO clientAccBo = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private AccountCurrencyBO accountCurrencyBO = new AccountCurrencyBO();
        private TransferLogBO transferLogBO = new TransferLogBO();
        private TransactionBO transactionBO = new TransactionBO();
        private L_BrokerExchangeRateBO lBrokerExchangeRateBO = new L_BrokerExchangeRateBO();
        private L_CountryBO countryBO = new L_CountryBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private BankAccountInformationBO bankBO = new BankAccountInformationBO();
        #endregion

        /// <summary>
        /// This action returns MyAccounts Index view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string[] currencyIds = clientAccBo.GetDifferentCurrencyAccountOfUser(loginInfo.LogAccountType, loginInfo.UserID).TrimEnd('/').Split('/');

                    var currModel = new ClientAccountsModel();
                    currModel.CurrencyAccountDetails = new List<MyAccountCurrencyModel>();

                    foreach (var curr in currencyIds)
                    {
                        var model = new MyAccountCurrencyModel();
                        var landingAccDetails = clientAccBo.GetLandingAccountForCurrencyOfUser(loginInfo.LogAccountType, loginInfo.UserID, Convert.ToInt32(curr));
                        model.CurrencyID = curr;
                        model.CurrencyName = lCurrValueBO.GetCurrencySymbolFromID(Convert.ToInt32(curr));
                        model.CurrencyImage = lCurrValueBO.GetCurrencyImageClass(Convert.ToInt32(curr));
                        model.LandingAccount = landingAccDetails.LandingAccount;
                        model.LAccBalance = Utility.FormatCurrencyValue((decimal)landingAccDetails.CurrentBalance, "");
                        currModel.CurrencyAccountDetails.Add(model);
                    }

                    return View(currModel);
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
        /// This action returns all trading account details for
        /// a particular currency
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <returns></returns>
        public ActionResult GetAccountInformtion(string currencyID)
        {
            try
            {
                LoginInformation loginInfo = SessionManagement.UserInfo;
                var tradingAccs = clientAccBo.GetAllTradingAccountsForCurrency(loginInfo.LogAccountType, loginInfo.UserID, Convert.ToInt32(currencyID));

                var tradingAccList = new List<CurrencyAccountModel>();

                foreach (var acc in tradingAccs)
                {
                    var accModel = new CurrencyAccountModel();
                    
                    //If account name is available
                    if (acc.AccountName != null)
                    {
                        //Master account
                        if ((bool)acc.IsTradingAccount)
                        {
                            accModel.Account = acc.TradingAccount + "<br/>" + acc.AccountName;
                            accModel.Type = "<img src='/Images/account-metatrader.png' title='Master Account' alt='Master Account'>";
                            accModel.Balance = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                            accModel.Equity = acc.Equity != null ? Utility.FormatCurrencyValue((decimal)acc.Equity, "") : "NA";
                            accModel.Floating = "$0.00";
                            accModel.Change = "-0.23%";
                        }
                        //Rebate account
                        else
                        {
                            accModel.Account = acc.TradingAccount + "<br/>" + acc.AccountName;
                            accModel.Type = "<img src='/Images/account-rebate.png' title='Fee/Rebate Account' alt='Fee/Rebate Account'>";
                            accModel.Equity = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                            accModel.Balance = "--";
                            accModel.Floating = "--";
                            accModel.Change = "--";
                        }
                    }
                    //No account name
                    else
                    {
                        //Master account
                        if ((bool)acc.IsTradingAccount)
                        {
                            accModel.Account = acc.TradingAccount + "<br/>Master Account";
                            accModel.Type = "<img src='/Images/account-metatrader.png' title='Master Account' alt='Master Account'>";
                            accModel.Balance = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                            accModel.Equity = acc.Equity != null ? Utility.FormatCurrencyValue((decimal)acc.Equity, "") : "NA";
                            accModel.Floating = "$0.00";
                            accModel.Change = "-0.23%";
                        }
                        //Rebate account
                        else
                        {
                            accModel.Account = acc.TradingAccount + "<br/>Fee/Rebate Account";
                            accModel.Type = "<img src='/Images/account-rebate.png' title='Fee/Rebate Account' alt='Fee/Rebate Account'>";
                            accModel.Equity = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                            accModel.Balance = "--";
                            accModel.Floating = "--";
                            accModel.Change = "--";
                        }
                    }

                    accModel.IsTradingAccount = acc.IsTradingAccount;

                    tradingAccList.Add(accModel);
                }

                return Json(new
                {
                    total = 1,
                    page = 1,
                    records = tradingAccList.Count,
                    rows = tradingAccList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns CreateNewLandingAcc view
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateNewLandingAcc()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_PARTNER, (int)SessionManagement.OrganizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");

                    return View("CreateNewLandingAcc");
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
        /// This action creates new landing account for user as per currency id
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <returns></returns>
        public ActionResult CreateNewLandingAccount(string currencyID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return (Json(clientAccBo.CreateNewLandingAccount(loginInfo.LogAccountType, loginInfo.UserID, accountCurrencyBO.GetCurrencyLookUpID(Convert.ToInt32(currencyID)))));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns RebateAccountDetails view with
        /// required data passed as model
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult RebateAccountDetails(string accountNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    var model = new AccountDetailsModel();
                    model.TransferLogDetails = new List<TransferLogDetails>();

                    var accountDetails = clientAccBo.GetAccountDetails(accountNumber, organizationID);
                    var latestTransactions = transferLogBO.GetLatestTransactionsForAccount(accountNumber, organizationID);

                    //Iterate through all transactions
                    foreach (var tran in latestTransactions)
                    {
                        var log = new TransferLogDetails();
                        log.TransactionDate = Convert.ToDateTime(tran.TransactionDateTime).ToString("dd/MM/yyyy HH:mm:ss tt");
                        log.TransactionType = tran.TransactionType;
                        log.TransactionAmount = Utility.FormatCurrencyValue((decimal)tran.Amount, "");
                        model.TransferLogDetails.Add(log);
                    }

                    model.AccountNumber = accountNumber;

                    model.Balance = Utility.FormatCurrencyValue((decimal)accountDetails.CurrentBalance, "");

                    model.AccountName = accountDetails.AccountName;
                    
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
        /// This action adds or edit account name in Client_Account table
        /// </summary>
        /// <param name="accountName">accountName</param>
        /// <param name="accNumber">accNumber</param>
        /// <returns></returns>
        public ActionResult SaveAccountName(string accountName, string accNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(clientAccBo.SaveAccountName(accountName, accNumber, (int)SessionManagement.OrganizationID));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns WithdrawFunds view with required
        /// data passed as model
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult WithdrawFunds(string accountNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");
                    
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var model = new TransfersModel();
                    model.BankInformation = new List<BankInformation>();
                    model.LandingAccInformation = new List<LandingAccInformation>();
                    model.AccountNumber = accountNumber;

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
                    var landingAccs = clientAccBo.GetAllLandingAccountForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[0]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;

                        lAccInfo.LAccBalance = Utility.FormatCurrencyValue((decimal)lAcc.CurrentBalance, "");

                        model.LandingAccInformation.Add(lAccInfo);
                    }

                    return View("WithdrawFunds", model);
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
        /// This action returns TransferFunds view with required
        /// data passed as model
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult TransferFunds(string accountNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var model = new TransfersModel();
                    model.TradingAccInformation = new List<TradingAccountGrouped>();
                    model.AccountNumber = accountNumber;

                    //Get all trading accounts
                    var tradingAccs = clientAccBo.GetAllTradingAccountsOfUser(loginInfo.LogAccountType, loginInfo.UserID);
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

                    return View("TransferFunds", model);
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
        /// This action returns MasterAccountDetails view with
        /// required data passed as model
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult MasterAccountDetails(string accountNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    var model = new AccountDetailsModel();
                    model.TransferLogDetails = new List<TransferLogDetails>();
                    var accountDetails = clientAccBo.GetAccountDetails(accountNumber, organizationID);
                    var latestTransactions = transferLogBO.GetLatestTransactionsForAccount(accountNumber, organizationID);

                    foreach (var tran in latestTransactions)
                    {
                        var log = new TransferLogDetails();
                        log.TransactionDate = Convert.ToDateTime(tran.TransactionDateTime).ToString("dd/MM/yyyy HH:mm:ss tt");
                        log.TransactionType = tran.TransactionType;
                        log.TransactionAmount = Utility.FormatCurrencyValue((decimal)tran.Amount, "");
                        model.TransferLogDetails.Add(log);
                    }

                    model.AccountNumber = accountNumber;

                    model.Balance = Utility.FormatCurrencyValue((decimal)accountDetails.CurrentBalance, "");
                    model.Equity = accountDetails.Equity != null ? Utility.FormatCurrencyValue((decimal)accountDetails.Equity, "") : "NA";

                    model.AccountName = accountDetails.AccountName;
                    model.IsTradingAcc = accountDetails.IsTradingAccount;
                    model.PlatformLogin = accountDetails.PlatformLogin.ToString();
                    model.PlatformPassword = accountDetails.PlatformPassword;
                    
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
        /// This action returns SlaveAccounts view
        /// </summary>
        /// <returns></returns>
        public ActionResult SlaveAccounts(string accountNumber)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["AccountNumber"] = accountNumber;
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

    }
}
