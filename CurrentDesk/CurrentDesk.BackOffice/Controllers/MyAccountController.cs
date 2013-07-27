#region Header Information
/****************************************************************
 * File Name     : MyAccountController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 7th Feb 2013
 * Modified Date : 7th Feb 2013
 * Description   : This file contains MyAccounts page functionality
 * **************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Models.MyAccount;
using CurrentDesk.BackOffice.Models.Transfers;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.BackOffice.Utilities;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository;
using CurrentDesk.Repository.CurrentDesk;
using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This controller contains actions for handling MyAccounts view data display functionality
    /// </summary>
    [AuthorizeTrader, NoCache]
    public class MyAccountController : Controller
    {
        #region Variables
        private Client_AccountBO clientAccBo = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private AccountCurrencyBO accountCurrencyBO = new AccountCurrencyBO();
        private L_AccountCodeBO accountCodeBO = new L_AccountCodeBO();
        private TradingPlatformBO tradingPlatformBO = new TradingPlatformBO();
        private TransferLogBO transferLogBO = new TransferLogBO();
        private L_CountryBO countryBO = new L_CountryBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private BankAccountInformationBO bankBO = new BankAccountInformationBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private AccountActivityBO accActivityBO = new AccountActivityBO();
        private FundingSourceBO fundSourceBO = new FundingSourceBO();
        #endregion

        /// <summary>
        /// This action returns view model for My Accounts view 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //LoginInformation loginInfo = (LoginInformation)System.Web.HttpContext.Current.Session["UserInfo"];
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int)SessionManagement.OrganizationID;
                    string[] currencyIds = clientAccBo.GetDifferentCurrencyAccountOfUser(loginInfo.LogAccountType, loginInfo.UserID).TrimEnd('/').Split('/');

                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_LIVE, organizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");
                    ViewData["AccountCode"] = new SelectList(accountCodeBO.GetSelectedAccount(Constants.K_BROKER_LIVE), "PK_AccountID", "AccountName");
                    ViewData["TradingPlatform"] = new SelectList(tradingPlatformBO.GetSelectedPlatform(Constants.K_BROKER_LIVE, organizationID), "PK_TradingPlatformID", "L_TradingPlatformValues.TradingValue");

                    var currModel = new MyAccountModel();
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
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        public string GetAllCurrencyAccountsForUser()
        {
            try
            {
                LoginInformation loginInfo = SessionManagement.UserInfo;
                return clientAccBo.GetDifferentCurrencyAccountOfUser(loginInfo.LogAccountType, loginInfo.UserID);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Equity from cache
        /// </summary>
        /// <param name="loginid"></param>
        /// <returns></returns>
        public JsonResult GetEquity(int loginid)
        {

            try
            {
                var marginCache = new DataCache("MarginCache");
                object objMargin = marginCache.Get(loginid.StringTryParse());
                if (objMargin != null)
                {
                    var margin = (Margin)objMargin;

                    var marginDetails = new MarginDetails();

                    marginDetails.Equ = margin.Equity.CurrencyFormat();
                    marginDetails.Bal = margin.Balance.CurrencyFormat();

                    var equity = margin.Equity ?? 0;
                    var bal = margin.Balance ?? 0;
                    var pnl = equity - bal;

                    marginDetails.Pnl = pnl.CurrencyFormat();

                    return Json(marginDetails, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get AllTrades from TradesCache and cacculate profit
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public double GetPnl(int loginid)
        {
            double profits = 0;

            try
            {
                var tradesCache = new DataCache("TradesCache");

                var lstLogin = new List<string>();
                lstLogin.Add(loginid.StringTryParse());
                IEnumerable<KeyValuePair<string, object>> lstTrades = tradesCache.BulkGet(lstLogin);
                profits = lstTrades.Select(s => (TradesInfo)s.Value).ToList().Sum(s => s.Profit);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }

            return profits;
        }

        /// <summary>
        /// Get Equity,Balance and Pnl from Cache
        /// </summary>
        /// <param name="lstLogin"></param>
        /// <returns></returns>
        public JsonResult GetEquityListwww(List<int> lstLogin)
        {



            decimal marginVal = 0;

            try
            {




                var marginCache = new DataCache("MarginCache");

                var lstStrLogin = lstLogin.Select(s => s.StringTryParse()).ToList();

                IEnumerable<KeyValuePair<string, object>> lstMarginsObj = marginCache.BulkGet(lstStrLogin);
                var lstMargins = lstMarginsObj.Select(s => (Margin)s.Value).ToList();


                var tradesCache = new DataCache("TradesCache");
                IEnumerable<KeyValuePair<string, object>> lstTrades = tradesCache.BulkGet(lstStrLogin);
                var lstTradesInfo = lstTrades.Select(s => (TradesInfo)s.Value).ToList();
                var lstPnl = (from lt in lstTradesInfo
                              group lt by lt.Login into g
                              select new
                              {

                                  Login = g.Key,
                                  Profit = g.Sum(k => k.Profit)

                              }).ToList();


                var lstData = (from lm in lstMargins
                               join lp in lstPnl on lm.Login equals lp.Login
                               select new
                               {

                                   Login = lm.Login,
                                   Balance = lm.Balance,
                                   Pnl = lp.Profit

                               }).ToList();

                return Json(lstData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }


            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Equity,Balance and Pnl from Cache
        /// </summary>
        /// <param name="lstLogin"></param>
        /// <returns></returns>
        public JsonResult GetEquityList(string strLogin)
        {

            var lstMarginDetails = new List<MarginDetails>();
            var lstLogin = new List<int>();

            try
            {

                //Convert platform login to List<int>
                string[] lstStrLogins = strLogin.Split(new[] { ',' });
                foreach (var pl in lstStrLogins)
                {
                    lstLogin.Add(pl.Int32TryParse());
                }

                //Get Data from cache
                var marginCache = new DataCache("MarginCache");
                List<string> lstStrLogin = lstLogin.Select(s => s.StringTryParse()).ToList();
                IEnumerable<KeyValuePair<string, object>> lstMarginsObj = marginCache.BulkGet(lstStrLogin);

                //Create a Json Data
                var lstMargins = lstMarginsObj.Select(s => (Margin)s.Value).ToList();
                foreach (var margin in lstMargins)
                {
                    var ed = new MarginDetails();
                    if (margin != null)
                    {
                        ed.Pl = margin.Login ?? 0;
                        ed.Equ = margin.Equity.CurrencyFormat();
                        ed.Bal = margin.Balance.CurrencyFormat();

                        var equity = margin.Equity ?? 0;
                        var bal = margin.Balance ?? 0;

                        var pnl = equity - bal;
                        ed.Pnl = pnl.CurrencyFormat();
                        lstMarginDetails.Add(ed);
                    }
                }

                return Json(lstMarginDetails, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action returns all trading account details for
        /// a particular currency
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <returns></returns>
        public ActionResult GetAccountInformtion(int currencyID)
        {
            try
            {
                LoginInformation loginInfo = SessionManagement.UserInfo;
                var tradingAccs = clientAccBo.GetAllTradingAccountsForCurrency(loginInfo.LogAccountType, loginInfo.UserID, currencyID);

                var tradingAccList = new List<CurrencyAccountModel>();

                foreach (var acc in tradingAccs)
                {
                    var accModel = new CurrencyAccountModel();
                    if (acc.IsTradingAccount == true)
                    {
                        if (acc.AccountName != null)
                        {
                            accModel.Account = acc.TradingAccount + "<br/>" + acc.AccountName;
                        }
                        else
                        {
                            accModel.Account = acc.TradingAccount + "<br/>Trading Account";
                        }
                        if (tradingPlatformBO.GetTradingPlatformLookUpID((int)acc.FK_PlatformID) == Constants.K_META_TRADER_ID)
                        {
                            accModel.Type = "<img src='../Images/account-metatrader.png' title='MetaTrader 4' alt='MetaTrader 4'>";
                        }
                    }
                    else
                    {
                        if (acc.AccountName != null)
                        {
                            accModel.Account = acc.TradingAccount + "<br/>" + acc.AccountName;
                        }
                        else
                        {
                            accModel.Account = acc.TradingAccount + "<br/>Managed Account";
                        }
                        accModel.Type = "<img src='../Images/account-managed.png' title='Managed Account' alt='Managed Account'>";
                    }

                    accModel.Balance = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");

                    accModel.Floating = "10,000.00";
                    accModel.Equity = acc.Equity != null ? Utility.FormatCurrencyValue((decimal)acc.Equity, "") : "NA";
                    accModel.Change = "1.42";
                    accModel.IsTradingAccount = acc.IsTradingAccount;
                    accModel.PlatFormLogin = acc.PlatformLogin ?? 0;

                    tradingAccList.Add(accModel);
                }

                return Json(new
                {
                    total = 1,
                    page = 1,
                    records = tradingAccList.Count(),
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
                    var currLookupValue = accountCurrencyBO.GetCurrencyLookUpID(Convert.ToInt32(currencyID));

                    var accCreationResult = clientAccBo.CreateNewTraderLandingAccount(loginInfo.UserID, currLookupValue, (int)SessionManagement.OrganizationID);

                    //If landing account creation successful
                    if (accCreationResult != 0)
                    {
                        //Logs new account creation in db
                        InsertAccountActivityDetails(currLookupValue, "Landing", accCreationResult);

                        return Json(true);
                    }
                    return Json(false);
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
        /// This action creates new trading account in database
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <param name="accountTypeID">accountTypeID</param>
        /// <param name="platformID">platformID</param>
        /// <returns></returns>
        public ActionResult CreateNewTradingAccount(int currencyID, int accountTypeID, int platformID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;
                    int currLookupValue = accountCurrencyBO.GetCurrencyLookUpID(Convert.ToInt32(currencyID));

                    if (accountTypeID == Constants.K_TRADING_ACCOUNT)
                    {
                        int pkClientAccID = clientAccBo.CreateNewTradingAccount(loginInfo.LogAccountType, loginInfo.UserID, currLookupValue, organizationID);

                        //Create platform trading account
                        var user = new User();
                        user.UserEmailID = loginInfo.UserEmail;
                        AccountCreationController.CreateMetaTraderAccountForUser(pkClientAccID, platformID, user, loginInfo.LogAccountType);

                        //Logs new account creation in db
                        InsertAccountActivityDetails(currLookupValue, "Trading", pkClientAccID);

                        return Json(true);
                    }
                    else if (accountTypeID == Constants.K_MANAGED_ACCOUNT)
                    {
                        int pkClientAccID = clientAccBo.CreateNewManagedAccount(loginInfo.LogAccountType, loginInfo.UserID, currLookupValue, organizationID);

                        //Logs new account creation in db
                        InsertAccountActivityDetails(currLookupValue, "Managed", pkClientAccID);

                        return Json(true);
                    }
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns AcocuntDetails view
        /// with respective model
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult ShowAccountDetails(string accountNumber)
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

                    return View("AccountDetails", model);
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
        /// This actions returns FundAccount view in My Accounts section
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public ActionResult FundAccount(string accountNumber)
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
                    model.AccountNumber = accountNumber;

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
                    var landingAccs = clientAccBo.GetAllLandingAccountForUser(loginInfo.LogAccountType, loginInfo.UserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;

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
        /// This actions returns InternalTransfer view in My Accounts section
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public ActionResult InternalTransfer(string accountNumber)
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
        /// This actions returns WithdrawFund view in My Accounts section
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public ActionResult WithdrawFund(string accountNumber)
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
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;

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
        /// This action logs account activity details in database
        /// </summary>
        /// <param name="currID">currID</param>
        /// <param name="newAccType">newAccType</param>
        public void InsertAccountActivityDetails(int currID, string newAccType, int? pkClientAccID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Insert in UserActivity
                    int pkActivityID = userActivityBO.InsertUserActivityDetails(loginInfo.UserID, (int)ActivityType.AccountActivity);

                    //Insert account activity details
                    accActivityBO.InsertAccountActivityDetails(pkActivityID, currID, newAccType, pkClientAccID);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

    }
}
