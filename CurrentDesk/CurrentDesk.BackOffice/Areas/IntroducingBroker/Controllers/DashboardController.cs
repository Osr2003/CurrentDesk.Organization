#region Header Information
/*****************************************************************************
 * File Name     : DashboardController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 8th April 2013
 * Modified Date : 8th April 2013
 * Description   : This file contains DashboardController and related actions
 *                 to handle all functionality of IB Dashboard page
 * ***************************************************************************/
#endregion

#region Namespace Used

using CurrentDesk.BackOffice.Areas.SuperAdmin.Models;
using CurrentDesk.Common;
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.Dashboard;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Logging;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using CurrentDesk.Repository.CurrentDesk;
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients;
using CurrentDesk.BackOffice.Models.Dashboard;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Utilities;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Controllers
{
    /// <summary>
    /// This class represents DashboardController and contains actions to
    /// handle all functionality of IB Dashboard page
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_IB), NoCache]
    public class DashboardController : Controller
    {
        #region Variables
        private Client_AccountBO clientAccBO = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private ClientBO clientBO = new ClientBO();
        private IndividualAccountInformationBO indAccInfoBO = new IndividualAccountInformationBO();
        private JointAccountInformationBO jointAccInfoBO = new JointAccountInformationBO();
        private CorporateAccountInformationBO corpAccInfoBO = new CorporateAccountInformationBO();
        private TrustAccountInformationBO trustAccInfoBO = new TrustAccountInformationBO();
        private UserActivityBO usrActivityBO = new UserActivityBO();
        private IntroducingBrokerBO intBrokerBO = new IntroducingBrokerBO();
        private AdminTransactionBO adminTransactionBO = new AdminTransactionBO();
        private UserBO userBO = new UserBO();
        private IntroducingBrokerBO introducingBrokerBO = new IntroducingBrokerBO();
        #endregion

        /// <summary>
        /// This action returns Dashboard view of IB with
        /// required data passed as model
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new IBDashboardModel();
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    model.RebateAccDetails = new List<RebateAccount>();

                    //Get all rebate accounts of IB
                    var rebateAccInfo = clientAccBO.GetDashboardAccounts(loginInfo.LogAccountType, loginInfo.UserID);

                    //Iterate through each rebate acc
                    foreach (var acc in rebateAccInfo)
                    {
                        var rebateAcc = new RebateAccount();
                        rebateAcc.RebateAccNumber = acc.TradingAccount;
                        rebateAcc.RebateAccCurrency = lCurrValueBO.GetCurrencySymbolFromID((int)acc.FK_CurrencyID);
                        rebateAcc.Equity = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                        model.RebateAccDetails.Add(rebateAcc);
                    }

                    //Get market news
                    model.MarketNews = GetMarketNews();

                    //Latest client changes display data
                    var clientStatus = clientBO.GetStatusReport(loginInfo.UserID);
                    model.ApprovedStatus = clientStatus[0];
                    model.PendingStatus = clientStatus[1];
                    model.MissingStatus = clientStatus[2];
                    model.DeniedStatus = clientStatus[3];

                    var activityStatus = clientBO.GetActivityStatusReport(loginInfo.UserID);
                    model.NewStatus = activityStatus[0];
                    model.ActiveStatus = activityStatus[1];
                    model.InactiveStatus = activityStatus[2];
                    model.DormantStatus = activityStatus[3];

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
        /// This action returns list of market news from
        /// forexfactory xml file
        /// </summary>
        /// <returns></returns>
        public List<MarketNewsDataModel> GetMarketNews()
        {
            var lstMarketNews = new List<MarketNewsDataModel>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load("http://www.forexfactory.com/ffcal_week_this.xml");
                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("event");

                //Iterate through each news event
                //and if news date >= todays date then add to list
                foreach (XmlNode node in nodes)
                {
                    if (node["date"].InnerText.DateTimeTryParse() >= DateTime.Now.Date)
                    {
                        var marketNewsData = new MarketNewsDataModel();
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
        /// This action returns list of client details
        /// with a particular status and IsDismissed = false
        /// </summary>
        /// <param name="status">status</param>
        /// <returns></returns>
        public ActionResult GetClientListOnStatus(string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    var lstClientList = new List<IBClientsModel>();
                    var clientsOfIB = clientBO.GetAllClientsOfIBOnStatus(status, loginInfo.UserID);

                    foreach (var client in clientsOfIB)
                    {
                        //Get Individual Acc Info
                        if (client.FK_AccountTypeID == Constants.K_LIVE_INDIVIDUAL)
                        {
                            var individualDetails = indAccInfoBO.GetIndividualAccountDetails(client.PK_ClientID);
                            if (individualDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = individualDetails.FirstName;
                                model.LastName = individualDetails.LastName;
                                model.CompanyName = "N/A";
                                lstClientList.Add(model);
                            }
                        }
                        //Get Joint Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_JOINT)
                        {
                            var jointDetails = jointAccInfoBO.GetJointAccountDetails(client.PK_ClientID);
                            if (jointDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = jointDetails.PrimaryAccountHolderFirstName;
                                model.LastName = jointDetails.PrimaryAccountHolderLastName;
                                model.CompanyName = "N/A";
                                lstClientList.Add(model);
                            }
                        }
                        //Get Corporate Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_CORPORATE)
                        {
                            var corpDetails = corpAccInfoBO.GetCorporateAccountDetails(client.PK_ClientID);
                            if (corpDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = corpDetails.AuthOfficerFirstName;
                                model.LastName = corpDetails.AuthOfficerLastName;
                                model.CompanyName = corpDetails.CompanyName;
                                lstClientList.Add(model);
                            }
                        }
                        //Get Trust Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_TRUST)
                        {
                            var trustDetails = trustAccInfoBO.GetTrustAccountDetails(client.PK_ClientID);
                            if (trustDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                if (trustDetails.FK_TrusteeTypeID == 1)
                                {
                                    model.FirstName = trustDetails.TrusteeAuthOfficerFirstName;
                                    model.LastName = trustDetails.TrusteeAuthOfficerLastName;
                                    model.CompanyName = trustDetails.TrusteeCompanyName;
                                }
                                else if (trustDetails.FK_TrusteeTypeID == 2)
                                {
                                    model.FirstName = trustDetails.TrusteeIndividualFirstName;
                                    model.LastName = trustDetails.TrusteeIndividualLastName;
                                    model.CompanyName = "N/A";
                                }
                                lstClientList.Add(model);
                            }
                        }
                    }

                    //Set IsSeen of all above clients to true before return
                    clientBO.SetIsSeenForClientsAfterReportShow(status, loginInfo.UserID);

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstClientList.Count,
                        rows = lstClientList
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
        /// This action sets IsDismiss to true for each selected client
        /// </summary>
        /// <param name="clientIDs">clientIDs</param>
        /// <param name="status">status</param>
        /// <returns></returns>
        public ActionResult DismissSelectedClients(string clientIDs, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(clientBO.DismissSelectedClients(clientIDs, status, loginInfo.UserID));
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
        /// This action returns list of client details
        /// with a particular activity and IsActivityDismissed = false
        /// </summary>
        /// <param name="activity">activity</param>
        /// <returns></returns>
        public ActionResult GetClientListOnActivityStatus(string activity)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    var lstClientList = new List<IBClientsModel>();
                    var clientsOfIB = clientBO.GetAllClientsOfIBOnActivityStatus(activity, loginInfo.UserID);

                    foreach (var client in clientsOfIB)
                    {
                        //Get Individual Acc Info
                        if (client.FK_AccountTypeID == Constants.K_LIVE_INDIVIDUAL)
                        {
                            var individualDetails = indAccInfoBO.GetIndividualAccountDetails(client.PK_ClientID);
                            if (individualDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = individualDetails.FirstName;
                                model.LastName = individualDetails.LastName;
                                model.CompanyName = "N/A";
                                lstClientList.Add(model);
                            }
                        }
                        //Get Joint Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_JOINT)
                        {
                            var jointDetails = jointAccInfoBO.GetJointAccountDetails(client.PK_ClientID);
                            if (jointDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = jointDetails.PrimaryAccountHolderFirstName;
                                model.LastName = jointDetails.PrimaryAccountHolderLastName;
                                model.CompanyName = "N/A";
                                lstClientList.Add(model);
                            }
                        }
                        //Get Corporate Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_CORPORATE)
                        {
                            var corpDetails = corpAccInfoBO.GetCorporateAccountDetails(client.PK_ClientID);
                            if (corpDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.FirstName = corpDetails.AuthOfficerFirstName;
                                model.LastName = corpDetails.AuthOfficerLastName;
                                model.CompanyName = corpDetails.CompanyName;
                                lstClientList.Add(model);
                            }
                        }
                        //Get Trust Acc Info
                        else if (client.FK_AccountTypeID == Constants.K_LIVE_TRUST)
                        {
                            var trustDetails = trustAccInfoBO.GetTrustAccountDetails(client.PK_ClientID);
                            if (trustDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                if (trustDetails.FK_TrusteeTypeID == 1)
                                {
                                    model.FirstName = trustDetails.TrusteeAuthOfficerFirstName;
                                    model.LastName = trustDetails.TrusteeAuthOfficerLastName;
                                    model.CompanyName = trustDetails.TrusteeCompanyName;
                                }
                                else if (trustDetails.FK_TrusteeTypeID == 2)
                                {
                                    model.FirstName = trustDetails.TrusteeIndividualFirstName;
                                    model.LastName = trustDetails.TrusteeIndividualLastName;
                                    model.CompanyName = "N/A";
                                }
                                lstClientList.Add(model);
                            }
                        }
                    }

                    //Set IsSeen of all above clients to true before return
                    clientBO.SetIsActivitySeenForClientsAfterReportShow(activity, loginInfo.UserID);

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstClientList.Count,
                        rows = lstClientList
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
        /// This action sets IsActivityDismiss to true for each selected client
        /// </summary>
        /// <param name="clientIDs">clientIDs</param>
        /// <returns></returns>
        public ActionResult DismissSelectedClientsOnActivity(string clientIDs)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(clientBO.DismissSelectedClientsOnActivity(clientIDs, loginInfo.UserID));
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
        /// This action returns list of IB activities
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIntroducingBrokerRecentActivityDetails()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var lstUserActivities = new List<UserActivityModel>();

                    //Get latest activities
                    var activities = usrActivityBO.GetUserRecentActivityDetails(loginInfo.UserID);

                    //Set IsSeen true for new activities if any
                    usrActivityBO.MarkSeenRecentActivity(loginInfo.UserID);

                    foreach (var act in activities)
                    {
                        var usrAct = new UserActivityModel();
                        usrAct.ActivityTimestamp = Convert.ToDateTime(act.Timestamp).ToString("dd/MM/yyyy HH:mm:ss tt");
                        usrAct.IsSeen = (bool)act.IsSeen;

                        //Profile Activities
                        if (act.FK_ActivityTypeID == (int)ActivityType.ProfileActivity)
                        {
                            usrAct.ActivityDetails = act.ProfileActivities.FirstOrDefault().ProfileActivityDetails;
                        }
                        //Document activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.DocumentActivity)
                        {
                            usrAct.ActivityDetails = "<a href=Document>" +
                                                     act.DocumentActivities.FirstOrDefault().Document.DocumentName +
                                                     "</a>" + " document status has changed to <i>" +
                                                     act.DocumentActivities.FirstOrDefault().DocumentStatus + "</i>.";
                        }
                        //Account activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.AccountActivity)
                        {
                            //New acc creation activity
                            if (act.AccountActivities.FirstOrDefault().FK_AccActivityTypeID ==
                                (int)AccountActivityType.NewAccountCreation)
                            {
                                usrAct.ActivityDetails = "A new " +
                                                         act.AccountActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue + " " +
                                                         act.AccountActivities.FirstOrDefault().AccountType +
                                                         " account has been created.";
                            }
                        }
                        //Transfer activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.TransferActivity)
                        {
                            if (act.TransferActivities.FirstOrDefault().TransferStatus == Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.TransferActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.TransferActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
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
                                    if (userBO.GetUserDetails((int)act.TransferActivities.FirstOrDefault().FK_FromUserID) !=
                                        null)
                                    {
                                        fromClientName = clientBO.GetClientName((int)act.TransferActivities.FirstOrDefault().FK_FromUserID);
                                    }
                                    else
                                    {
                                        fromClientName = introducingBrokerBO.GetPartnerName((int)act.TransferActivities.FirstOrDefault().FK_FromUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been deposited from " + fromClientName + " to <a href='MyAccount'>" +
                                                             act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                                }
                                else if (act.TransferActivities.FirstOrDefault().FK_ToUserID != null)
                                {
                                    string toClientName = String.Empty;
                                    if (userBO.GetUserDetails((int)act.TransferActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName = clientBO.GetClientName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName = introducingBrokerBO.GetPartnerName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
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
                                usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
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
                        else if (act.FK_ActivityTypeID == (int)ActivityType.ConversionActivity)
                        {
                            if (act.ConversionActivities.FirstOrDefault().ConversionStatus ==
                                Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.ConversionActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.ConversionActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
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
                                                                        .ConversionAmount) *
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> and transferred to <a href='MyAccount'>" +
                                                             act.ConversionActivities.FirstOrDefault().ToAccount +
                                                             "</a>.";
                                }
                                else if (act.ConversionActivities.FirstOrDefault().FK_FromUserID != null)
                                {
                                    string fromClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int)act.ConversionActivities.FirstOrDefault().FK_FromUserID) !=
                                        null)
                                    {
                                        fromClientName =
                                            clientBO.GetClientName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_FromUserID);
                                    }
                                    else
                                    {
                                        fromClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_FromUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
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
                                                                        .ConversionAmount) *
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
                                        userBO.GetUserDetails((int)act.ConversionActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName =
                                            clientBO.GetClientName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
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
                                                                        .ConversionAmount) *
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
                                usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
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
                                                                    .ConversionAmount) *
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
                        //Withdraw fund activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.DepositOrWithdrawActivity)
                        {
                            if (act.DepositOrWithdrawActivities.FirstOrDefault().TransferStatus ==
                                Constants.K_STATUS_PENDING)
                            {
                                usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                          .Amount, "") +
                                                         " " +
                                                         act.DepositOrWithdrawActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue +
                                                         "</b> have been withdrawn from <a href='MyAccount'>" +
                                                         act.DepositOrWithdrawActivities.FirstOrDefault().AccountNumber +
                                                         "</a> and is pending transfer to " +
                                                         act.DepositOrWithdrawActivities.FirstOrDefault()
                                                            .BankAccountInformation.BankName + ".";
                            }
                            else
                            {
                                usrAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.DepositOrWithdrawActivities
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
        /// This action gets recent activities of clients under IB
        /// and formats it accordingly depending upon activity type
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientsRecentActivityDetails()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;
                    var lstUserActivities = new List<UserActivityModel>();
                    string clientIds = String.Empty;

                    var ibClients = clientBO.GetAllClientsOfIB(loginInfo.UserID);
                    foreach (var client in ibClients)
                    {
                        clientIds += client.FK_UserID + ",";
                    }

                    //Get recent client activities
                    var clientActivities = usrActivityBO.GetClientsRecentActivityDetails(clientIds.TrimEnd(','));

                    //Mark IsSeen true for new activities
                    usrActivityBO.MarkSeenRecentClientActivity(clientIds.TrimEnd(','));

                    foreach (var act in clientActivities)
                    {
                        var usrAct = new UserActivityModel();
                        usrAct.ActivityTimestamp = Convert.ToDateTime(act.Timestamp).ToString("dd/MM/yyyy hh:mm:ss tt");
                        usrAct.IsSeen = (bool)act.IsSeen;

                        //Document activities
                        if (act.FK_ActivityTypeID == (int)ActivityType.DocumentActivity)
                        {
                            usrAct.ActivityDetails = "Client " +
                                                     act.User.Clients.FirstOrDefault()
                                                        .IndividualAccountInformations.FirstOrDefault()
                                                        .FirstName + " " +
                                                     act.User.Clients.FirstOrDefault()
                                                        .IndividualAccountInformations.FirstOrDefault()
                                                        .LastName + " <a href='Document'>" +
                                                     act.DocumentActivities.FirstOrDefault().Document.DocumentName +
                                                     "</a> document status has changed to <i>" +
                                                     act.DocumentActivities.FirstOrDefault().DocumentStatus + "</i>.";
                        }
                        //Account activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.AccountActivity)
                        {
                            //New acc creation activity
                            if (act.AccountActivities.FirstOrDefault().FK_AccActivityTypeID ==
                                (int)AccountActivityType.NewAccountCreation)
                            {
                                usrAct.ActivityDetails =
                                    act.AccountActivities.FirstOrDefault().L_CurrencyValue.CurrencyValue + " " +
                                    act.AccountActivities.FirstOrDefault().AccountType +
                                    " account has been created for Client " +
                                    act.User.Clients.FirstOrDefault()
                                       .IndividualAccountInformations.FirstOrDefault()
                                       .FirstName + " " +
                                    act.User.Clients.FirstOrDefault()
                                       .IndividualAccountInformations.FirstOrDefault()
                                       .LastName + ".";
                            }
                        }
                        //Transfer activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.TransferActivity)
                        {
                            string clientName =
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .FirstName + "@" +
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .LastName;
                            string clientAccountID = act.TransferActivities.FirstOrDefault().ToAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                            int clientID = act.User.Clients.FirstOrDefault().PK_ClientID;

                            if (act.TransferActivities.FirstOrDefault().TransferStatus == Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.TransferActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.TransferActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has transferred <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from account <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.TransferActivities.FirstOrDefault().FromAccount +
                                                             "</a> to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                                }
                                else if (act.TransferActivities.FirstOrDefault().FK_ToUserID != null)
                                {
                                    string toClientName = String.Empty;
                                    if (userBO.GetUserDetails((int)act.TransferActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName = clientBO.GetClientName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName = introducingBrokerBO.GetPartnerName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has transferred <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from account <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.TransferActivities.FirstOrDefault().FromAccount +
                                                             "</a> to " +
                                                             toClientName + ".";
                                }
                                else if (act.TransferActivities.FirstOrDefault().FK_FromUserID != null)
                                {
                                    string fromClientName = String.Empty;
                                    if (userBO.GetUserDetails((int)act.TransferActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        fromClientName = clientBO.GetClientName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        fromClientName = introducingBrokerBO.GetPartnerName((int)act.TransferActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has received <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault()
                                                                              .TransferAmount, "") + " " +
                                                             act.TransferActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from account " +
                                                             fromClientName +
                                                             " to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                                }
                            }
                            else
                            {
                                usrAct.ActivityDetails = "Client " +
                                                         act.User.Clients.FirstOrDefault()
                                                            .IndividualAccountInformations.FirstOrDefault()
                                                            .FirstName + " " +
                                                         act.User.Clients.FirstOrDefault()
                                                            .IndividualAccountInformations.FirstOrDefault()
                                                            .LastName + " has <b>" + Utility.FormatCurrencyValue((decimal)act
                                                                                                       .TransferActivities
                                                                                                       .FirstOrDefault()
                                                                                                       .TransferAmount, "") +
                                                         " " +
                                                         act.TransferActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue +
                                                         "</b> pending transfer from account <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                         clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                         clientName + "'>" +
                                                         act.TransferActivities.FirstOrDefault().FromAccount +
                                                         "</a> to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                         clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                         clientName + "'>" +
                                                         act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                            }
                        }
                        //Conversion activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.ConversionActivity)
                        {
                            string clientName =
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .FirstName + "@" +
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .LastName;
                            string clientAccountID = act.ConversionActivities.FirstOrDefault().ToAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                            int clientID = act.User.Clients.FirstOrDefault().PK_ClientID;

                            if (act.ConversionActivities.FirstOrDefault().ConversionStatus ==
                                Constants.K_STATUS_TRANSFERRED)
                            {
                                if (act.ConversionActivities.FirstOrDefault().FK_FromUserID == null &&
                                    act.ConversionActivities.FirstOrDefault().FK_ToUserID == null)
                                {
                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has converted <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
                                                                              .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.ConversionActivities.FirstOrDefault().FromAccount +
                                                             "</a> at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount) *
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue1.CurrencyValue +
                                                             "</b> and transferred to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.ConversionActivities.FirstOrDefault().ToAccount +
                                                             "</a>.";
                                }
                                else if (act.ConversionActivities.FirstOrDefault().FK_ToUserID != null)
                                {
                                    string toClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        toClientName =
                                            clientBO.GetClientName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        toClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has converted <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
                                                                              .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.ConversionActivities.FirstOrDefault().FromAccount +
                                                             "</a> at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount) *
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
                                else if (act.ConversionActivities.FirstOrDefault().FK_FromUserID != null)
                                {
                                    string fromClientName = String.Empty;
                                    if (
                                        userBO.GetUserDetails(
                                            (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID) !=
                                        null)
                                    {
                                        fromClientName =
                                            clientBO.GetClientName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }
                                    else
                                    {
                                        fromClientName =
                                            introducingBrokerBO.GetPartnerName(
                                                (int)act.ConversionActivities.FirstOrDefault().FK_ToUserID);
                                    }

                                    usrAct.ActivityDetails = "Client " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .FirstName + " " +
                                                             act.User.Clients.FirstOrDefault()
                                                                .IndividualAccountInformations.FirstOrDefault()
                                                                .LastName + " has received <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
                                                                              .ConversionAmount, "") + " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from " +
                                                             fromClientName +
                                                             " at an exchange rate of " +
                                                             act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                             " totaling <b>" +
                                                             Math.Round(
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ConversionAmount) *
                                                                 Convert.ToDecimal(
                                                                     act.ConversionActivities.FirstOrDefault()
                                                                        .ExchangeRate),
                                                                 2) +
                                                             " " +
                                                             act.ConversionActivities.FirstOrDefault()
                                                                .L_CurrencyValue1.CurrencyValue +
                                                             "</b> and deposited to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.ConversionActivities.FirstOrDefault().ToAccount +
                                                             "</a>.";
                                }
                            }
                            else
                            {
                                usrAct.ActivityDetails = "Client " +
                                                         act.User.Clients.FirstOrDefault()
                                                            .IndividualAccountInformations.FirstOrDefault()
                                                            .FirstName + " " +
                                                         act.User.Clients.FirstOrDefault()
                                                            .IndividualAccountInformations.FirstOrDefault()
                                                            .LastName + " has converted <b>" +
                                                         Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault()
                                                                          .ConversionAmount, "") + " " +
                                                         act.ConversionActivities.FirstOrDefault()
                                                            .L_CurrencyValue.CurrencyValue +
                                                         "</b> from <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                         clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                         clientName + "'>" +
                                                         act.ConversionActivities.FirstOrDefault().FromAccount +
                                                         "</a> at an exchange rate of " +
                                                         act.ConversionActivities.FirstOrDefault().ExchangeRate +
                                                         " totaling <b>" +
                                                         Math.Round(
                                                             Convert.ToDecimal(
                                                                 act.ConversionActivities.FirstOrDefault()
                                                                    .ConversionAmount) *
                                                             Convert.ToDecimal(
                                                                 act.ConversionActivities.FirstOrDefault().ExchangeRate),
                                                             2) +
                                                         " " +
                                                         act.ConversionActivities.FirstOrDefault()
                                                            .L_CurrencyValue1.CurrencyValue +
                                                         "</b> and is pending transfer to <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                         clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                         clientName + "'>" +
                                                         act.ConversionActivities.FirstOrDefault().ToAccount + "</a>.";
                            }
                        }
                        //Deposit Or Withdraw Activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.DepositOrWithdrawActivity)
                        {
                            string clientName =
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .FirstName + "@" +
                                act.User.Clients.FirstOrDefault()
                                   .IndividualAccountInformations.FirstOrDefault()
                                   .LastName;
                            string clientAccountID =
                                act.DepositOrWithdrawActivities.FirstOrDefault().AccountNumber.Split('-')[ruleInfo.AccountNumberPosition - 1];
                            int clientID = act.User.Clients.FirstOrDefault().PK_ClientID;

                            //Deposit
                            if (act.DepositOrWithdrawActivities.FirstOrDefault().Type == Constants.K_DEPOSIT)
                            {
                                if (act.DepositOrWithdrawActivities.FirstOrDefault().TransferStatus ==
                                    Constants.K_STATUS_PENDING)
                                {
                                    usrAct.ActivityDetails = "Client " + clientName.Replace('@', ' ') +
                                                             " has submitted a deposit of <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") + " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue + "</b>.";
                                }
                                else
                                {
                                    usrAct.ActivityDetails = "Client " + clientName.Replace('@', ' ') +
                                                             " has received <b>" +
                                                             Utility.FormatCurrencyValue((decimal)act.DepositOrWithdrawActivities
                                                                              .FirstOrDefault().Amount, "") + " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> and been deposited into <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .AccountNumber + "</a>.";
                                }
                            }
                            //Withdraw
                            else
                            {
                                if (act.DepositOrWithdrawActivities.FirstOrDefault().TransferStatus ==
                                    Constants.K_STATUS_PENDING)
                                {
                                    usrAct.ActivityDetails = "Client " + clientName.Replace('@', ' ') +
                                                             " has withdrawn <b>" + Utility.FormatCurrencyValue((decimal)act
                                                                                                      .DepositOrWithdrawActivities
                                                                                                      .FirstOrDefault()
                                                                                                      .Amount, "") + " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> from <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .AccountNumber + "</a> and is pending transfer to " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .BankAccountInformation.BankName + ".";
                                }
                                else
                                {
                                    usrAct.ActivityDetails = "Client " + clientName.Replace('@', ' ') +
                                                             " withdrawal request of <b>" + Utility.FormatCurrencyValue((decimal)act
                                                                                                              .DepositOrWithdrawActivities
                                                                                                              .FirstOrDefault
                                                                                                              ()
                                                                                                              .Amount, "") +
                                                             " " +
                                                             act.DepositOrWithdrawActivities.FirstOrDefault()
                                                                .L_CurrencyValue.CurrencyValue +
                                                             "</b> has been withdrawn from <a href='IntroducingBrokerClients/ClientAccounts?clientID=" +
                                                             clientID + "&accountID=" + clientAccountID + "&clientName=" +
                                                             clientName + "'>" +
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
        /// This actions returns total volume of Currency trades done per day
        /// for current month
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrencyVolumeOverviewOFData()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    var monthToDateVol = 0;
                    var lstTradesVolume = new List<TradesVolume>();

                    //Loop through 1st day of current month to last day
                    for (int ctDay = 1; ctDay <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); ctDay++)
                    {
                        var vol = new TradesVolume();
                        vol.Day = DateTime.Now.ToString("MMM") + " " + ctDay;

                        if (ctDay < DateTime.Now.Day)
                        {
                            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                            //Calculate EST from UTC
                            DateTime fromDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, ctDay, 2, 30, 0).ToUniversalTime(), easternZone);
                            DateTime toDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, (ctDay + 1), 2, 29, 59).ToUniversalTime(), easternZone);
                            //Calculate epoch time from EST
                            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
                            long epochFrom = Convert.ToInt64((fromDate - epoch).TotalSeconds);
                            long epochTo = Convert.ToInt64((toDate - epoch).TotalSeconds);

                            //Get volume for the day
                            vol.Volume = (int)intBrokerBO.GetCurrencyTradesVolumeByDay(intBrokerBO.GetBrokerIDFromBrokerUserID(loginInfo.UserID), epochFrom, epochTo) * 1000;
                            monthToDateVol += vol.Volume;
                        }
                        else
                        {
                            vol.Volume = 0;
                        }

                        lstTradesVolume.Add(vol);
                    }

                    //Add month to volume data to list
                    TradesVolume volMonthToDate = new TradesVolume();
                    volMonthToDate.Day = "MonthToDate";
                    volMonthToDate.Volume = monthToDateVol;
                    lstTradesVolume.Add(volMonthToDate);

                    return Json(lstTradesVolume, JsonRequestBehavior.AllowGet);
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
        /// This actions returns total volume of Currency trades done per day
        /// for current month
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrencyVolumeOverviewData()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get all the Platform login
                    List<int?> platformLoginList = introducingBrokerBO.GetPlatformLoginsIntroducingBroker(loginInfo.UserID);

                    long epochFrom = GetESTFromUTCStart(1);
                    long epochTo = GetESTFromUTCEnd(DateTime.UtcNow.Day);

                    //Get All Closed Trades
                    var tradesHistoryBO = new TradesHistoryBO();
                    var resultantTrade = tradesHistoryBO.GetAllCurrencyClosedTradesByPlatformLogin(platformLoginList, epochFrom, epochTo);

                    List<TradesVolume> lstTradesVolume = new List<TradesVolume>();
                    int monthToDateVol = 0;

                    for (int ctDay = 1; ctDay <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); ctDay++)
                    {
                        TradesVolume vol = new TradesVolume();
                        vol.Day = DateTime.Now.ToString("MMM") + " " + ctDay;
                        if (ctDay < DateTime.Now.Day)
                        { 
                            long selEpochFrom = GetESTFromUTCStart(ctDay);
                            long selEpochTo = GetESTFromUTCEnd(ctDay + 1);

                            vol.Volume = (int)resultantTrade.Where(x => x.Timestamp > selEpochFrom && x.Timestamp < selEpochTo ).Sum(x => x.Volume) * 1000;
                            monthToDateVol += vol.Volume;
                        }
                        else
                        {
                            vol.Volume = 0;
                        }

                        lstTradesVolume.Add(vol);
                    }

                    //Add month to volume data to list
                    TradesVolume volMonthToDate = new TradesVolume();
                    volMonthToDate.Day = "MonthToDate";
                    volMonthToDate.Volume = monthToDateVol;
                    lstTradesVolume.Add(volMonthToDate);

                    return Json(lstTradesVolume, JsonRequestBehavior.AllowGet);
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
        /// This actions returns total volume of CFD trades done per day
        /// for current month
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCFDVolumeOverviewData()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get all the Platform login
                    List<int?> platformLoginList = introducingBrokerBO.GetPlatformLoginsIntroducingBroker(loginInfo.UserID);

                    long epochFrom = GetESTFromUTCStart(1);
                    long epochTo = GetESTFromUTCEnd(DateTime.UtcNow.Day);

                    //Get All Closed Trades
                    var tradesHistoryBO = new TradesHistoryBO();
                    var resultantTrade = tradesHistoryBO.GetAllCurrencyCFDTradesByPlatformLogin(platformLoginList, epochFrom, epochTo);

                    List<TradesVolume> lstTradesVolume = new List<TradesVolume>();
                    int monthToDateVol = 0;

                    for (int ctDay = 1; ctDay <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); ctDay++)
                    {
                        TradesVolume vol = new TradesVolume();
                        vol.Day = DateTime.Now.ToString("MMM") + " " + ctDay;
                        if (ctDay < DateTime.Now.Day)
                        {
                            long selEpochFrom = GetESTFromUTCStart(ctDay);
                            long selEpochTo = GetESTFromUTCEnd(ctDay + 1);

                            vol.Volume = (int)resultantTrade.Where(x => x.Timestamp > selEpochFrom && x.Timestamp < selEpochTo).Sum(x => x.Volume) * 1000;
                            monthToDateVol += vol.Volume;
                        }
                        else
                        {
                            vol.Volume = 0;
                        }

                        lstTradesVolume.Add(vol);
                    }

                    //Add month to volume data to list
                    TradesVolume volMonthToDate = new TradesVolume();
                    volMonthToDate.Day = "MonthToDate";
                    volMonthToDate.Volume = monthToDateVol;
                    lstTradesVolume.Add(volMonthToDate);

                    return Json(lstTradesVolume, JsonRequestBehavior.AllowGet);
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
        /// This actions returns total volume of CFD trades done per day
        /// for current month
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCFDVolumeOverviewOFData()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    int monthToDateVol = 0;
                    List<TradesVolume> lstTradesVolume = new List<TradesVolume>();

                    //Loop through 1st day of current month to last day
                    for (int ctDay = 1; ctDay <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); ctDay++)
                    {
                        var vol = new TradesVolume();
                        vol.Day = DateTime.Now.ToString("MMM") + " " + ctDay;

                        if (ctDay < DateTime.Now.Day)
                        {
                            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                            //Calculate EST from UTC
                            DateTime fromDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, ctDay, 2, 30, 0).ToUniversalTime(), easternZone);
                            DateTime toDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, (ctDay + 1), 2, 29, 59).ToUniversalTime(), easternZone);
                            //Calculate epoch time from EST
                            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
                            long epochFrom = Convert.ToInt64((fromDate - epoch).TotalSeconds);
                            long epochTo = Convert.ToInt64((toDate - epoch).TotalSeconds);

                            //Get volume for the day
                            vol.Volume = (int)intBrokerBO.GetCFDTradesVolumeByDay(intBrokerBO.GetBrokerIDFromBrokerUserID(loginInfo.UserID), epochFrom, epochTo) * 1000;
                            monthToDateVol += vol.Volume;
                        }
                        else
                        {
                            vol.Volume = 0;
                        }

                        lstTradesVolume.Add(vol);
                    }

                    //Add month to volume data to list
                    var volMonthToDate = new TradesVolume();
                    volMonthToDate.Day = "MonthToDate";
                    volMonthToDate.Volume = monthToDateVol;
                    lstTradesVolume.Add(volMonthToDate);

                    return Json(lstTradesVolume, JsonRequestBehavior.AllowGet);
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

        #region Client Transactions
        /// <summary>
        /// This action returns list of withdrawal transactions
        /// of clients under this IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientWithdrawalTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string clientIds = String.Empty;

                    //Get all clients of IB
                    var ibClients = clientBO.GetAllClientsOfIB(loginInfo.UserID);

                    foreach (var client in ibClients)
                    {
                        clientIds += client.FK_UserID + ",";
                    }

                    //Get all withdraw transactions of IB clients
                    var withdrawTransactions =
                        adminTransactionBO.GetAllClientsTransactionOfParticulaType(clientIds.TrimEnd(','),
                                                                                   (int)
                                                                                   AdminTransactionType.OutgoingFunds,
                                                                                   (int)
                                                                                   SessionManagement.OrganizationID);

                    var lstWithdrawals = new List<DashboardTransactionModel>();

                    //Iterating through each withdrawal transaction
                    foreach (var transaction in withdrawTransactions)
                    {
                        var withdrawal = new DashboardTransactionModel();
                        withdrawal.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        withdrawal.AccountNumber = transaction.AccountNumber;
                        withdrawal.ClientName = transaction.ClientName;
                        withdrawal.Amount = Utility.FormatCurrencyValue((decimal)transaction.TransactionAmount, "");
                        withdrawal.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstWithdrawals.Add(withdrawal);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstWithdrawals.Count,
                            rows = lstWithdrawals
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
                throw;
            }
        }

        /// <summary>
        /// This action returns list of deposit transactions
        /// of clients under this IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientDepositTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string clientIds = String.Empty;

                    //Get all clients of IB
                    var ibClients = clientBO.GetAllClientsOfIB(loginInfo.UserID);

                    foreach (var client in ibClients)
                    {
                        clientIds += client.FK_UserID + ",";
                    }

                    //Get all deposit transactions of IB clients
                    var depositTransactions =
                        adminTransactionBO.GetAllClientsTransactionOfParticulaType(clientIds.TrimEnd(','),
                                                                                   (int)
                                                                                   AdminTransactionType.IncomingFunds,
                                                                                   (int)
                                                                                   SessionManagement.OrganizationID);

                    var lstWithdrawals = new List<DashboardTransactionModel>();

                    //Iterating through each deposit transaction
                    foreach (var transaction in depositTransactions)
                    {
                        var deposit = new DashboardTransactionModel();
                        deposit.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        deposit.AccountNumber = transaction.AccountNumber;
                        deposit.ClientName = transaction.ClientName;
                        deposit.Amount = Utility.FormatCurrencyValue((decimal)transaction.TransactionAmount, "");
                        deposit.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstWithdrawals.Add(deposit);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstWithdrawals.Count,
                            rows = lstWithdrawals
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
                throw;
            }
        }

        /// <summary>
        /// This action returns list of internal transfer 
        /// transactions of clients under this IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientInternalTransferTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string clientIds = String.Empty;

                    //Get all clients of IB
                    var ibClients = clientBO.GetAllClientsOfIB(loginInfo.UserID);

                    foreach (var client in ibClients)
                    {
                        clientIds += client.FK_UserID + ",";
                    }

                    //Get all internal transactions of IB clients
                    var internalTransactions =
                        adminTransactionBO.GetAllClientsTransactionOfParticulaType(clientIds.TrimEnd(','),
                                                                                   (int)
                                                                                   AdminTransactionType
                                                                                       .InternalTransfers,
                                                                                   (int)
                                                                                   SessionManagement.OrganizationID);

                    var lstInternalTransfers = new List<DashboardTransactionModel>();

                    //Iterating through each internal transfer transaction
                    foreach (var transaction in internalTransactions)
                    {
                        var internalTran = new DashboardTransactionModel();
                        internalTran.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        internalTran.AccountNumber = transaction.AccountNumber;
                        internalTran.ClientName = transaction.ClientName;
                        internalTran.ToAccount = transaction.ToAccountNumber;
                        internalTran.ToClientName = transaction.ToClientName;
                        internalTran.Amount = Utility.FormatCurrencyValue((decimal)transaction.TransactionAmount, "");
                        internalTran.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstInternalTransfers.Add(internalTran);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstInternalTransfers.Count,
                            rows = lstInternalTransfers
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
                throw;
            }
        }

        /// <summary>
        /// This action returns list of conversion
        /// transactions of clients under this IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientConversionTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string clientIds = String.Empty;

                    //Get all clients of IB
                    var ibClients = clientBO.GetAllClientsOfIB(loginInfo.UserID);

                    foreach (var client in ibClients)
                    {
                        clientIds += client.FK_UserID + ",";
                    }

                    //Get all conversion transactions of IB clients
                    var conversionTransactions =
                        adminTransactionBO.GetAllClientsTransactionOfParticulaType(clientIds.TrimEnd(','),
                                                                                   (int)
                                                                                   AdminTransactionType
                                                                                       .ConversionsRequests,
                                                                                   (int)
                                                                                   SessionManagement.OrganizationID);

                    var lstConversionTransfers = new List<DashboardTransactionModel>();

                    //Iterating through each conversion transaction
                    foreach (var transaction in conversionTransactions)
                    {
                        var convTran = new DashboardTransactionModel();
                        convTran.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        convTran.AccountNumber = transaction.AccountNumber;
                        convTran.ClientName = transaction.ClientName;
                        convTran.ToAccount = transaction.ToAccountNumber;
                        convTran.ToClientName = transaction.ToClientName;
                        convTran.Amount = Utility.FormatCurrencyValue((decimal)transaction.TransactionAmount, "");
                        convTran.ExchangeRate = (double)transaction.ExchangeRate;
                        convTran.ExchangedAmount =
                            Utility.FormatCurrencyValue(
                                Math.Round(
                                    (decimal)(transaction.TransactionAmount * (decimal)transaction.ExchangeRate), 2), "");
                        convTran.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstConversionTransfers.Add(convTran);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstConversionTransfers.Count,
                            rows = lstConversionTransfers
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
                throw;
            }
        }
        #endregion

        #region Common
        /// <summary>
        /// This Function will convert date 
        /// from EST to UTC 
        /// </summary>
        /// <param name="day">day</param>
        /// <returns>Epoch Value</returns>
        public long GetESTFromUTCStart(int day)
        {
            //Calculate EST from UTC
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime selectedDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, 2, 30, 0).ToUniversalTime(), easternZone); 
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            return Convert.ToInt64((selectedDate - epoch).TotalSeconds);            
        }


        /// <summary>
        /// This Function will convert date 
        /// from EST to UTC 
        /// </summary>
        /// <param name="day">day</param>
        /// <returns>Epoch Value</returns>
        public long GetESTFromUTCEnd(int day)
        {
            //Calculate EST from UTC
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime selectedDate = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, (day), 2, 29, 59).ToUniversalTime(), easternZone);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            return Convert.ToInt64((selectedDate - epoch).TotalSeconds);
        }

        #endregion
    }
}
