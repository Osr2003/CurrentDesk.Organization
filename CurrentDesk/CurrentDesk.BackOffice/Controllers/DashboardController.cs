#region Header Information
/***************************************************************
 * File Name     : DashboardController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 15th March 2013
 * Modified Date : 15th March 2013
 * Description   : This file contains controller and related actions 
 *                 for Dashboard view display of data
 * *************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models.Dashboard;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.Models;
using System.Xml;
using CurrentDesk.Common;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Utilities;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This controller contains actions related to Dashboard view
    /// </summary>
    [AuthorizeTrader, NoCache]
    public class DashboardController : Controller
    {
        #region Variables
        private Client_AccountBO clientAccBO = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private ClientBO clientBO = new ClientBO();
        private UserImageBO userImgBO = new UserImageBO();
        private UserActivityBO usrActivityBO = new UserActivityBO();
        private UserBO userBO = new UserBO();
        private IntroducingBrokerBO introducingBrokerBO = new IntroducingBrokerBO();
        #endregion

        /// <summary>
        /// This action returns Dashboard view with view-model
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    DashboardModel model = new DashboardModel();
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var userAccInfo = clientAccBO.GetDashboardAccounts(loginInfo.LogAccountType, loginInfo.UserID);

                    //Group all accounts by currency
                    var pairedAccInfo = userAccInfo.GroupBy(o => o.FK_CurrencyID);
                    var tradeList = new List<UserAccountGrouped>();

                    //Iterate through each currency grouped accounts and add them to model
                    foreach (var item in pairedAccInfo)
                    {
                        var groupedTradingAccount = new UserAccountGrouped();
                        groupedTradingAccount.AccountCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)item.Key);
                        var list = new List<Client_Account>();
                        foreach (var groupedItem in item)
                        {
                            list.Add(groupedItem);
                        }
                        groupedTradingAccount.UserAccountList = list;
                        tradeList.Add(groupedTradingAccount);
                    }
                    model.UserAccInformation = tradeList;

                    //Get market news
                    model.MarketNews = GetMarketNews();

                    //Get IB userID under which client is present
                    int IbUserID = clientBO.GetIntroducingBrokerIDOfClient(loginInfo.UserID);
                    model.BrokerPromoImgName = String.Empty;
                    
                    //If client is under any IB
                    if (IbUserID != 0)
                    {
                        var imgDetail = userImgBO.GetActiveImageOfIB(IbUserID);
                        if (imgDetail != null)
                        {
                            var imgExt = imgDetail.ImageName.Substring(imgDetail.ImageName.LastIndexOf('.'));
                            model.BrokerPromoImgName = imgDetail.PK_UserImageID + imgExt;
                        }
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
        /// This action returns list of market news from
        /// forexfactory xml file
        /// </summary>
        /// <returns></returns>
        public List<MarketNewsDataModel> GetMarketNews()
        {
            List<MarketNewsDataModel> lstMarketNews = new List<MarketNewsDataModel>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("http://www.forexfactory.com/ffcal_week_this.xml");
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("event");

                //Iterate through each news event
                //and if news date >= todays date then add to list
                foreach (XmlNode node in nodes)
                {
                    if (node["date"].InnerText.DateTimeTryParse() >= DateTime.Now.Date)
                    {
                        MarketNewsDataModel marketNewsData = new MarketNewsDataModel();
                        marketNewsData.NewsDateTime = node["date"].InnerText + " " + node["time"].InnerText;
                        marketNewsData.Currency = node["country"].InnerText;
                        marketNewsData.Title = node["title"].InnerText;
                        marketNewsData.Impact = node["impact"].InnerText;
                        lstMarketNews.Add(marketNewsData);
                    }
                }

                //Return list of market news
                return lstMarketNews;
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return lstMarketNews;
            }
        }

        /// <summary>
        /// This action returns list of user activities
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientRecentActivityDetails()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    List<UserActivityModel> lstUserActivities = new List<UserActivityModel>();

                    //Get latest activities
                    var activities = usrActivityBO.GetUserRecentActivityDetails(loginInfo.UserID);

                    //Set IsSeen true for new activities if any
                    usrActivityBO.MarkSeenRecentActivity(loginInfo.UserID);

                    foreach (var act in activities)
                    {
                        UserActivityModel usrAct = new UserActivityModel();
                        usrAct.ActivityTimestamp = Convert.ToDateTime(act.Timestamp).ToString("dd/MM/yyyy HH:mm:ss tt");
                        usrAct.IsSeen = (bool) act.IsSeen;

                        //Profile Activities
                        if (act.FK_ActivityTypeID == (int) ActivityType.ProfileActivity)
                        {
                            usrAct.ActivityDetails = act.ProfileActivities.FirstOrDefault().ProfileActivityDetails;
                        }
                            //Document activities
                        else if (act.FK_ActivityTypeID == (int) ActivityType.DocumentActivity)
                        {
                            usrAct.ActivityDetails = "<a href=Document>" +
                                                     act.DocumentActivities.FirstOrDefault().Document.DocumentName +
                                                     "</a>" + " document status has changed to <i>" +
                                                     act.DocumentActivities.FirstOrDefault().DocumentStatus + "</i>.";
                        }
                            //Account activities
                        else if (act.FK_ActivityTypeID == (int) ActivityType.AccountActivity)
                        {
                            //New acc creation activity
                            if (act.AccountActivities.FirstOrDefault().FK_AccActivityTypeID ==
                                (int) AccountActivityType.NewAccountCreation)
                            {
                                //If account number is present
                                if (act.AccountActivities.FirstOrDefault().FK_ClientAccountID != null)
                                {
                                    //If landing acc
                                    if (act.AccountActivities.FirstOrDefault().AccountType == "Landing")
                                    {
                                        usrAct.ActivityDetails = "A new " +
                                                                 act.AccountActivities.FirstOrDefault()
                                                                    .L_CurrencyValue.CurrencyValue + " " +
                                                                 act.AccountActivities.FirstOrDefault().AccountType +
                                                                 " <a href='MyAccount'>" +
                                                                 act.AccountActivities.FirstOrDefault()
                                                                    .Client_Account.LandingAccount +
                                                                 "</a> account has been created.";
                                    }
                                    else
                                    {
                                        usrAct.ActivityDetails = "A new " +
                                                                 act.AccountActivities.FirstOrDefault()
                                                                    .L_CurrencyValue.CurrencyValue + " " +
                                                                 act.AccountActivities.FirstOrDefault().AccountType +
                                                                 " <a href='MyAccount'>" +
                                                                 act.AccountActivities.FirstOrDefault()
                                                                    .Client_Account.TradingAccount +
                                                                 "</a> account has been created.";
                                    }
                                }
                                else
                                {
                                    usrAct.ActivityDetails = "A new " +
                                                             act.AccountActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue + " " +
                                                             act.AccountActivities.FirstOrDefault().AccountType +
                                                             " account has been created.";
                                }
                            }
                        }
                            //Transfer activities
                        else if (act.FK_ActivityTypeID == (int) ActivityType.TransferActivity)
                        {
                            if (act.TransferActivities.FirstOrDefault().TransferStatus == Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.TransferActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.TransferActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been transferred from account <a href='MyAccount'>" +
                                                             act.TransferActivities.FirstOrDefault().FromAccount +
                                                             "</a> to <a href='MyAccount'>" +
                                                             act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                                }
                                else if (act.TransferActivities.FirstOrDefault().FK_FromUserID != null)
                                {
                                    string fromClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int) act.TransferActivities.FirstOrDefault().FK_FromUserID) !=
                                        null)
                                    {
                                        fromClientName =
                                            clientBO.GetClientName(
                                                (int) act.TransferActivities.FirstOrDefault().FK_FromUserID);
                                    }
                                    else
                                    {
                                        fromClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int) act.TransferActivities.FirstOrDefault().FK_FromUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been deposited from " + fromClientName +
                                                             " to <a href='MyAccount'>" +
                                                             act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                                }
                                else if (act.TransferActivities.FirstOrDefault().FK_ToUserID != null)
                                {
                                    string toClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails((int) act.TransferActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName =
                                            clientBO.GetClientName(
                                                (int) act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int) act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been transferred from account <a href='MyAccount'>" +
                                                             act.TransferActivities.FirstOrDefault().FromAccount +
                                                             "</a> to " + toClientName + ".";
                                }
                            }
                            else
                            {
                                usrAct.ActivityDetails = "<b>" +
                                                         Utility.FormatCurrencyValue(
                                                             (decimal) act.TransferActivities.FirstOrDefault()
                                                                          .TransferAmount, "") + " " +
                                                         act.TransferActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue +
                                                         "</b> is pending transfer from account <a href='MyAccount'>" +
                                                         act.TransferActivities.FirstOrDefault().FromAccount +
                                                         "</a> to <a href='MyAccount'>" +
                                                         act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                            }
                        }
                            //Conversion activities
                        else if (act.FK_ActivityTypeID == (int) ActivityType.ConversionActivity)
                        {
                            if (act.ConversionActivities.FirstOrDefault().ConversionStatus ==
                                Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.ConversionActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.ConversionActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (int) act.ConversionActivities.FirstOrDefault()
                                                                          .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b>  has been converted from <a href='MyAccount'>" +
                                                             act.ConversionActivities.FirstOrDefault().FromAccount +
                                                             "</a> at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount)*
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue1.CurrencyValue +
                                                             "</b> and transferred to <a href='MyAccount'>" +
                                                             act.ConversionActivities.FirstOrDefault().ToAccount +
                                                             "</a>.";
                                }
                                else if (act.ConversionActivities.FirstOrDefault().FK_FromUserID != null)
                                {
                                    string fromClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int) act.ConversionActivities.FirstOrDefault().FK_FromUserID) !=
                                        null)
                                    {
                                        fromClientName =
                                            clientBO.GetClientName(
                                                (int) act.ConversionActivities.FirstOrDefault().FK_FromUserID);
                                    }
                                    else
                                    {
                                        fromClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int) act.ConversionActivities.FirstOrDefault().FK_FromUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (int) act.ConversionActivities.FirstOrDefault()
                                                                          .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b>  has been converted from " +
                                                             fromClientName +
                                                             " at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount)*
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue1.CurrencyValue +
                                                             "</b> and transferred to <a href='MyAccount'>" +
                                                             act.ConversionActivities.FirstOrDefault().ToAccount +
                                                             "</a>.";
                                }
                                else if (act.ConversionActivities.FirstOrDefault().FK_ToUserID != null)
                                {
                                    string toClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int) act.ConversionActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName =
                                            clientBO.GetClientName(
                                                (int) act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int) act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.ConversionActivities.FirstOrDefault()
                                                                              .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b>  has been converted from <a href='MyAccount'>" +
                                                             act.ConversionActivities.FirstOrDefault().FromAccount +
                                                             "</a> at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount)*
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue1.CurrencyValue +
                                                             "</b> and transferred to " +
                                                             toClientName +
                                                             ".";
                                }
                            }
                            else
                            {
                                usrAct.ActivityDetails = "<b>" +
                                                         Utility.FormatCurrencyValue(
                                                             (int) act.ConversionActivities.FirstOrDefault()
                                                                      .ConversionAmount, "") + " " +
                                                         act.ConversionActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue +
                                                         "</b>  has been converted from <a href='MyAccount'>" +
                                                         act.ConversionActivities.FirstOrDefault().FromAccount +
                                                         "</a> at an exchange rate of " +
                                                         act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                         " totaling <b>" +
                                                         Math.Round(
                                                             Convert.ToDecimal(
                                                                 act.ConversionActivities.FirstOrDefault()
                                                                    .ConversionAmount)*
                                                             Convert.ToDecimal(
                                                                 act.ConversionActivities.FirstOrDefault().ExchangeRate),
                                                             2) +
                                                         " " +
                                                         act.ConversionActivities.FirstOrDefault()
                                                            .L_CurrencyValue1.CurrencyValue +
                                                         "</b> and is pending transfer to <a href='MyAccount'>" +
                                                         act.ConversionActivities.FirstOrDefault().ToAccount + "</a>.";
                            }
                        }
                            //Fund or withdraw activities
                        else if (act.FK_ActivityTypeID == (int) ActivityType.DepositOrWithdrawActivity)
                        {
                            if (act.DepositOrWithdrawActivities.FirstOrDefault().TransferStatus ==
                                Constants.K_STATUS_PENDING)
                            {
                                if (act.DepositOrWithdrawActivities.FirstOrDefault().Type == Constants.K_DEPOSIT)
                                {
                                    usrAct.ActivityDetails = "You have submitted a deposit of <b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") + " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue + "</b>.";
                                }
                                else
                                {
                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") +
                                                             " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> have been withdrawn from <a href='MyAccount'>" +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .AccountNumber +
                                                             "</a> and is pending transfer to " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .BankAccountInformation.BankName + ".";
                                }
                            }
                            else
                            {
                                if (act.DepositOrWithdrawActivities.FirstOrDefault().Type == Constants.K_DEPOSIT)
                                {
                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") + " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been received and deposited into <a href='MyAccount'>" +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .AccountNumber + "</a>.";
                                }
                                else
                                {
                                    usrAct.ActivityDetails = "<b>" +
                                                             Utility.FormatCurrencyValue(
                                                                 (decimal) act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") +
                                                             " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> have been withdrawn from <a href='MyAccount'>" +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .AccountNumber + "</a> and transferred to " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .BankAccountInformation.BankName + ".";
                                }
                            }
                        }

                        lstUserActivities.Add(usrAct);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstUserActivities.Count,
                            rows = lstUserActivities
                        }, JsonRequestBehavior.AllowGet);
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

    }
}
