#region Header Information
/*****************************************************************************
 * File Name     : ProfileController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 19th April 2013
 * Modified Date : 19th April 2013
 * Description   : This file contains action methods of Profile controller of AM
 * ***************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models;
using CurrentDesk.Models;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using CurrentDesk.BackOffice.Models.Edit;
using CurrentDesk.BackOffice.Areas.AssetManager.Models.Profile;
using CurrentDesk.BackOffice.Extension;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Areas.AssetManager.Controllers
{
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_AM), NoCache]
    public class ProfileController : Controller
    {
        #region Variables
        private IntroducingBrokerBO introducingBrokerBO = new IntroducingBrokerBO();
        private L_CountryBO countryBO = new L_CountryBO();
        private L_IDInformationTypeBO idInfoTypeBO = new L_IDInformationTypeBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private Client_AccountBO clientAccBO = new Client_AccountBO();
        private UserImageBO userImgBO = new UserImageBO();
        private L_WidenSpreadsValueBO widenSpreadValuesBO = new L_WidenSpreadsValueBO();
        private L_CommissionIncrementValueBO commIncValueBO = new L_CommissionIncrementValueBO();
        private AccountCurrencyBO accountCurrencyBO = new AccountCurrencyBO();
        private PartnerCommissionBO partCommBO = new PartnerCommissionBO();
        private UserBO userBO = new UserBO();
        private L_CompanyTypeValueBO companyTypeValuesBO = new L_CompanyTypeValueBO();
        private TradingPlatformBO tradingPlatformBO = new TradingPlatformBO();
        private ManagedAccountProgramBO manAccPrgmBO = new ManagedAccountProgramBO();
        private AccountCurrencyBO accCurrBO = new AccountCurrencyBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private ProfileActivityBO profileActivityBO = new ProfileActivityBO();
        private AccountTypeBO accountTypeBO = new AccountTypeBO();
        #endregion

        /// <summary>
        /// This class represents controller for Profile page of AM and contains
        /// actions to handle required functionality
        /// </summary>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;
                    var accountType = loginInfo.AccountType;

                    //Get account type details
                    var accountTypeDetails = accountTypeBO.GetAccountTypeAndFormTypeValue(accountType);

                    Session["AccountTypeValue"] = accountTypeDetails.FK_AccountTypeValue;

                    if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
                    {
                        return RedirectToAction("PersonalInformation");
                    }
                    else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
                    {
                        return RedirectToAction("PrimaryHolderInformation");
                    }
                    else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
                    {
                        return RedirectToAction("CompanyInformation");
                    }
                    else
                    {
                        return View("");
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
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action gets AM individual personal
        /// details and returns view with model
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalInformation()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Individual Account details for the partner user
                    var selectedClient = introducingBrokerBO.GetIndividualAccountDetails(loginInfo.UserID);
                    var userInformation = selectedClient.User;
                    var clientInformations = selectedClient.IndividualAccountInformations.FirstOrDefault();
                    var clientAccInfo = selectedClient.Client_Account.FirstOrDefault();

                    //Assigning properties to IndividualAccountReviewModel object
                    var model = new IndividualAccountReviewModel();
                    model.Title = clientInformations.Title != null ? (clientInformations.Title == "1" ? "Mr." : "Mrs.") : "";
                    model.FirstName = clientInformations.FirstName ?? "";
                    model.MiddleName = clientInformations.MiddleName ?? "";
                    model.LastName = clientInformations.LastName ?? "";
                    model.DobMonth = Convert.ToDateTime(clientInformations.BirthDate).Month.ToString("D2");
                    model.DobDay = Convert.ToDateTime(clientInformations.BirthDate).Day.ToString("D2");
                    model.DobYear = Convert.ToDateTime(clientInformations.BirthDate).Year;
                    model.Gender = clientInformations.Gender != null ? (clientInformations.Gender == true ? "Male" : "Female") : "";
                    model.Citizenship = clientInformations.FK_CitizenShipCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_CitizenShipCountryID) : "";
                    model.IdInfo = clientInformations.FK_IDInformationID != null ? idInfoTypeBO.GetSelectedIDInformation((int)clientInformations.FK_IDInformationID) : "";
                    model.IdNumber = clientInformations.IDNumber != null ? clientInformations.IDNumber : "";
                    model.ResidenceCountry = countryBO.GetSelectedCountry((int)clientInformations.FK_ResidenceCountryID);
                    model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[2] : "";
                    model.PhoneID = clientInformations.PhoneID ?? "";

                    model.ResidentialAddLine1 = clientInformations.ResidentialAddress != null ? (clientInformations.ResidentialAddress.Split('@')[0] + " " + clientInformations.ResidentialAddress.Split('@')[1]) : "";
                    model.ResidentialCity = clientInformations.ResidentialAddressCity ?? "";
                    model.ResidentialCountry = clientInformations.FK_ResidentialAddressCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_ResidentialAddressCountryID) : "";
                    model.ResidentialPostalCode = clientInformations.ResidentialAddressPostalCode ?? "";
                    model.YearsInCurrentAdd = clientInformations.MonthsAtCurrentAddress != null ? (int)(clientInformations.MonthsAtCurrentAddress / 12) : 0;
                    model.MonthsInCurrentAdd = clientInformations.MonthsAtCurrentAddress != null ? (int)(clientInformations.MonthsAtCurrentAddress % 12) : 0;
                    model.PreviousAddLine1 = clientInformations.PreviousAddress != null ? (clientInformations.PreviousAddress.Split('@')[0] + " " + clientInformations.PreviousAddress.Split('@')[1]) : "";
                    model.PreviousCity = clientInformations.PreviousAddressCity ?? "";
                    model.PreviousCountry = clientInformations.FK_PreviousAddressCounrtyID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_PreviousAddressCounrtyID) : "";
                    model.PreviousPostalCode = clientInformations.PreviousAddressPostalCode ?? "";
                    model.TelNumberCountryCode = string.IsNullOrWhiteSpace(clientInformations.TelephoneNumber.Split('-')[0] + clientInformations.TelephoneNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[0]);
                    model.TelNumber = string.IsNullOrWhiteSpace(clientInformations.TelephoneNumber.Split('-')[0] + clientInformations.TelephoneNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[1]);
                    model.MobileNumberCountryCode = string.IsNullOrWhiteSpace(clientInformations.MobileNumber.Split('-')[0] + clientInformations.MobileNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.MobileNumber.Split('-')[0]);
                    model.MobileNumber = string.IsNullOrWhiteSpace(clientInformations.MobileNumber.Split('-')[0] + clientInformations.MobileNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.MobileNumber.Split('-')[1]);
                    model.EmailAddress = userInformation.UserEmailID ?? "";


                    //Return IndividualProfile view with above model
                    return View("PersonalInformation", model);
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
        /// This action gets partner primary profile
        /// details and sends them to view
        /// </summary>
        /// <returns></returns>
        public ActionResult PrimaryHolderInformation()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Joint Account details for the partner user
                    var selectedClient = introducingBrokerBO.GetJointAccountDetails(loginInfo.UserID);
                    var userInformation = selectedClient.User;
                    var clientInformations = selectedClient.JointAccountInformations.FirstOrDefault();
                    var clientAccInfo = selectedClient.Client_Account.FirstOrDefault();

                    //Assigning properties to JointAccountReviewModel object
                    var model = new JointAccountReviewModel();
                    model.PrimaryAccountHolderTitle = clientInformations.PrimaryAccountHolderTitle != null ? (clientInformations.PrimaryAccountHolderTitle == "1" ? "Mr." : "Mrs.") : "";
                    model.PrimaryAccountHolderFirstName = clientInformations.PrimaryAccountHolderFirstName ?? "";
                    model.PrimaryAccountHolderMiddleName = clientInformations.PrimaryAccountHolderMiddleName ?? "";
                    model.PrimaryAccountHolderLastName = clientInformations.PrimaryAccountHolderLastName ?? "";
                    model.PrimaryAccountHolderDobMonth = Convert.ToDateTime(clientInformations.PrimaryAccountHolderBirthDate).Month.ToString("D2");
                    model.PrimaryAccountHolderDobDay = Convert.ToDateTime(clientInformations.PrimaryAccountHolderBirthDate).Day.ToString("D2");
                    model.PrimaryAccountHolderDobYear = Convert.ToDateTime(clientInformations.PrimaryAccountHolderBirthDate).Year;
                    model.PrimaryAccountHolderGender = clientInformations.PrimaryAccountHolderGender != null ? (clientInformations.PrimaryAccountHolderGender == true ? "Male" : "Female") : "";
                    model.PrimaryAccountHolderCitizenship = clientInformations.FK_PrimaryAccountHolderCitizenshipCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_PrimaryAccountHolderCitizenshipCountryID) : "";
                    model.PrimaryAccountHolderIdInfo = clientInformations.FK_PrimaryAccountHolderIDTypeID != null ? idInfoTypeBO.GetSelectedIDInformation((int)clientInformations.FK_PrimaryAccountHolderIDTypeID) : "";
                    model.PrimaryAccountHolderIdNumber = clientInformations.PrimaryAccountHolderIDNumber ?? "";
                    model.PrimaryAccountHolderResidenceCountry = clientInformations.FK_PrimaryAccountHolderResidenceCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_PrimaryAccountHolderResidenceCountryID) : "";
                    model.ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[2] : "";
                    model.PhoneID = clientInformations.PhoneID ?? "";

                    model.PrimaryAccountHolderResidentialAddLine1 = clientInformations.ResidentialAddress != null ? (clientInformations.ResidentialAddress.Split('@')[0] + " " + clientInformations.ResidentialAddress.Split('@')[1]) : "";
                    model.PrimaryAccountHoldeResidentialCity = clientInformations.ResidentialAddressCity ?? "";
                    model.PrimaryAccountHoldeResidentialCountry = clientInformations.FK_PrimaryAccountHolderResidenceCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_PrimaryAccountHolderResidenceCountryID) : "";
                    model.PrimaryAccountHoldeResidentialPostalCode = clientInformations.ResidentialAddressPostalCode ?? "";
                    model.PrimaryAccountHoldeYearsInCurrentAdd = clientInformations.MonthsAtCurrentAddress != null ? (int)(clientInformations.MonthsAtCurrentAddress / 12) : 0;
                    model.PrimaryAccountHoldeMonthsInCurrentAdd = clientInformations.MonthsAtCurrentAddress != null ? (int)(clientInformations.MonthsAtCurrentAddress % 12) : 0;
                    model.PrimaryAccountHoldePreviousAddLine1 = clientInformations.PreviousAddress != null ? (clientInformations.PreviousAddress.Split('@')[0] + " " + clientInformations.PreviousAddress.Split('@')[1]) : "";
                    model.PrimaryAccountHoldePreviousCity = clientInformations.PreviousAddressCity ?? "";
                    model.PrimaryAccountHoldePreviousCountry = clientInformations.FK_PreviousAddressCounrtyID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_PreviousAddressCounrtyID) : "";
                    model.PrimaryAccountHoldePreviousPostalCode = clientInformations.PreviousAddressPostalCode ?? "";
                    model.PrimaryAccountHoldeTelNumberCountryCode = string.IsNullOrWhiteSpace(clientInformations.TelephoneNumber.Split('-')[0] + clientInformations.TelephoneNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[0]);
                    model.PrimaryAccountHoldeTelNumber = string.IsNullOrWhiteSpace(clientInformations.TelephoneNumber.Split('-')[0] + clientInformations.TelephoneNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[1]);
                    model.PrimaryAccountHoldeMobileNumberCountryCode = string.IsNullOrWhiteSpace(clientInformations.MobileNumber.Split('-')[0] + clientInformations.MobileNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.MobileNumber.Split('-')[0]);
                    model.PrimaryAccountHoldeMobileNumber = string.IsNullOrWhiteSpace(clientInformations.MobileNumber.Split('-')[0] + clientInformations.MobileNumber.Split('-')[1]) ? 0 : Convert.ToInt64(clientInformations.MobileNumber.Split('-')[1]);
                    model.PrimaryAccountHoldeEmailAddress = userInformation.UserEmailID ?? "";

                    //Return JointProfile view with above model
                    return View("PrimaryHolder", model);
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
        /// This action gets partner secondary profile
        /// details and sends them to view
        /// </summary>
        /// <returns></returns>
        public ActionResult SecondaryHolderInformation()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Joint Account details for the partner user
                    var selectedClient = introducingBrokerBO.GetJointAccountDetails(loginInfo.UserID);
                    var clientInformations = selectedClient.JointAccountInformations.FirstOrDefault();

                    //Assigning properties to JointAccountReviewModel object
                    var model = new JointAccountReviewModel();

                    model.SecondaryAccountHolderTitle = clientInformations.SecondaryAccountHolderTitle != null ? (clientInformations.SecondaryAccountHolderTitle == "1" ? "Mr." : "Mrs.") : "";
                    model.SecondaryAccountHolderFirstName = clientInformations.SecondaryAccountHolderFirstName ?? "";
                    model.SecondaryAccountHolderMiddleName = clientInformations.SecondaryAccountHolderMiddleName ?? "";
                    model.SecondaryAccountHolderLastName = clientInformations.SecondaryAccountHolderLastName ?? "";
                    model.SecondaryAccountHolderDobMonth = Convert.ToDateTime(clientInformations.SecondaryAccountHolderBirthDate).Month.ToString("D2");
                    model.SecondaryAccountHolderDobDay = Convert.ToDateTime(clientInformations.SecondaryAccountHolderBirthDate).Day.ToString("D2");
                    model.SecondaryAccountHolderDobYear = Convert.ToDateTime(clientInformations.SecondaryAccountHolderBirthDate).Year;
                    model.SecondaryAccountHolderGender = clientInformations.SecondaryAccountHolderGender != null ? (clientInformations.SecondaryAccountHolderGender == true ? "Male" : "Female") : "";
                    model.SecondaryAccountHolderCitizenship = clientInformations.FK_SecondaryAccountHolderCitizenshipCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_SecondaryAccountHolderCitizenshipCountryID) : "";
                    model.SecondaryAccountHolderIdInfo = clientInformations.FK_SecondaryAccountHolderIDTypeID != null ? idInfoTypeBO.GetSelectedIDInformation((int)clientInformations.FK_SecondaryAccountHolderIDTypeID) : "";
                    model.SecondaryAccountHolderIdNumber = clientInformations.SecondaryAccountHolderIDNumber ?? "";
                    model.SecondaryAccountHolderResidenceCountry = clientInformations.FK_SecondaryAccountHolderResidenceCountryID != null ? countryBO.GetSelectedCountry((int)clientInformations.FK_SecondaryAccountHolderResidenceCountryID) : "";

                    //Return JointProfile view with above model
                    return View("SecondaryHolder", model);
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
        /// This action gets AM company info
        /// details and sends them to view
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyInformation()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Corporate Account details for the partner user
                    var selectedResult = introducingBrokerBO.GetCorporateAccountDetails(loginInfo.UserID);
                    var corporateAccountInformation = selectedResult.CorporateAccountInformations.FirstOrDefault();
                    var clientAccInfo = selectedResult.Client_Account.FirstOrDefault();

                    //Assigning properties to CorporateAccountReviewModel object
                    var corporateAccountReviewModel = new CorporateAccountReviewModel()
                    {
                        CompanyName = corporateAccountInformation.CompanyName,
                        CompanyType = companyTypeValuesBO.GetSelectedCompany((int)corporateAccountInformation.FK_CompanyTypeID),
                        CompanyAddLine1 = corporateAccountInformation.CompanyAddress.Split('@')[0] + " " + corporateAccountInformation.CompanyAddress.Split('@')[1],
                        CompanyAddLine2 = "",
                        CompanyCity = corporateAccountInformation.City,
                        CompanyCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_CompanyCountryID),
                        CompanyPostalCode = corporateAccountInformation.CompanyPostalCode,
                        ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[2] : "",
                        PhoneID = corporateAccountInformation.PhoneID ?? "",
                        IsLive = false
                    };

                    //Return CorporateProfile view with above model
                    return View("CompanyInformation", corporateAccountReviewModel);
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
        /// This action gets AM auth officer info
        /// details and sends them to view
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthOfficerInformation()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Corporate Account details for the partner user
                    var selectedResult = introducingBrokerBO.GetCorporateAccountDetails(loginInfo.UserID);
                    var userInformation = selectedResult.User;
                    var corporateAccountInformation = selectedResult.CorporateAccountInformations.FirstOrDefault();

                    //Assigning properties to CorporateAccountReviewModel object
                    var corporateAccountReviewModel = new CorporateAccountReviewModel()
                    {
                        AuthOfficerTitle = corporateAccountInformation.AuthOfficerTitle == "1" ? "Mr" : "Mrs",
                        AuthOfficerFirstName = corporateAccountInformation.AuthOfficerFirstName,
                        AuthOfficerMiddleName = corporateAccountInformation.AuthOfficerMiddleName,
                        AuthOfficerLastName = corporateAccountInformation.AuthOfficerLastName,
                        AuthOfficerDobMonth = ((DateTime)corporateAccountInformation.AuthOfficerBirthDate).Month.ToString("D2"),
                        AuthOfficerDobDay = ((DateTime)corporateAccountInformation.AuthOfficerBirthDate).Day.ToString("D2"),
                        AuthOfficerDobYear = ((DateTime)corporateAccountInformation.AuthOfficerBirthDate).Year,
                        AuthOfficerGender = (bool)corporateAccountInformation.AuthOfficerGender ? "Male" : "Female",
                        AuthOfficerCitizenship = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_AuthOfficerCitizenshipCountryID),
                        AuthOfficerIdInfo = idInfoTypeBO.GetSelectedIDInformation((int)corporateAccountInformation.FK_AuthOfficerInformationTypeID),
                        AuthOfficerIdNumber = corporateAccountInformation.AuthOfficerIDNumber,

                        AuthOfficerResidenceCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_AuthOfficerResidenceCountryID),
                        AuthOfficerAddLine1 = corporateAccountInformation.AuthOfficerAddress.Split('@')[0] + " " + corporateAccountInformation.AuthOfficerAddress.Split('@')[1],
                        AuthOfficerAddLine2 = "",
                        AuthOfficerCity = corporateAccountInformation.AuthOfficerCity,
                        AuthOfficerCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_AuthOfficerCountryID),
                        AuthOfficerPostalCode = corporateAccountInformation.AuthOfficerPostalCode,
                        AuthOfficerTelNumberCountryCode = Convert.ToInt64(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[0]),
                        AuthOfficerTelNumber = Convert.ToInt64(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[1]),
                        AuthOfficerMobileNumberCountryCode = Convert.ToInt64(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[0]),
                        AuthOfficerMobileNumber = Convert.ToInt64(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[1]),
                        AuthOfficerEmailAddress = userInformation.UserEmailID,
                        IsLive = false
                    };

                    //Return CorporateProfile view with above model
                    return View("AuthOfficerInformation", corporateAccountReviewModel);
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
        /// This action gets AM banking information
        /// details and returns view with model
        /// </summary>
        /// <returns></returns>
        public ActionResult Banking()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get Individual Account details for the partner user
                    var selectedClient = introducingBrokerBO.GetIndividualAccountDetails(loginInfo.UserID);
                    var bankInformationList = selectedClient.BankAccountInformations;
                    var bankList = new List<BankAccountModel>();

                    //Assigning properties to IndividualAccountReviewModel object
                    var model = new IndividualAccountReviewModel();

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
                    return View("Banking", model);
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
        /// This action returns Marketing view of AM
        /// </summary>
        /// <returns></returns>
        public ActionResult Marketing()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    ViewData["CustomizedLink"] = introducingBrokerBO.GetCustomizedLinkOfIB(loginInfo.UserID);

                    ViewData["AccountNumber"] = clientAccBO.GetAccountNumberOfUser(loginInfo.LogAccountType, loginInfo.UserID).Split('-')[2];


                    return View("Marketing");
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
        /// This action returns all marketing image details of AM
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllMarketingImageDetails()
        {
            try
            {
                var lstMarketingImg = new List<MarketingModel>();
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get all marketing images from db
                    var imageDetails = userImgBO.GetAllMarketingImagesOfUser(loginInfo.UserID);

                    //Iterate through each image detail and add to list
                    foreach (var img in imageDetails)
                    {
                        var marktImg = new MarketingModel();
                        marktImg.ImageID = img.PK_UserImageID;
                        marktImg.ImageName = img.ImageName;
                        if (img.Status == "Active")
                        {
                            marktImg.Status = "<strong class='green'>" + img.Status + "</strong>";
                            marktImg.Actions = "<a onclick='disableMarketingAdImg(this)' class='icon active' href='#' title='Disable Ad'>active</a><a onclick='deleteMarketingImg(this)' class='icon delete' href='#' title='Delete'>delete</a>";
                        }
                        else if (img.Status == "Rejected")
                        {
                            marktImg.Status = "<strong class='red'>" + img.Status + "</strong>";
                            marktImg.Actions = "<a class='icon disabled' href='#'>active</a><a onclick='deleteMarketingImg(this)' class='icon delete' href='#' title='Delete'>delete</a>";
                        }
                        else if (img.Status == "Approved")
                        {
                            marktImg.Status = "<strong>" + img.Status + "</strong>";
                            marktImg.Actions = "<a onclick='makeMarketingImgActive(this)' class='icon inactive' href='#' title='Make Active'>active</a><a onclick='deleteMarketingImg(this)' class='icon delete' href='#' title='Delete'>delete</a>";
                        }
                        else if (img.Status == "Pending")
                        {
                            marktImg.Status = img.Status;
                            marktImg.Actions = "<a class='icon disabled' href='#'>active</a><a onclick='deleteMarketingImg(this)' class='icon delete' href='#' title='Delete'>delete</a>";
                        }

                        lstMarketingImg.Add(marktImg);
                    }
                }

                return Json(new
                {
                    total = 1,
                    page = 1,
                    records = lstMarketingImg.Count,
                    rows = lstMarketingImg
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action gets AM fee structure
        /// details and returns view with model
        /// </summary>
        /// <returns></returns>
        public ActionResult FeeStructure()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["WideSpreads"] = new SelectList(widenSpreadValuesBO.GetWidenSpreadValues(), "PK_WidenSpreadsID", "WidenSpreadsValue");
                    ViewData["CommSpreads"] = new SelectList(commIncValueBO.GetCommissionIncrementValues(), "PK_CommissionIncrementID", "CommissionIncrementValue");
                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_PARTNER, (int)SessionManagement.OrganizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");

                    return View("FeeStructure");
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
        /// This action returns list of fee structure of AM
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllFeeStructure()
        {
            try
            {
                var lstFeeStructure = new List<FeeStructure>();
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get all fee structures
                    var feeStructureList = partCommBO.GetAllFeeStructureForUser(loginInfo.UserID);

                    //Iterate through each fee structure and add to list
                    foreach (var fee in feeStructureList)
                    {
                        var feeStructure = new FeeStructure();
                        feeStructure.StructureName = fee.FeeStructureName;
                        if (fee.FK_WidenSpreadID == 7)
                        {
                            feeStructure.SpreadMarkUp = String.Format("{0:0.0}", fee.WidenSpreadValue);
                        }
                        else
                        {
                            feeStructure.SpreadMarkUp = String.Format("{0:0.0}", widenSpreadValuesBO.GetWidenSpreadValueFromID((int)fee.FK_WidenSpreadID));
                        }
                        if (fee.FK_CommissionID == 6)
                        {
                            feeStructure.CommissionMarkUp = String.Format("{0:0.0}", fee.CommissionValue);
                        }
                        else
                        {
                            feeStructure.CommissionMarkUp = String.Format("{0:0.0}", commIncValueBO.GetCommIncValueFromID((int)fee.FK_CommissionID));
                        }
                        feeStructure.Currency = accountCurrencyBO.GetCurrencyLookUpValue((int)fee.FK_AccountCurrencyID);
                        if ((bool)fee.IsDefault)
                        {
                            feeStructure.Action = "<a class='icon active' href='#' title='Active Fee'>active</a>";
                        }
                        else
                        {
                            feeStructure.Action = "<a class='icon inactive' href='#' onclick='makeDefaultFeeStructure(" + fee.PK_PartnerCommID + ")' title='Enable Fee'>active</a>";
                        }
                        lstFeeStructure.Add(feeStructure);
                    }
                }

                return Json(new
                {
                    total = 1,
                    page = 1,
                    records = lstFeeStructure.Count(),
                    rows = lstFeeStructure
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action inserts new fee structure for AM
        /// </summary>
        /// <param name="newFeeStruct">newFeeStruct</param>
        /// <returns></returns>
        public ActionResult InsertNewFeeStructure(FeeStructure newFeeStruct)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var newFee = new PartnerCommission();
                    newFee.FK_UserID = loginInfo.UserID;
                    newFee.FK_WidenSpreadID = newFeeStruct.SpreadMarkUp.Int32TryParse();
                    newFee.FK_CommissionID = newFeeStruct.CommissionMarkUp.Int32TryParse();
                    newFee.FeeStructureName = newFeeStruct.StructureName;
                    newFee.FK_AccountCurrencyID = newFeeStruct.Currency.Int32TryParse();
                    newFee.IsDefault = false;
                    if (newFeeStruct.SpreadMarkUp.Int32TryParse() == 7)
                    {
                        newFee.WidenSpreadValue = Convert.ToDouble(newFeeStruct.WidenSpreadOther);
                    }
                    if (newFeeStruct.CommissionMarkUp.Int32TryParse() == 6)
                    {
                        newFee.CommissionValue = Convert.ToDouble(newFeeStruct.CommissionSpreadOther);
                    }

                    partCommBO.AddNewPartnerSpread(newFee);

                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This Function Will Update Bank Information
        /// </summary>
        /// <param name="bankAccountModel">bankAccountModel</param>
        /// <returns>return result</returns>
        public ActionResult UpdateBankInformation(BankAccountModel bankAccountModel)
        {
            try
            {
                var bankAccountInformtionBO = new BankAccountInformationBO();

                var bankAccountInformtion = new BankAccountInformation()
                {
                    PK_BankAccountInformationID = bankAccountModel.BankAccountID,
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
                    bankAccountInformtionBO.UpdateBankAccountInformation(bankAccountInformtion);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated banking information.");

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
        /// This Function Will Add New BankAccount Information
        /// </summary>
        /// <param name="bankAccountModel">bankAccountModel</param>
        /// <returns>return Result</returns>
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
                    var currentUserInfo = SessionManagement.UserInfo;

                    switch (currentUserInfo.LogAccountType)
                    {
                        case LoginAccountType.LiveAccount:
                            bankAccountInformtionBO.AddNewLiveBankAccountInformation(currentUserInfo.UserID, bankAccountInformtion);
                            break;
                        case LoginAccountType.PartnerAccount:
                            bankAccountInformtionBO.AddNewPartnerBankAccountInformation(currentUserInfo.UserID, bankAccountInformtion);
                            break;
                    }

                    //Log activity details in db
                    InsertProfileActivityDetails("You have added new banking information.");

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
        /// This action stores uploaded profile image file in file system
        /// </summary>
        /// <param name="file">file</param>
        /// <returns></returns>
        [HttpPost]
        public string AddImage(HttpPostedFileBase file)
        {
            var javascriptSerailizer = new JavaScriptSerializer();
            try
            {
                if (file.ContentLength > 0)
                {
                    //LoginInformation loginInfo = (LoginInformation)System.Web.HttpContext.Current.Session["UserInfo"];

                    if (SessionManagement.UserInfo != null)
                    {
                        var loginInfo = SessionManagement.UserInfo;


                        //If there is a existing file with different extension delete
                        if (Directory.EnumerateFileSystemEntries(System.Web.HttpContext.Current.Server.MapPath("~/UploadedImages"), loginInfo.UserID + ".*").Any())
                        {
                            System.IO.File.Delete(Directory.EnumerateFileSystemEntries(System.Web.HttpContext.Current.Server.MapPath("~/UploadedImages"), loginInfo.UserID + ".*").First());
                        }

                        var fileName = loginInfo.UserID + file.FileName.Substring(file.FileName.LastIndexOf('.'));

                        //Specify the path for saving
                        var path = Path.Combine(Server.MapPath("~/UploadedImages"), fileName);
                        //Save the file
                        file.SaveAs(path);

                        //Add to context
                        System.Web.HttpContext.Current.Session["ImageURl"] = "../UploadedImages/" + fileName;

                        //Log activity details in db
                        InsertProfileActivityDetails("You have uploaded a new profile image.");

                        return javascriptSerailizer.Serialize(true);
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
        /// This action returns true if emailID already exists in Clients or IntroducingBrokers table
        /// </summary>
        /// <param name="emailID">emailID</param>
        /// <returns></returns>
        public ActionResult CheckIfDuplicateEmail(string emailID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    //If update mail same as present mail, no error is given
                    if (emailID == loginInfo.UserEmail)
                    {
                        return Json(false);
                    }

                    //Check if email present in Users table
                    return Json(userBO.CheckIfEmailExistsInUser(emailID, (int)SessionManagement.OrganizationID));
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(true);
            }
        }

        /// <summary>
        /// This action updates user email of session object
        /// </summary>
        /// <param name="newEmailID">newEmailID</param>
        public void UpdateSessionEmail(string newEmailID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;
                    if (newEmailID != loginInfo.UserEmail)
                    {
                        var prevMailAddress = loginInfo.UserEmail;
                        loginInfo.UserEmail = newEmailID;
                    }
                }

            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method adds uploaded marketing image details
        /// in db and stores image in file system
        /// </summary>
        /// <param name="file">file</param>
        /// <returns></returns>
        public string AddMarketingImage(HttpPostedFileBase file)
        {
            var javascriptSerailizer = new JavaScriptSerializer();
            try
            {
                if (file.ContentLength > 0)
                {
                    if (SessionManagement.UserInfo != null)
                    {
                        LoginInformation loginInfo = SessionManagement.UserInfo;

                        //Store image details in db and get pk ID
                        var pkUserImgID = userImgBO.AddMarketingImageDetails(file.FileName, loginInfo.UserID);

                        var fileName = pkUserImgID + file.FileName.Substring(file.FileName.LastIndexOf('.'));

                        //Specify the path for saving
                        var path = Path.Combine(Server.MapPath("~/MarketingImages"), fileName);
                        //Save the file
                        file.SaveAs(path);

                        return javascriptSerailizer.Serialize(true);
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
        /// This action method deletes marketing image in
        /// database as per image ID
        /// </summary>
        /// <param name="imgID">imgID</param>
        /// <returns></returns>
        public ActionResult DeleteMarketingImage(int imgID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(userImgBO.DeleteMarketingImage(loginInfo.UserID, imgID));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action makes a marketing image active for an AM
        /// </summary>
        /// <param name="imgID">imgID</param>
        /// <returns></returns>
        public ActionResult MakeMarketingImageActive(int imgID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(userImgBO.MakeMarketingImageActive(loginInfo.UserID, imgID));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action disables a marketing ad image of AM
        /// </summary>
        /// <param name="imgID">imgID</param>
        /// <returns></returns>
        public ActionResult DisableMarketingImage(int imgID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(userImgBO.DisableMarketingImage(loginInfo.UserID, imgID));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action returns true if referral link
        /// is not present in database
        /// </summary>
        /// <param name="referralLink">referralLink</param>
        /// <returns></returns>
        public ActionResult CheckDuplicateReferralLink(string referralLink)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(introducingBrokerBO.CheckDuplicateReferralLink(referralLink, (int)SessionManagement.OrganizationID));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action saves AM referral link in IB table
        /// </summary>
        /// <param name="referralLink">referralLink</param>
        /// <returns></returns>
        public ActionResult SaveReferralLink(string referralLink)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(introducingBrokerBO.SaveReferralLink(loginInfo.UserID, referralLink));
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        #region INDIVIDUAL EDIT
        /// <summary>
        /// This method updates personal information based on Live or Partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateIndividualPersonalInformation(PersonalInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateIndividualPersonalInformation(currentUserInfo.UserID, model.PhoneID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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
        /// This method updates Trustee Cmpy Auth Officer Contact Info based on Live or Partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateIndividualContactInforamation(ContactInfoEditModel model)
        {
            try
            {
                var individualAccountInfo = new IndividualAccountInformation()
                {
                    TelephoneNumber = model.TelephoneCountryCode + '-' + model.TelephoneNumber,
                    MobileNumber = model.MobileCountryCode + '-' + model.MobileNumber,
                    EmailAddress = model.EmailID
                };

                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateIndividualContactInformation(currentUserInfo.UserID, individualAccountInfo);
                    
                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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
        #endregion

        #region JOINT EDIT

        /// <summary>
        /// This method updates joint information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateJointPersonalInformation(PersonalInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateJointPersonalInformation(currentUserInfo.UserID, model.PhoneID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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
        /// This method updates joint Contact Info 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateJointContactInforamation(ContactInfoEditModel model)
        {
            try
            {
                var jointAccountInformation = new JointAccountInformation()
                {
                    TelephoneNumber = model.TelephoneCountryCode + '-' + model.TelephoneNumber,
                    MobileNumber = model.MobileCountryCode + '-' + model.MobileNumber,
                    EmailAddress = model.EmailID
                };

                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateJointContactInforamation(currentUserInfo.UserID, jointAccountInformation);

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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

        #endregion

        #region CORPORATE EDIT

        /// <summary>
        /// This method updates company information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateCompanyInformation(PersonalInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateCompanyInformation(loginInfo.UserID, model.PhoneID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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
        /// This method updates company auth officer contact info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateCompanyAuthOfficerContactInfo(ContactInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    introducingBrokerBO.UpdateCompanyAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

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

        #endregion

        /// <summary>
        /// This action disables currently active fee and
        /// makes selected fee active
        /// </summary>
        /// <param name="feeID">feeID</param>
        /// <returns></returns>
        public ActionResult MakeFeeStructureDefault(int feeID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(partCommBO.MakeFeeStructureDefault(loginInfo.UserID, feeID));
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
        /// This action returns ManagedAccountProgram view
        /// </summary>
        /// <returns></returns>
        public ActionResult ManagedAccountProgram()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var organizationID = (int) SessionManagement.OrganizationID;

                    ViewData["AccountCurrency"] = new SelectList(accountCurrencyBO.GetSelectedCurrency(Constants.K_BROKER_PARTNER, organizationID), "PK_AccountCurrencyID", "L_CurrencyValue.CurrencyValue");
                    ViewData["Periods"] = new SelectList(ExtensionUtility.GetPeriod(), "ID", "Value");
                    ViewData["DepositAcceptance"] = new SelectList(ExtensionUtility.GetAllDepositAcceptance(), "ID", "Value");
                    ViewData["FeeGroup"] = new SelectList(partCommBO.GetAllFeeStructureForUser(loginInfo.UserID), "PK_PartnerCommID", "FeeStructureName");
                    ViewData["Platform"] = new SelectList(tradingPlatformBO.GetSelectedPlatform(Constants.K_BROKER_PARTNER, organizationID), "PK_TradingPlatformID", "L_TradingPlatformValues.TradingValue");

                    return View();
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
        /// This action returns ProgramDetails view with
        /// required data passed as model
        /// </summary>
        /// <returns></returns>
        public ActionResult ProgramDetails(int programID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new ManagedAccountProgramDetail();
                    var programDetails = manAccPrgmBO.GetProgramDetails(programID);
                    if (programDetails != null)
                    {
                        model.PK_ProgramID = programDetails.PK_ManagedAccPrgmID;
                        model.ProgramName = programDetails.ProgramName;
                        model.MinimumDeposit = programDetails.MinimumDeposit.ToString();
                        model.Currency = accCurrBO.GetCurrencyLookUpValue((int)programDetails.FK_AccountCurrencyID);
                        model.Platform = tradingPlatformBO.GetTradingPlatformLookUpValue(Constants.K_BROKER_PARTNER, (int)programDetails.FK_PlatformID);
                        model.MinimumStopOutLevel = (float)programDetails.MinStopOutLevel;
                        model.PerformanceFee = (float)programDetails.PerformanceFee;
                        model.ManagementFee = (float)programDetails.ManagementFee;
                        model.PerformanceFeePeriod = programDetails.PerformanceFeePeriod == 1 ? "Monthly" : programDetails.PerformanceFeePeriod == 2 ? "Quaterly" : "Annualy";
                        model.ManagementFeePeriod = programDetails.ManagementFeePeriod == 1 ? "Monthly" : programDetails.ManagementFeePeriod == 2 ? "Quaterly" : "Annualy";

                        switch (programDetails.DepositAcceptance)
                        {
                            case 1:
                                model.DepositAcceptance = "Daily";
                                break;
                            case 2:
                                model.DepositAcceptance = "Weekly";
                                break;
                            case 3:
                                model.DepositAcceptance = "Bi-weekly";
                                break;
                            case 4:
                                model.DepositAcceptance = "Monthly";
                                break;
                            case 5:
                                model.DepositAcceptance = "Quaterly";
                                break;
                            case 6:
                                model.DepositAcceptance = "Annually";
                                break;
                            default:
                                model.DepositAcceptance = "";
                                break;
                        }
                    }

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
        /// This action returns IntroducingBrokerProfitShare view
        /// with required data passed as model
        /// </summary>
        /// <returns></returns>
        public ActionResult IntroducingBrokerProfitShare(int programID, string programName)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var model = new IntroducingBrokerProfitShareModel();

                    model.ProgramID = programID;
                    model.ProgramName = programName;

                    //Get program details
                    var programDetails = manAccPrgmBO.GetProgramDetails(programID);
                    if (programDetails != null)
                    {
                        model.IBPerformanceFeeShare = (double)programDetails.IBPerformanceFeeShare;
                        model.IBManagemantFeeShare = (double)programDetails.IBManagemantFeeShare;
                        model.IBCommissionMarkupShare = (double)programDetails.IBCommissionMarkupShare;
                        model.IBSpreadMarkupShare = (double)programDetails.IBSpreadMarkupShare;
                        model.IBMaskedOffering = (bool)programDetails.IBMaskedOffering;
                    }

                    return View(model);
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
        /// This action adds new managed account program for AM
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddNewManagedAccountProgram(ManagedAccountProgramModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    var program = new ManagedAccountProgram();
                    program.ProgramName = model.ProgramName;
                    program.MinimumDeposit = model.MinimumDeposit;
                    program.FK_AccountCurrencyID = model.CurrencyID;
                    program.FK_FeeGroupID = model.FeeGroupID;
                    program.FK_PlatformID = model.PlatformID;
                    program.PerformanceFeePeriod = model.PerformanceFeePeriod;
                    program.ManagementFeePeriod = model.ManagementFeePeriod;
                    program.DepositAcceptance = model.DepositAcceptance;
                    program.MinStopOutLevel = model.MinimumStopOutLevel;
                    program.PerformanceFee = model.PerformanceFee;
                    program.ManagementFee = model.ManagementFee;
                    program.FK_UserID = loginInfo.UserID;
                    program.IsEnabled = true;
                    program.IBPerformanceFeeShare = 0;
                    program.IBManagemantFeeShare = 0;
                    program.IBCommissionMarkupShare = 0;
                    program.IBSpreadMarkupShare = 0;
                    program.IBMaskedOffering = false;

                    return Json(manAccPrgmBO.AddNewManagedAccount(program));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action return list of managed programs of AssetManager
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllManagedAccountPrograms()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    var lstPrograms = new List<ManagedAccountProgramDetail>();

                    var allPrograms = manAccPrgmBO.GetAllManagedAccPrograms(loginInfo.UserID);
                    foreach (var program in allPrograms)
                    {
                        var prg = new ManagedAccountProgramDetail();
                        prg.PK_ProgramID = program.PK_ManagedAccPrgmID;
                        prg.ProgramName = program.ProgramName;
                        prg.Currency = accCurrBO.GetCurrencyLookUpValue((int)program.FK_AccountCurrencyID);
                        if ((bool)program.IsEnabled)
                        {
                            prg.Action = "<a class='icon active' href='#' onclick='disableProgram(" + program.PK_ManagedAccPrgmID +")' title='Disable Fee'>active</a>";
                        }
                        else
                        {
                            prg.Action = "<a class='icon inactive' href='#' onclick='enableProgram(" + program.PK_ManagedAccPrgmID + ")' title='Enable Fee'>active</a>";
                        }

                        lstPrograms.Add(prg);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstPrograms.Count,
                        rows = lstPrograms
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
                throw;
            }
        }

        /// <summary>
        /// This action disables managed account program of AM
        /// </summary>
        /// <param name="programID">programID</param>
        /// <returns></returns>
        public ActionResult DisableManagedAccountProgram(int programID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(manAccPrgmBO.DisableManagedAccountProgram(programID));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action enables managed account program of AM
        /// </summary>
        /// <param name="programID">programID</param>
        /// <returns></returns>
        public ActionResult EnableManagedAccountProgram(int programID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(manAccPrgmBO.EnableManagedAccountProgram(programID));
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
        /// This action updates IB profit share for a particular program
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateIBProfitShare(IntroducingBrokerProfitShareModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(manAccPrgmBO.UpdateIBProfitShare(model.ProgramID, model.IBPerformanceFeeShare, model.IBManagemantFeeShare, model.IBCommissionMarkupShare, model.IBSpreadMarkupShare, model.IBMaskedOffering));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action logs profile activity details in database
        /// </summary>
        /// <param name="actDetails"></param>
        public void InsertProfileActivityDetails(string activityDetails)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Insert in UserActivity
                    int pkActivityID = userActivityBO.InsertUserActivityDetails(loginInfo.UserID, (int)ActivityType.ProfileActivity);

                    //Insert in ProfileActivity
                    profileActivityBO.InsertProfileActivityDetails(pkActivityID, activityDetails);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }
    }
}
