#region Header Information
/******************************************************************
 * File Name     : IntroducingBrokerClientsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 26th March 2013
 * Modified Date : 26th March 2013
 * Description   : This file contains IBClients controller and 
 *                 related actions for handling MyClients section functionality
 * ***************************************************************/
#endregion

#region Namespace Used
using System.Linq;
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Models;
using CurrentDesk.BackOffice.Models.MyAccount;
using System.Globalization;
using CurrentDesk.BackOffice.Models.Transfers;
using CurrentDesk.Models;
using System.Web;
using System.IO;
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBAgents;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Utilities;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Controllers
{
    /// <summary>
    /// This class represents IBClients controller and contains actions for 
    /// handling MyClients section functionality
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_IB), NoCache]
    public class IntroducingBrokerClientsController : Controller
    {
        #region Variables
        private ClientBO clientBO = new ClientBO();
        private IndividualAccountInformationBO indAccInfoBO = new IndividualAccountInformationBO();
        private JointAccountInformationBO jointAccInfoBO = new JointAccountInformationBO();
        private CorporateAccountInformationBO corpAccInfoBO = new CorporateAccountInformationBO();
        private TrustAccountInformationBO trustAccInfoBO = new TrustAccountInformationBO();
        private L_CountryBO countryBO = new L_CountryBO();
        private L_IDInformationTypeBO idInfoTypeBO = new L_IDInformationTypeBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private AccountCurrencyBO accountCurrencyBO = new AccountCurrencyBO();
        private L_AccountCodeBO accountCodeBO = new L_AccountCodeBO();
        private TradingPlatformBO tradingPlatformBO = new TradingPlatformBO();
        private Client_AccountBO clientAccBo = new Client_AccountBO();
        private L_CurrencyValueBO lCurrValueBO = new L_CurrencyValueBO();
        private TransferLogBO transferLogBO = new TransferLogBO();
        private TransactionBO transactionBO = new TransactionBO();
        private BankAccountInformationBO bankBO = new BankAccountInformationBO();
        private R_UserDocumentBO r_UserDocumentBO = new R_UserDocumentBO();
        private UserDocumentBO userDocumentBO = new UserDocumentBO();
        private ClientNoteBO clientNotesBO = new ClientNoteBO();
        private AgentBO agentBO = new AgentBO();
        private PartnerCommissionBO partCommBO = new PartnerCommissionBO();
        private UserActivityBO usrActivityBO = new UserActivityBO();
        private AccountTypeBO accountTypeBO = new AccountTypeBO();
        #endregion

        /// <summary>
        /// This action returns Index view of MyClients section
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
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
        /// This action returns list of client details under a
        /// particular IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClientsList()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    var lstClientList = new List<IBClientsModel>();
                    var clientsOfIB = clientBO.GetAllClientsOfIB(loginInfo.UserID);  

                    foreach (var client in clientsOfIB)
                    {
                        //Get account type details
                        var accountTypeDetails = accountTypeBO.GetAccountTypeAndFormTypeValue((int)client.FK_AccountTypeID);

                        //Get Individual Acc Info
                        if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
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
                                model.Activity = clientAccBo.GetClientActivityStatus(client.PK_ClientID);
                                model.Status = client.Status;
                                lstClientList.Add(model);
                            }
                        }
                        //Get Joint Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
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
                                model.Activity = clientAccBo.GetClientActivityStatus(client.PK_ClientID);
                                model.Status = client.Status;
                                lstClientList.Add(model);
                            }
                        }
                        //Get Corporate Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
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
                                model.Activity = clientAccBo.GetClientActivityStatus(client.PK_ClientID);
                                model.Status = client.Status;
                                lstClientList.Add(model);
                            }
                        }
                        //Get Trust Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_TRUST)
                        {
                            var trustDetails = trustAccInfoBO.GetTrustAccountDetails(client.PK_ClientID);
                            if (trustDetails != null)
                            {
                                var model = new IBClientsModel();
                                model.PK_ClientID = client.PK_ClientID;
                                model.AccountID = client.Client_Account.FirstOrDefault().LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1];
                                model.Activity = clientAccBo.GetClientActivityStatus(client.PK_ClientID);
                                model.Status = client.Status;
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
        /// This action returns ClientDetails view with
        /// client data passed as model
        /// </summary>
        /// <returns></returns>
        public ActionResult ClientDetails(int clientID, string accountID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    ViewData["Agents"] = new SelectList(agentBO.GetAllAgentsOfIB(loginInfo.UserID), "PK_AgentID", "FirstName");

                    var model = new ClientDetailsModel();
                    var selectedClient = clientBO.GetClientInformationOnClientPK(clientID);
                    var userInformation = selectedClient.User;
                    var bankInformationList = selectedClient.BankAccountInformations;
                    var clientAccInfo = selectedClient.Client_Account.FirstOrDefault();
                    var bankList = new List<BankAccountModel>();

                    if (selectedClient != null)
                    {
                        model.ClientID = clientID;
                        model.AccountID = accountID;
                        model.Status = selectedClient.Status;
                        model.AgentID = selectedClient.FK_AgentID;

                        //Get account type details
                        var accountTypeDetails = accountTypeBO.GetAccountTypeAndFormTypeValue((int)selectedClient.FK_AccountTypeID);

                        if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
                        {
                            var individualDetails = indAccInfoBO.GetIndividualAccountDetails(selectedClient.PK_ClientID);
                            if (individualDetails != null)
                            {
                                #region Individual Information
                                model.Title = individualDetails.Title != null ? (individualDetails.Title == "1" ? "Mr." : "Mrs.") : "";
                                model.FirstName = individualDetails.FirstName ?? "";
                                model.MiddleName = individualDetails.MiddleName ?? "";
                                model.LastName = individualDetails.LastName ?? "";
                                model.DobMonth = Convert.ToDateTime(individualDetails.BirthDate).Month.ToString("D2");
                                model.DobDay = Convert.ToDateTime(individualDetails.BirthDate).Day.ToString("D2");
                                model.DobYear = Convert.ToDateTime(individualDetails.BirthDate).Year;
                                model.Gender = individualDetails.Gender != null ? (individualDetails.Gender == true ? "Male" : "Female") : "";
                                model.Citizenship = individualDetails.FK_CitizenShipCountryID != null ? countryBO.GetSelectedCountry((int)individualDetails.FK_CitizenShipCountryID) : "";
                                model.IdInfo = individualDetails.FK_IDInformationID != null ? idInfoTypeBO.GetSelectedIDInformation((int)individualDetails.FK_IDInformationID) : "";
                                model.IdNumber = individualDetails.IDNumber != null ? individualDetails.IDNumber : "";
                                model.ResidenceCountry = countryBO.GetSelectedCountry((int)individualDetails.FK_ResidenceCountryID);
                                model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1] : "";
                                model.PhoneID = individualDetails.PhoneID ?? "";

                                model.ResidentialAddLine1 = individualDetails.ResidentialAddress != null ? (individualDetails.ResidentialAddress.Split('@')[0] + " " + individualDetails.ResidentialAddress.Split('@')[1]) : "";
                                model.ResidentialCity = individualDetails.ResidentialAddressCity ?? "";
                                model.ResidentialCountry = individualDetails.FK_ResidentialAddressCountryID != null ? countryBO.GetSelectedCountry((int)individualDetails.FK_ResidentialAddressCountryID) : "";
                                model.ResidentialPostalCode = individualDetails.ResidentialAddressPostalCode ?? "";
                                model.YearsInCurrentAdd = individualDetails.MonthsAtCurrentAddress != null ? (int)(individualDetails.MonthsAtCurrentAddress / 12) : 0;
                                model.MonthsInCurrentAdd = individualDetails.MonthsAtCurrentAddress != null ? (int)(individualDetails.MonthsAtCurrentAddress % 12) : 0;
                                model.PreviousAddLine1 = individualDetails.PreviousAddress != null ? (individualDetails.PreviousAddress.Split('@')[0] + " " + individualDetails.PreviousAddress.Split('@')[1]) : "";
                                model.PreviousCity = individualDetails.PreviousAddressCity ?? "";
                                model.PreviousCountry = individualDetails.FK_PreviousAddressCounrtyID != null ? countryBO.GetSelectedCountry((int)individualDetails.FK_PreviousAddressCounrtyID) : "";
                                model.PreviousPostalCode = individualDetails.PreviousAddressPostalCode ?? "";
                                model.TelNumberCountryCode = Convert.ToInt64(individualDetails.TelephoneNumber.Split('-')[0]);
                                model.TelNumber = Convert.ToInt64(individualDetails.TelephoneNumber.Split('-')[1]);
                                model.MobileNumberCountryCode = Convert.ToInt64(individualDetails.MobileNumber.Split('-')[0]);
                                model.MobileNumber = Convert.ToInt64(individualDetails.MobileNumber.Split('-')[1]);
                                model.EmailAddress = userInformation.UserEmailID ?? "";

                                if (bankInformationList != null)
                                {
                                    foreach (var item in bankInformationList)
                                    {
                                        var bankAccountModel = new BankAccountModel()
                                        {
                                            BankAccountID = item.PK_BankAccountInformationID,
                                            BankName = item.BankName ?? "",
                                            AccountNumber = item.AccountNumber ?? "",
                                            BicOrSwiftCode = item.BicNumberOrSwiftCode ?? "",
                                            ReceivingBankInfoId = item.FK_ReceivingBankInformationID != null ? receivingBankInfoBO.GetSelectedRecievingBankInfo((int)item.FK_ReceivingBankInformationID) : "",
                                            ReceivingBankInfo = item.ReceivingBankInfo ?? "",
                                            BankAddLine1 = item.BankingAddress != null ? item.BankingAddress.Split('@')[0] : "",
                                            BankAddLine2 = item.BankingAddress != null ? item.BankingAddress.Split('@')[1] : "",
                                            BankCity = item.City ?? "",
                                            BankCountry = item.FK_CountryID != null ? countryBO.GetSelectedCountry((int)item.FK_CountryID) : "",
                                            BankPostalCode = item.PostalCode ?? ""
                                        };

                                        bankList.Add(bankAccountModel);
                                    }
                                    model.BankAccountModelList = bankList;
                                }
                                #endregion
                            }

                        }
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
                        {
                            var jointDetails = jointAccInfoBO.GetJointAccountDetails(selectedClient.PK_ClientID);
                            if (jointDetails != null)
                            {
                                #region JointInformation
                                model.Title = jointDetails.PrimaryAccountHolderTitle != null ? (jointDetails.PrimaryAccountHolderTitle == "1" ? "Mr." : "Mrs.") : "";
                                model.FirstName = jointDetails.PrimaryAccountHolderFirstName ?? "";
                                model.MiddleName = jointDetails.PrimaryAccountHolderMiddleName ?? "";
                                model.LastName = jointDetails.PrimaryAccountHolderLastName ?? "";
                                model.DobMonth = Convert.ToDateTime(jointDetails.PrimaryAccountHolderBirthDate).Month.ToString("D2");
                                model.DobDay = Convert.ToDateTime(jointDetails.PrimaryAccountHolderBirthDate).Day.ToString("D2");
                                model.DobYear = Convert.ToDateTime(jointDetails.PrimaryAccountHolderBirthDate).Year;
                                model.Gender = jointDetails.PrimaryAccountHolderGender != null ? (jointDetails.PrimaryAccountHolderGender == true ? "Male" : "Female") : "";
                                model.Citizenship = jointDetails.FK_PrimaryAccountHolderCitizenshipCountryID != null ? countryBO.GetSelectedCountry((int)jointDetails.FK_PrimaryAccountHolderCitizenshipCountryID) : "";
                                model.IdInfo = jointDetails.FK_PrimaryAccountHolderIDTypeID != null ? idInfoTypeBO.GetSelectedIDInformation((int)jointDetails.FK_PrimaryAccountHolderIDTypeID) : "";
                                model.IdNumber = jointDetails.PrimaryAccountHolderIDNumber != null ? jointDetails.PrimaryAccountHolderIDNumber : "";
                                model.ResidenceCountry = countryBO.GetSelectedCountry((int)jointDetails.FK_PrimaryAccountHolderResidenceCountryID);
                                model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1] : "";
                                model.PhoneID = jointDetails.PhoneID ?? "";

                                model.ResidentialAddLine1 = jointDetails.ResidentialAddress != null ? (jointDetails.ResidentialAddress.Split('@')[0] + " " + jointDetails.ResidentialAddress.Split('@')[1]) : "";
                                model.ResidentialCity = jointDetails.ResidentialAddressCity ?? "";
                                model.ResidentialCountry = jointDetails.FK_ResidentialAddressCountryID != null ? countryBO.GetSelectedCountry((int)jointDetails.FK_ResidentialAddressCountryID) : "";
                                model.ResidentialPostalCode = jointDetails.ResidentialAddressPostalCode ?? "";
                                model.YearsInCurrentAdd = jointDetails.MonthsAtCurrentAddress != null ? (int)(jointDetails.MonthsAtCurrentAddress / 12) : 0;
                                model.MonthsInCurrentAdd = jointDetails.MonthsAtCurrentAddress != null ? (int)(jointDetails.MonthsAtCurrentAddress % 12) : 0;
                                model.PreviousAddLine1 = jointDetails.PreviousAddress != null ? (jointDetails.PreviousAddress.Split('@')[0] + " " + jointDetails.PreviousAddress.Split('@')[1]) : "";
                                model.PreviousCity = jointDetails.PreviousAddressCity ?? "";
                                model.PreviousCountry = jointDetails.FK_PreviousAddressCounrtyID != null ? countryBO.GetSelectedCountry((int)jointDetails.FK_PreviousAddressCounrtyID) : "";
                                model.PreviousPostalCode = jointDetails.PreviousAddressPostalCode ?? "";
                                model.TelNumberCountryCode = Convert.ToInt64(jointDetails.TelephoneNumber.Split('-')[0]);
                                model.TelNumber = Convert.ToInt64(jointDetails.TelephoneNumber.Split('-')[1]);
                                model.MobileNumberCountryCode = Convert.ToInt64(jointDetails.MobileNumber.Split('-')[0]);
                                model.MobileNumber = Convert.ToInt64(jointDetails.MobileNumber.Split('-')[1]);
                                model.EmailAddress = userInformation.UserEmailID ?? "";

                                if (bankInformationList != null)
                                {
                                    foreach (var item in bankInformationList)
                                    {
                                        var bankAccountModel = new BankAccountModel()
                                        {
                                            BankAccountID = item.PK_BankAccountInformationID,
                                            BankName = item.BankName ?? "",
                                            AccountNumber = item.AccountNumber ?? "",
                                            BicOrSwiftCode = item.BicNumberOrSwiftCode ?? "",
                                            ReceivingBankInfoId = item.FK_ReceivingBankInformationID != null ? receivingBankInfoBO.GetSelectedRecievingBankInfo((int)item.FK_ReceivingBankInformationID) : "",
                                            ReceivingBankInfo = item.ReceivingBankInfo ?? "",
                                            BankAddLine1 = item.BankingAddress != null ? item.BankingAddress.Split('@')[0] : "",
                                            BankAddLine2 = item.BankingAddress != null ? item.BankingAddress.Split('@')[1] : "",
                                            BankCity = item.City ?? "",
                                            BankCountry = item.FK_CountryID != null ? countryBO.GetSelectedCountry((int)item.FK_CountryID) : "",
                                            BankPostalCode = item.PostalCode ?? ""
                                        };

                                        bankList.Add(bankAccountModel);
                                    }
                                    model.BankAccountModelList = bankList;
                                }
                                #endregion
                            }
                        }
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
                        {
                            var corpDetails = corpAccInfoBO.GetCorporateAccountDetails(selectedClient.PK_ClientID);
                            if (corpDetails != null)
                            {
                                #region Corporate Information
                                model.Title = corpDetails.AuthOfficerTitle != null ? (corpDetails.AuthOfficerTitle == "1" ? "Mr." : "Mrs.") : "";
                                model.FirstName = corpDetails.AuthOfficerFirstName ?? "";
                                model.MiddleName = corpDetails.AuthOfficerMiddleName ?? "";
                                model.LastName = corpDetails.AuthOfficerLastName ?? "";
                                model.DobMonth = Convert.ToDateTime(corpDetails.AuthOfficerBirthDate).Month.ToString("D2");
                                model.DobDay = Convert.ToDateTime(corpDetails.AuthOfficerBirthDate).Day.ToString("D2");
                                model.DobYear = Convert.ToDateTime(corpDetails.AuthOfficerBirthDate).Year;
                                model.Gender = corpDetails.AuthOfficerGender != null ? (corpDetails.AuthOfficerGender == true ? "Male" : "Female") : "";
                                model.Citizenship = corpDetails.FK_AuthOfficerCitizenshipCountryID != null ? countryBO.GetSelectedCountry((int)corpDetails.FK_AuthOfficerCitizenshipCountryID) : "";
                                model.IdInfo = corpDetails.FK_AuthOfficerInformationTypeID != null ? idInfoTypeBO.GetSelectedIDInformation((int)corpDetails.FK_AuthOfficerInformationTypeID) : "";
                                model.IdNumber = corpDetails.AuthOfficerIDNumber != null ? corpDetails.AuthOfficerIDNumber : "";
                                model.ResidenceCountry = countryBO.GetSelectedCountry((int)corpDetails.FK_AuthOfficerResidenceCountryID);
                                model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1] : "";
                                model.PhoneID = corpDetails.PhoneID ?? "";

                                model.ResidentialAddLine1 = corpDetails.AuthOfficerAddress != null ? (corpDetails.AuthOfficerAddress.Split('@')[0] + " " + corpDetails.AuthOfficerAddress.Split('@')[1]) : "";
                                model.ResidentialCity = corpDetails.AuthOfficerCity ?? "";
                                model.ResidentialCountry = corpDetails.FK_AuthOfficerResidenceCountryID != null ? countryBO.GetSelectedCountry((int)corpDetails.FK_AuthOfficerResidenceCountryID) : "";
                                model.ResidentialPostalCode = corpDetails.AuthOfficerPostalCode ?? "";
                                model.TelNumberCountryCode = Convert.ToInt64(corpDetails.AuthOfficerTelephoneNumber.Split('-')[0]);
                                model.TelNumber = Convert.ToInt64(corpDetails.AuthOfficerTelephoneNumber.Split('-')[1]);
                                model.MobileNumberCountryCode = Convert.ToInt64(corpDetails.AuthOfficerMobileNumber.Split('-')[0]);
                                model.MobileNumber = Convert.ToInt64(corpDetails.AuthOfficerMobileNumber.Split('-')[1]);
                                model.EmailAddress = userInformation.UserEmailID ?? "";

                                if (bankInformationList != null)
                                {
                                    foreach (var item in bankInformationList)
                                    {
                                        var bankAccountModel = new BankAccountModel()
                                        {
                                            BankAccountID = item.PK_BankAccountInformationID,
                                            BankName = item.BankName ?? "",
                                            AccountNumber = item.AccountNumber ?? "",
                                            BicOrSwiftCode = item.BicNumberOrSwiftCode ?? "",
                                            ReceivingBankInfoId = item.FK_ReceivingBankInformationID != null ? receivingBankInfoBO.GetSelectedRecievingBankInfo((int)item.FK_ReceivingBankInformationID) : "",
                                            ReceivingBankInfo = item.ReceivingBankInfo ?? "",
                                            BankAddLine1 = item.BankingAddress != null ? item.BankingAddress.Split('@')[0] : "",
                                            BankAddLine2 = item.BankingAddress != null ? item.BankingAddress.Split('@')[1] : "",
                                            BankCity = item.City ?? "",
                                            BankCountry = item.FK_CountryID != null ? countryBO.GetSelectedCountry((int)item.FK_CountryID) : "",
                                            BankPostalCode = item.PostalCode ?? ""
                                        };

                                        bankList.Add(bankAccountModel);
                                    }
                                    model.BankAccountModelList = bankList;
                                }
                                #endregion
                            }
                        }
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_TRUST)
                        {
                            #region Trust Information
                            var trustDetails = trustAccInfoBO.GetTrustAccountDetails(selectedClient.PK_ClientID);
                            if (trustDetails != null)
                            {
                                if (trustDetails.FK_TrusteeTypeID == 1)
                                {
                                    model.Title = trustDetails.TrusteeAuthOfficerTitle != null ? (trustDetails.TrusteeAuthOfficerTitle == "1" ? "Mr." : "Mrs.") : "";
                                    model.FirstName = trustDetails.TrusteeAuthOfficerFirstName ?? "";
                                    model.MiddleName = trustDetails.TrusteeAuthOfficerMiddleName ?? "";
                                    model.LastName = trustDetails.TrusteeAuthOfficerLastName ?? "";
                                    model.DobMonth = Convert.ToDateTime(trustDetails.TrusteeAuthOfficerBirthDate).Month.ToString("D2");
                                    model.DobDay = Convert.ToDateTime(trustDetails.TrusteeAuthOfficerBirthDate).Day.ToString("D2");
                                    model.DobYear = Convert.ToDateTime(trustDetails.TrusteeAuthOfficerBirthDate).Year;
                                    model.Gender = trustDetails.TrusteeAuthOfficerGender != null ? (trustDetails.TrusteeAuthOfficerGender == true ? "Male" : "Female") : "";
                                    model.Citizenship = trustDetails.FK_TrusteeAuthOfficerCitizenshipCountryID != null ? countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeAuthOfficerCitizenshipCountryID) : "";
                                    model.IdInfo = trustDetails.FK_TrusteeAuthOfficerIDType != null ? idInfoTypeBO.GetSelectedIDInformation((int)trustDetails.FK_TrusteeAuthOfficerIDType) : "";
                                    model.IdNumber = trustDetails.TrusteeAuthOfficerIDNumber != null ? trustDetails.TrusteeAuthOfficerIDNumber : "";
                                    model.ResidenceCountry = countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeAuthOfficerResidenceCountryID);
                                    model.PhoneID = trustDetails.PhoneID ?? "";

                                    model.ResidentialAddLine1 = trustDetails.TrusteeAuthOfficerAddrerss != null ? (trustDetails.TrusteeAuthOfficerAddrerss.Split('@')[0] + " " + trustDetails.TrusteeAuthOfficerAddrerss.Split('@')[1]) : "";
                                    model.ResidentialCity = trustDetails.TrusteeAuthOfficerCity ?? "";
                                    model.ResidentialCountry = trustDetails.FK_TrusteeAuthOfficerResidenceCountryID != null ? countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeAuthOfficerResidenceCountryID) : "";
                                    model.ResidentialPostalCode = trustDetails.TrusteeAuthOfficerPostalCode ?? "";
                                    model.TelNumberCountryCode = Convert.ToInt64(trustDetails.TrusteeAuthOfficerTelephoneNumber.Split('-')[0]);
                                    model.TelNumber = Convert.ToInt64(trustDetails.TrusteeAuthOfficerTelephoneNumber.Split('-')[1]);
                                    model.MobileNumberCountryCode = Convert.ToInt64(trustDetails.TrusteeAuthOfficerMobileNumber.Split('-')[0]);
                                    model.MobileNumber = Convert.ToInt64(trustDetails.TrusteeAuthOfficerMobileNumber.Split('-')[1]);
                                }
                                else if (trustDetails.FK_TrusteeTypeID == 2)
                                {
                                    model.Title = trustDetails.TrusteeIndividualTitle != null ? (trustDetails.TrusteeIndividualTitle == "1" ? "Mr." : "Mrs.") : "";
                                    model.FirstName = trustDetails.TrusteeIndividualFirstName ?? "";
                                    model.MiddleName = trustDetails.TrusteeIndividualMiddleName ?? "";
                                    model.LastName = trustDetails.TrusteeIndividualLastName ?? "";
                                    model.DobMonth = Convert.ToDateTime(trustDetails.TrusteeIndividualBirthDate).Month.ToString("D2");
                                    model.DobDay = Convert.ToDateTime(trustDetails.TrusteeIndividualBirthDate).Day.ToString("D2");
                                    model.DobYear = Convert.ToDateTime(trustDetails.TrusteeIndividualBirthDate).Year;
                                    model.Gender = trustDetails.TrusteeIndividualGender != null ? (trustDetails.TrusteeIndividualGender == true ? "Male" : "Female") : "";
                                    model.Citizenship = trustDetails.FK_TrusteeIndividualCitizenshipID != null ? countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeIndividualCitizenshipID) : "";
                                    model.IdInfo = trustDetails.FK_TrusteeIndividualIDTypeId != null ? idInfoTypeBO.GetSelectedIDInformation((int)trustDetails.FK_TrusteeIndividualIDTypeId) : "";
                                    model.IdNumber = trustDetails.TrusteeIndividualIDNumber != null ? trustDetails.TrusteeIndividualIDNumber : "";
                                    model.ResidenceCountry = countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeIndividualResidenceCountryID);
                                    model.PhoneID = trustDetails.PhoneID ?? "";

                                    model.ResidentialAddLine1 = trustDetails.TrusteeIndividualResidentialAddress != null ? (trustDetails.TrusteeIndividualResidentialAddress.Split('@')[0] + " " + trustDetails.TrusteeIndividualResidentialAddress.Split('@')[1]) : "";
                                    model.ResidentialCity = trustDetails.TrusteeIndividualResidentialCity ?? "";
                                    model.ResidentialCountry = trustDetails.FK_TrusteeIndividualResidentialCountryID != null ? countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeIndividualResidentialCountryID) : "";
                                    model.ResidentialPostalCode = trustDetails.TrusteeIndividualResidentialPostalCode ?? "";
                                    model.PreviousAddLine1 = trustDetails.TrusteeIndividualPreviousAddress != null ? (trustDetails.TrusteeIndividualPreviousAddress.Split('@')[0] + " " + trustDetails.TrusteeIndividualPreviousAddress.Split('@')[1]) : "";
                                    model.PreviousCity = trustDetails.TrusteeIndividualPreviousCity ?? "";
                                    model.PreviousCountry = trustDetails.FK_TrusteeIndividualPreviousCountryID != null ? countryBO.GetSelectedCountry((int)trustDetails.FK_TrusteeIndividualPreviousCountryID) : "";
                                    model.PreviousPostalCode = trustDetails.TrusteeIndividualPreviousPostalCode ?? "";
                                    model.TelNumberCountryCode = Convert.ToInt64(trustDetails.TrusteeIndividualTelephoneNumber.Split('-')[0]);
                                    model.TelNumber = Convert.ToInt64(trustDetails.TrusteeIndividualTelephoneNumber.Split('-')[1]);
                                    model.MobileNumberCountryCode = Convert.ToInt64(trustDetails.TrusteeIndividualMobileNumber.Split('-')[0]);
                                    model.MobileNumber = Convert.ToInt64(trustDetails.TrusteeIndividualMobileNumber.Split('-')[1]);
                                }
                                model.EmailAddress = userInformation.UserEmailID ?? "";
                                model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[ruleInfo.AccountNumberPosition - 1] : "";
                                if (bankInformationList != null)
                                {
                                    foreach (var item in bankInformationList)
                                    {
                                        var bankAccountModel = new BankAccountModel()
                                        {
                                            BankAccountID = item.PK_BankAccountInformationID,
                                            BankName = item.BankName ?? "",
                                            AccountNumber = item.AccountNumber ?? "",
                                            BicOrSwiftCode = item.BicNumberOrSwiftCode ?? "",
                                            ReceivingBankInfoId = item.FK_ReceivingBankInformationID != null ? receivingBankInfoBO.GetSelectedRecievingBankInfo((int)item.FK_ReceivingBankInformationID) : "",
                                            ReceivingBankInfo = item.ReceivingBankInfo ?? "",
                                            BankAddLine1 = item.BankingAddress != null ? item.BankingAddress.Split('@')[0] : "",
                                            BankAddLine2 = item.BankingAddress != null ? item.BankingAddress.Split('@')[1] : "",
                                            BankCity = item.City ?? "",
                                            BankCountry = item.FK_CountryID != null ? countryBO.GetSelectedCountry((int)item.FK_CountryID) : "",
                                            BankPostalCode = item.PostalCode ?? ""
                                        };

                                        bankList.Add(bankAccountModel);
                                    }
                                    model.BankAccountModelList = bankList;
                                }
                            }
                            #endregion
                        }
                    }
                    
                    return View("ClientDetails", model);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns ClientAccounts view with
        /// client data passed as model
        /// </summary>
        /// <returns></returns>
        public ActionResult ClientAccounts(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_LIVE, organizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");
                    ViewData["AccountCode"] = new SelectList(accountCodeBO.GetSelectedAccount(Constants.K_BROKER_LIVE), "PK_AccountID", "AccountName");
                    ViewData["TradingPlatform"] = new SelectList(tradingPlatformBO.GetSelectedPlatform(Constants.K_BROKER_LIVE, organizationID), "PK_TradingPlatformID", "L_TradingPlatformValues.TradingValue");

                    string[] currencyIds = clientAccBo.GetDifferentCurrencyAccountOfClientOnClientPK(clientID).TrimEnd('/').Split('/');

                    var clientModel = new ClientAccountsModel();
                    clientModel.CurrencyAccountDetails = new List<MyAccountCurrencyModel>();

                    clientModel.ClientID = clientID;
                    clientModel.AccountID = accountID;
                    clientModel.ClientName = clientName;

                    foreach (var curr in currencyIds)
                    {
                        var model = new MyAccountCurrencyModel();
                        var landingAccDetails = clientAccBo.GetLandingAccountForCurrencyOfClientOnClientPK(clientID, Convert.ToInt32(curr));
                        model.CurrencyID = curr;
                        model.CurrencyName = lCurrValueBO.GetCurrencySymbolFromID(Convert.ToInt32(curr));
                        model.CurrencyImage = lCurrValueBO.GetCurrencyImageClass(Convert.ToInt32(curr));
                        model.LandingAccount = landingAccDetails.LandingAccount;
                        model.LAccBalance = Utility.FormatCurrencyValue((decimal)landingAccDetails.CurrentBalance, "");
                        clientModel.CurrencyAccountDetails.Add(model);
                    }

                    return View("ClientAccounts", clientModel);
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
        /// This action returns all trading account details for
        /// a particular currency and client
        /// </summary>
        /// <param name="currencyID">currencyID</param>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public ActionResult GetAccountInformtion(string currencyID, int clientID)
        {
            try
            {
                LoginInformation loginInfo = SessionManagement.UserInfo;
                var tradingAccs = clientAccBo.GetAllTradingAccountsForCurrencyOfClient(clientID, Convert.ToInt32(currencyID));
                var feeStructure = partCommBO.GetAllFeeStructureForUser(loginInfo.UserID);

                var tradingAccList = new List<CurrencyAccountModel>();

                foreach (var acc in tradingAccs)
                {
                    string strFee = "<select onchange='accFeeStructureChange(this, "+acc.PK_ClientAccountID+")' class='chzn-select width-150'><option></option>";
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
                        if (acc.FK_PlatformID == Constants.K_META_TRADER)
                        {
                            accModel.Type = "<img src='/Images/account-metatrader.png' title='MetaTrader 4' alt='MetaTrader 4'>";
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
                        accModel.Type = "<img src='/Images/account-managed.png' title='Managed Account' alt='Managed Account'>";
                    }

                    accModel.Balance = Utility.FormatCurrencyValue((decimal)acc.CurrentBalance, "");
                    accModel.Equity = acc.Equity != null ? Utility.FormatCurrencyValue((decimal)acc.Equity, "") : "NA";

                    accModel.Floating = "10,000.00";
                    accModel.Change = "1.42";

                    //Fee Structure select list
                    foreach (var fee in feeStructure)
                    {
                        //Make applied fee structure selected in dropdown
                        if (fee.PK_PartnerCommID == acc.FK_FeeStructureID)
                        {
                            strFee += "<option value='" + fee.PK_PartnerCommID + "' selected=''>" + fee.FeeStructureName + "</option>";
                        }
                        else
                        {
                            strFee += "<option value='" + fee.PK_PartnerCommID + "'>" + fee.FeeStructureName + "</option>";
                        }
                    }
                    strFee += "</select>";
                    accModel.FeeStructure = strFee;

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
        /// This action returns CreateNewLanding
        /// view with necessary data for display
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateNewLandingAccount(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_LIVE, (int)SessionManagement.OrganizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");
                    var model = new ClientAccountsModel();
                    model.AccountID = accountID;
                    model.ClientID = clientID;
                    model.ClientName = clientName;

                    return View("CreateNewLanding", model);
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
        /// This action gets account details and pass it in ClientAccountDetails
        /// view as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountID">accountID</param>
        /// <param name="accountNumber">accountNumber</param>
        /// <returns></returns>
        public ActionResult ClientAccountDetails(int clientID, string accountNumber, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var organizationID = (int) SessionManagement.OrganizationID;

                    var model = new ClientAccountDetailsModel();
                    model.TransferLogDetails = new List<TransferLogDetails>();
                    model.ClientID = clientID;
                    model.ClientName = clientName;

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

                    return View("ClientAccountDetails", model);
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
        /// This action returns list of account list of
        /// clients under an IB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAccountList()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var lstAccList = new List<IBClientsModel>();
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var clientsOfIB = clientBO.GetAllClientsOfIB(loginInfo.UserID);

                    foreach (var client in clientsOfIB)
                    {
                        var clientAccounts = clientAccBo.GetAllAccountsOfClientOnClientPK(client.PK_ClientID);
                        dynamic clientDetails = null;

                        //Get account type details
                        var accountTypeDetails = accountTypeBO.GetAccountTypeAndFormTypeValue((int)client.FK_AccountTypeID);

                        //Get Individual Acc Info
                        if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
                        {
                            clientDetails = indAccInfoBO.GetIndividualAccountDetails(client.PK_ClientID);
                        }
                        //Get Joint Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
                        {
                            clientDetails = jointAccInfoBO.GetJointAccountDetails(client.PK_ClientID);
                        }
                        //Get Corporate Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
                        {
                            clientDetails = corpAccInfoBO.GetCorporateAccountDetails(client.PK_ClientID);
                        }
                        //Get Trust Acc Info
                        else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_TRUST)
                        {
                            clientDetails = trustAccInfoBO.GetTrustAccountDetails(client.PK_ClientID);
                        }

                        foreach (var acc in clientAccounts)
                        {
                            var model = new IBClientsModel();
                            
                            //Set Acc Type image
                            if (acc.IsLandingAccount == true)
                            {
                                model.Type = "<img src='/Images/account-landing.png' title='Landing Account' alt='Landing Account'>";
                            }
                            else if (acc.IsTradingAccount == true)
                            {
                                if (acc.FK_PlatformID == Constants.K_META_TRADER)
                                {
                                    model.Type = "<img src='/Images/account-metatrader.png' title='MetaTrader 4' alt='MetaTrader 4'>";
                                }
                            }
                            else
                            {
                                model.Type = "<img src='/Images/account-managed.png' title='Managed Account' alt='Managed Account'>";
                            }

                            //Individual
                            if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
                            {
                                if (clientDetails != null)
                                {
                                    model.PK_ClientID = client.PK_ClientID;
                                    model.AccountID = acc.IsLandingAccount == true ? acc.LandingAccount : acc.TradingAccount;
                                    model.FirstName = clientDetails.FirstName;
                                    model.LastName = clientDetails.LastName;
                                    model.CompanyName = "N/A";
                                    lstAccList.Add(model);
                                }
                            }
                            //Joint
                            else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
                            {
                                if (clientDetails != null)
                                {
                                    model.PK_ClientID = client.PK_ClientID;
                                    model.AccountID = acc.IsLandingAccount == true ? acc.LandingAccount : acc.TradingAccount;
                                    model.FirstName = clientDetails.PrimaryAccountHolderFirstName;
                                    model.LastName = clientDetails.PrimaryAccountHolderLastName;
                                    model.CompanyName = "N/A";
                                    lstAccList.Add(model);
                                }
                            }
                            //Corporate
                            else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
                            {
                                if (clientDetails != null)
                                {
                                    model.PK_ClientID = client.PK_ClientID;
                                    model.AccountID = acc.IsLandingAccount == true ? acc.LandingAccount : acc.TradingAccount;
                                    model.FirstName = clientDetails.AuthOfficerFirstName;
                                    model.LastName = clientDetails.AuthOfficerLastName;
                                    model.CompanyName = clientDetails.CompanyName;
                                    lstAccList.Add(model);
                                }
                            }
                            //Trust
                            else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_TRUST)
                            {
                                if (clientDetails != null)
                                {
                                    model.PK_ClientID = client.PK_ClientID;
                                    model.AccountID = acc.IsLandingAccount == true ? acc.LandingAccount : acc.TradingAccount;
                                    if (clientDetails.FK_TrusteeTypeID == 1)
                                    {
                                        model.FirstName = clientDetails.TrusteeAuthOfficerFirstName;
                                        model.LastName = clientDetails.TrusteeAuthOfficerLastName;
                                        model.CompanyName = clientDetails.TrusteeCompanyName;
                                    }
                                    else if (clientDetails.FK_TrusteeTypeID == 2)
                                    {
                                        model.FirstName = clientDetails.TrusteeIndividualFirstName;
                                        model.LastName = clientDetails.TrusteeIndividualLastName;
                                        model.CompanyName = "N/A";
                                    }
                                    lstAccList.Add(model);
                                }
                            }
                        }
                    }
                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstAccList.Count,
                        rows = lstAccList
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
        /// This action returns DepositClientFund view with
        /// required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult DepositClientFund(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");
                    var model = new TransfersModel();
                    model.BankInformation = new List<BankInformation>();
                    model.LandingAccInformation = new List<LandingAccInformation>();

                    //Get client UserID
                    var clientUserID = (int)clientBO.GetClientInformationOnClientPK(clientID).FK_UserID;

                    //Get all bank accounts
                    var userBankInfos = bankBO.GetAllBankInfosForUser(LoginAccountType.LiveAccount, clientUserID);
                    foreach (var bank in userBankInfos)
                    {
                        var bankInfo = new BankInformation();
                        bankInfo.BankName = bank.BankName;
                        bankInfo.BankAccNumber = bank.AccountNumber;
                        model.BankInformation.Add(bankInfo);
                    }

                    //Get all landing accounts
                    var landingAccs = clientAccBo.GetAllLandingAccountForUser(LoginAccountType.LiveAccount, clientUserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;
                        lAccInfo.LAccCustomName = lAcc.AccountName;

                        lAccInfo.LAccBalance = Utility.FormatCurrencyValue((decimal)lAcc.CurrentBalance, "");

                        model.LandingAccInformation.Add(lAccInfo);
                    }
                    
                    model.AccountID = accountID;
                    model.ClientID = clientID;
                    model.ClientName = clientName;

                    return View("DepositClientFund", model);
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
        /// This action returns WithdrawClientFund view
        /// with required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult WithdrawClientFund(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    AccountNumberRuleInfo ruleInfo = SessionManagement.AccountRuleInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    var model = new TransfersModel();
                    model.BankInformation = new List<BankInformation>();
                    model.LandingAccInformation = new List<LandingAccInformation>();

                    //Get client UserID
                    var clientUserID = (int)clientBO.GetClientInformationOnClientPK(clientID).FK_UserID;

                    //Get all bank accounts
                    var userBankInfos = bankBO.GetAllBankInfosForUser(LoginAccountType.LiveAccount, clientUserID);
                    foreach (var bank in userBankInfos)
                    {
                        var bankInfo = new BankInformation();
                        bankInfo.BankName = bank.BankName;
                        bankInfo.BankAccNumber = bank.AccountNumber;
                        model.BankInformation.Add(bankInfo);
                    }

                    //Get all landing accounts
                    var landingAccs = clientAccBo.GetAllLandingAccountForUser(LoginAccountType.LiveAccount, clientUserID);
                    foreach (var lAcc in landingAccs)
                    {
                        var lAccInfo = new LandingAccInformation();
                        lAccInfo.LCurrencyName = lCurrValueBO.GetCurrencySymbolFromCurrencyAccountCode(lAcc.LandingAccount.Split('-')[ruleInfo.CurrencyPosition - 1]);
                        lAccInfo.LAccNumber = lAcc.LandingAccount;
                        lAccInfo.LAccCustomName = lAcc.AccountName;

                        lAccInfo.LAccBalance = Utility.FormatCurrencyValue((decimal)lAcc.CurrentBalance, "");

                        model.LandingAccInformation.Add(lAccInfo);
                    }

                    model.AccountID = accountID;
                    model.ClientID = clientID;
                    model.ClientName = clientName;

                    return View("WithdrawClientFund", model);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns TransferClientFund view
        /// with required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult TransferClientFund(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new TransfersModel();
                    model.TradingAccInformation = new List<TradingAccountGrouped>();

                    //Get client UserID
                    var clientUserID = (int)clientBO.GetClientInformationOnClientPK(clientID).FK_UserID;

                    //Get all trading accounts
                    var tradingAccs = clientAccBo.GetAllTradingAccountsOfUser(LoginAccountType.LiveAccount, clientUserID);
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

                    model.AccountID = accountID;
                    model.ClientID = clientID;
                    model.ClientName = clientName;

                    return View("TransferClientFund", model);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This Function Will Add New BankAccount Information for client
        /// </summary>
        /// <param name="bankAccountModel">bankAccountModel</param>
        /// <returns>return ActionResult</returns>
        public ActionResult AddNewBankAccount(BankAccountModel bankAccountModel)
        {
            try
            {
                var bankAccountInformtionBO = new BankAccountInformationBO();
                var bankAccountInformtion = new BankAccountInformation()
                {
                    BankName = bankAccountModel.BankName,
                    AccountNumber = bankAccountModel.AccountNumber,
                    BicNumberOrSwiftCode = bankAccountModel.BicOrSwiftCode,
                    BankingAddress = bankAccountModel.BankAddLine1 + '@' + bankAccountModel.BankAddLine2,
                    FK_ReceivingBankInformationID = Convert.ToInt32(bankAccountModel.ReceivingBankInfoId),
                    ReceivingBankInfo = bankAccountModel.ReceivingBankInfo,
                    City = bankAccountModel.BankCity,
                    FK_CountryID = Convert.ToInt32(bankAccountModel.BankCountry),
                    PostalCode = bankAccountModel.BankPostalCode
                };

                if (SessionManagement.UserInfo != null)
                {
                    //Get client UserID
                    var clientInfo = clientBO.GetClientInformationOnClientPK(bankAccountModel.ClientID);

                    bankAccountInformtionBO.AddNewLiveBankAccountInformation((int)clientInfo.FK_UserID, bankAccountInformtion);
                    return Json(true);

                }
                return Json(false);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action returns ClientDocuments view
        /// with required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountID">accountID</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult ClientDocuments(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new ClientDocumentModel();
                    model.ClientID = clientID;
                    model.AccountID = accountID;
                    model.ClientName = clientName;

                    return View("ClientDocuments", model);
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
        /// This action returns list of document details related to user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDocumentDetails(int clientID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var lstDocument = new List<ClientDocumentModel>();

                    //Get client UserID
                    var clientInfo = clientBO.GetClientInformationOnClientPK(clientID);

                    //Get docs required for client account type
                    var reqDocs = r_UserDocumentBO.GetAllDocumentsOfAccountType((int)clientInfo.FK_AccountTypeID);

                    //Iterate through each doc type
                    foreach (var doc in reqDocs)
                    {
                        var document = new ClientDocumentModel();
                        document.DocumentName = doc.Document.DocumentName;
                        document.DocumentID = (int)doc.FK_DocumentID;

                        //Get client document details if exists from db
                        var docDetails = userDocumentBO.GetUserDocumentDetails((int)clientInfo.FK_UserID, (int)doc.FK_DocumentID);
                        if (docDetails != null)
                        {
                            document.Status = docDetails.Status;
                        }
                        else
                        {
                            document.Status = "Missing Documents";
                        }
                        lstDocument.Add(document);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstDocument.Count,
                        rows = lstDocument
                    }, JsonRequestBehavior.AllowGet);
                }
                
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action method make entry in database for 
        /// uploaded doc for client and save file in file system
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="docID">docID</param>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        [HttpPost]
        public string UploadClientDocument(HttpPostedFileBase file, int docID, int clientID)
        {
            var javascriptSerailizer = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                if (file.ContentLength > 0)
                {
                    if (SessionManagement.UserInfo != null)
                    {
                        //Get client UserID
                        var clientInfo = clientBO.GetClientInformationOnClientPK(clientID);

                        //Make entry in database for uploaded document
                        if (userDocumentBO.UploadDocument((int)clientInfo.FK_UserID, docID, file.FileName))
                        {
                            //Create file name using userID and docID plus extension
                            var fileName = clientInfo.FK_UserID + "-" + docID + file.FileName.Substring(file.FileName.LastIndexOf('.'));

                            //Specify the path for saving
                            var path = Path.Combine(Server.MapPath("~/UserDocuments"), fileName);

                            //Save the file
                            file.SaveAs(path);

                            //Update user status
                            UpdateClientStatus((int)clientInfo.FK_UserID, (int)clientInfo.FK_AccountTypeID);

                            return javascriptSerailizer.Serialize(true);
                        }
                        else
                        {
                            return javascriptSerailizer.Serialize(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
            return javascriptSerailizer.Serialize(false);
        }

        /// <summary>
        /// This method sends document to be
        /// downloaded as attachment for saving in browser
        /// </summary>
        /// <param name="docID">docID</param>
        /// <param name="clientID">clientID</param>
        public void DownloadClientDocument(int docID, int clientID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get client UserID
                    var clientInfo = clientBO.GetClientInformationOnClientPK(clientID);

                    //Get file name from database
                    var fileName = userDocumentBO.GetUploadedDocumentName((int)clientInfo.FK_UserID, docID);

                    if (fileName != String.Empty)
                    {
                        //Get file extension
                        var fileExt = fileName.Substring(fileName.LastIndexOf('.'));

                        var file = new FileInfo(Server.MapPath("~/UserDocuments/" + clientInfo.FK_UserID + "-" + docID + fileExt));

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.AppendHeader("Content-Disposition", "attachment; filename = " + fileName);
                        Response.AppendHeader("Content-Length", file.Length.ToString());
                        Response.ContentType = GetContentType(file.Name);
                        Response.WriteFile(file.FullName);
                        Response.Flush();
                        Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method returns extension of a file
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns></returns>
        private string GetContentType(string fileName)
        {

            var contentType = "application/octetstream";

            var ext = Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                contentType = registryKey.GetValue("Content Type").ToString();
            }

            return contentType;
        }

        /// <summary>
        /// This action method deletes client document from
        /// file system and makes IsDeleted = true entry in db
        /// </summary>
        /// <param name="docID">docID</param>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public ActionResult ClearClientDocument(int docID, int clientID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get client UserID
                    var clientInfo = clientBO.GetClientInformationOnClientPK(clientID);

                    var fileName = userDocumentBO.ClearUserDocument((int)clientInfo.FK_UserID, docID);
                    var fileExt = fileName.Substring(fileName.LastIndexOf('.'));

                    if (fileName != String.Empty)
                    {
                        //Delete document file from file system
                        System.IO.File.Delete(Directory.EnumerateFileSystemEntries(System.Web.HttpContext.Current.Server.MapPath("~/UserDocuments"), clientInfo.FK_UserID + "-" + docID + fileExt).First());

                        //Update user status
                        UpdateClientStatus((int)clientInfo.FK_UserID, (int)clientInfo.FK_AccountTypeID);

                        return Json(true);
                    }
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action method checks client document
        /// status and sets client status
        /// </summary>
        /// <param name="userID">userID</param>
        /// <param name="accountTypeID">accountTypeID</param>
        public void UpdateClientStatus(int userID, int accountTypeID)
        {
            try
            {
                //Get total doc count for account type
                var docCount = r_UserDocumentBO.GetAllDocumentsOfAccountType(accountTypeID).Count();

                //Get all docs of client
                var userDocs = userDocumentBO.GetAllUserDocuments(userID);

                //Check and update client status
                if (userDocs.Count < docCount)
                {
                    clientBO.UpdateClientStatus(userID, "Missing Documents");
                }
                else
                {
                    if (userDocs.Any(doc => doc.Status == "Missing Documents" || doc.Status == "Denied"))
                    {
                        clientBO.UpdateClientStatus(userID, "Missing Documents");
                    }
                    else if (userDocs.Any(doc => doc.Status == "Pending"))
                    {
                        clientBO.UpdateClientStatus(userID, "Pending");
                    }
                    else if (userDocs.Any(doc => doc.Status == "Approved"))
                    {
                        clientBO.UpdateClientStatus(userID, "Approved");
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action returns ClientNoteHistory view with
        /// required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountID">accountID</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult ClientNoteHistory(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new ClientNoteHistoryModel();
                    model.ClientID = clientID;
                    model.AccountID = accountID;
                    model.ClientName = clientName;
                    model.PartnerDisplayName = SessionManagement.DisplayName;
                    
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
        /// This action inserts new client note in ClientNotes table for IB
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="subject">subject</param>
        /// <param name="note">note</param>
        /// <returns></returns>
        public ActionResult AddNewClientNote(int clientID, string subject, string note)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    
                    return Json(clientNotesBO.AddNewClientNote(clientID, loginInfo.UserID, subject, note));
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
        /// This action returns list of client notes
        /// for a particular client
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public ActionResult GetAllNotesOfClient(int clientID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var lstClientNotes = new List<ClientNoteDetails>();

                    //Get all notes of this client
                    var clientNotes = clientNotesBO.GetAllNotesOfClient(clientID, loginInfo.UserID);

                    foreach (var note in clientNotes)
                    {
                        var clntNote = new ClientNoteDetails();
                        clntNote.Subject = note.Subject;
                        clntNote.Note = note.Note;
                        clntNote.Timestamp = Convert.ToDateTime(note.Timestamp).ToString("dd/MM/yyyy hh:mm:ss tt");
                        clntNote.TimestampDay = "";
                        clntNote.TimestampLong = Convert.ToDateTime(note.Timestamp).ToLongDateString() + " " + Convert.ToDateTime(note.Timestamp).ToLongTimeString();

                        lstClientNotes.Add(clntNote);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstClientNotes.Count,
                        rows = lstClientNotes
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
        /// This action change or assign agent to client
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="agentID">agentID</param>
        /// <returns></returns>
        public ActionResult ChangeClientAgent(int clientID, int agentID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(clientBO.ChangeClientAgent(clientID, agentID));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(true);
            }
        }

        /// <summary>
        /// This action returns ClientActivity view with
        /// required data passed as model
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="accountID">accountID</param>
        /// <param name="clientName">clientName</param>
        /// <returns></returns>
        public ActionResult ClientActivity(int clientID, string accountID, string clientName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new ClientActivityModel();
                    model.ClientID = clientID;
                    model.AccountID = accountID;
                    model.ClientName = clientName;

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Action", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action updates fee structure of client account
        /// </summary>
        /// <param name="feeValue">feeValue</param>
        /// <param name="pkClientAccID">pkClientAccID</param>
        /// <returns></returns>
        public ActionResult UpdateClientAccFeeStructure(int feeValue, int pkClientAccID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(clientAccBo.UpdateClientAccFeeStructure(feeValue, pkClientAccID));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This actions returns list of client activities
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public ActionResult GetClientActivityDetails(int clientID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var lstClientActivity = new List<ClientActivity>();

                    //Get client userID
                    var clientUserID = (int)clientBO.GetClientInformationOnClientPK(clientID).FK_UserID;
                    
                    //Get client activities
                    var clientActivities = usrActivityBO.GetUserRecentActivityDetails(clientUserID);

                    foreach (var act in clientActivities)
                    {
                        var clntAct = new ClientActivity();
                        clntAct.ActivityTimestamp = Convert.ToDateTime(act.Timestamp).ToString("dd/MM/yyyy HH:mm:ss tt");
                        clntAct.ActivityType = act.L_ActivityType.ActivityTypeValue;

                        //Profile Activities
                        if (act.FK_ActivityTypeID == (int)ActivityType.ProfileActivity)
                        {
                            clntAct.ActivityDetails = act.ProfileActivities.FirstOrDefault().ProfileActivityDetails;
                        }
                        //Document activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.DocumentActivity)
                        {
                            clntAct.ActivityDetails = "<a href=Document>" + act.DocumentActivities.FirstOrDefault().Document.DocumentName + "</a>" + " document status has changed to <i>" + act.DocumentActivities.FirstOrDefault().DocumentStatus + "</i>.";
                        }
                        //Account activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.AccountActivity)
                        {
                            //New acc creation activity
                            if (act.AccountActivities.FirstOrDefault().FK_AccActivityTypeID == (int)AccountActivityType.NewAccountCreation)
                            {
                                clntAct.ActivityDetails = "A new " + act.AccountActivities.FirstOrDefault().L_CurrencyValue.CurrencyValue + " " + act.AccountActivities.FirstOrDefault().AccountType + " account has been created.";
                            }
                        }
                        //Transfer activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.TransferActivity)
                        {
                            clntAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.TransferActivities.FirstOrDefault().TransferAmount, "") + " " + act.TransferActivities.FirstOrDefault().L_CurrencyValue.CurrencyValue + "</b> has been transferred from account <a href='MyAccount/ShowAccountDetails?accountNumber=" + act.TransferActivities.FirstOrDefault().FromAccount + "'>" + act.TransferActivities.FirstOrDefault().FromAccount + "</a> to <a href='MyAccount/ShowAccountDetails?accountNumber=" + act.TransferActivities.FirstOrDefault().ToAccount + "'>" + act.TransferActivities.FirstOrDefault().ToAccount + "</a>.";
                        }
                        //Conversion activities
                        else if (act.FK_ActivityTypeID == (int)ActivityType.ConversionActivity)
                        {
                            clntAct.ActivityDetails = "<b>" + Utility.FormatCurrencyValue((decimal)act.ConversionActivities.FirstOrDefault().ConversionAmount, "") + " " + act.ConversionActivities.FirstOrDefault().L_CurrencyValue.CurrencyValue + "</b>  has been converted from <a href='MyAccount/ShowAccountDetails?accountNumber=" + act.ConversionActivities.FirstOrDefault().FromAccount + "'>" + act.ConversionActivities.FirstOrDefault().FromAccount + "</a> at an exchange rate of " + act.ConversionActivities.FirstOrDefault().ExchangeRate + " totaling <b>" + Math.Round(Convert.ToDecimal(act.ConversionActivities.FirstOrDefault().ConversionAmount) * Convert.ToDecimal(act.ConversionActivities.FirstOrDefault().ExchangeRate), 2) + " " + act.ConversionActivities.FirstOrDefault().L_CurrencyValue1.CurrencyValue + "</b> and transferred to <a href='MyAccount/ShowAccountDetails?accountNumber=" + act.ConversionActivities.FirstOrDefault().ToAccount + "'>" + act.ConversionActivities.FirstOrDefault().ToAccount + "</a>.";
                        }

                        lstClientActivity.Add(clntAct);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstClientActivity.Count,
                        rows = lstClientActivity
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
    }
}
