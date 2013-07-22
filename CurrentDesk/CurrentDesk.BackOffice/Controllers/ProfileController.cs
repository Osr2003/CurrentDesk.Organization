#region Header Information
/********************************************************
 * File Name     : ProfileController.cs
 * Purpose       : This file contains controller actions for profile view of different account types
 * Creation Date : 21st Jan 2013 
 * *****************************************************/
#endregion

#region
using System.Web.Mvc;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Repository.CurrentDesk;
using CurrentDesk.BackOffice.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using CurrentDesk.Models;
using CurrentDesk.BackOffice.Models.Edit;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using CurrentDesk.Logging;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This controller contains actions for profile view of different account types
    /// </summary>
    [AuthorizeTrader, NoCache]
    public class ProfileController : Controller
    {
        #region Variables

        private L_CountryBO countryBO = new L_CountryBO();
        private AccountTypeBO accountTypeBO = new AccountTypeBO();
        private L_InitialInvestmentBO initialInvestmentBO = new L_InitialInvestmentBO();
        private TradingPlatformBO tradingPlatformBO = new TradingPlatformBO();
        private L_TicketSizeBO ticketSizeBO = new L_TicketSizeBO();
        private AccountCurrencyBO accountCurrencyBO = new AccountCurrencyBO();
        private L_LanguagesBO languageBO = new L_LanguagesBO();
        private L_AccountCodeBO accountCodeBO = new L_AccountCodeBO();
        private DemoLeadBO demoLeadBO = new DemoLeadBO();
        private L_IDInformationTypeBO idInfoTypeBO = new L_IDInformationTypeBO();
        private L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        private L_AnnualIncomeValueBO annualIncomeValuesBO = new L_AnnualIncomeValueBO();
        private L_LiquidAssetsValueBO liquidAssetsValuesBO = new L_LiquidAssetsValueBO();
        private L_NetWorthEurosBO netWorthEurosValuesBO = new L_NetWorthEurosBO();
        private L_TradingExperienceBO tradingExpValuesBO = new L_TradingExperienceBO();
        private L_TrusteeTypeValueBO trusteeTypeValuesBO = new L_TrusteeTypeValueBO();
        private L_CompanyTypeValueBO companyTypeValuesBO = new L_CompanyTypeValueBO();
        private L_CommissionIncrementValueBO commissionIncremantValuesBO = new L_CommissionIncrementValueBO();
        private L_WidenSpreadsValueBO widenSpreadValuesBO = new L_WidenSpreadsValueBO();
        private LiveLeadBO liveLeadBO = new LiveLeadBO();
        private ClientBO clientBO = new ClientBO();
        private UserBO userBO = new UserBO();
        private IndividualAccountInformationBO individualAccountInformationBO = new IndividualAccountInformationBO();
        private CorporateAccountInformationBO corporateAccountInformationBO = new CorporateAccountInformationBO();
        private IntroducingBrokerBO introducingBrokerBO = new IntroducingBrokerBO();
        private AssetManagerBO assetManagerBO = new AssetManagerBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private ProfileActivityBO profileActivityBO = new ProfileActivityBO();


        #endregion

        /// <summary>
        /// This actions redirects to proper profile view as per account type
        /// </summary>
        /// <returns></returns>
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

                    if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_INDIVIDUAL)
                    {
                        return RedirectToAction("IndividualProfile");
                    }
                    else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_JOINT)
                    {
                        return RedirectToAction("JointProfile");
                    }
                    else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_CORPORATE)
                    {
                        return RedirectToAction("CorporateProfile");
                    }
                    else if (accountTypeDetails.FK_AccountTypeValue == Constants.K_ACCT_TRUST)
                    {
                        return RedirectToAction("TrustProfile");
                    }
                    else
                    {
                        return View("");
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
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This actions returns Individual profile view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult IndividualProfile()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    //Get Individual Account details for the user
                    var selectedClient = clientBO.GetIndividualAccountDetails(loginInfo.UserID);
                    var userInformation = selectedClient.User;
                    var clientInformations = selectedClient.IndividualAccountInformations.FirstOrDefault();
                    var bankInformationList = selectedClient.BankAccountInformations;
                    var clientAccInfo = selectedClient.Client_Account.FirstOrDefault();
                    var bankList = new List<BankAccountModel>();


                    //Assigning properties to IndividualAccountReviewModel object
                    IndividualAccountReviewModel model = new IndividualAccountReviewModel();
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
                    model.TelNumberCountryCode = Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[0]);
                    model.TelNumber = Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[1]);
                    model.MobileNumberCountryCode = Convert.ToInt64(clientInformations.MobileNumber.Split('-')[0]);
                    model.MobileNumber = Convert.ToInt64(clientInformations.MobileNumber.Split('-')[1]);
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

                    //Return IndividualProfile view with above model
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
        /// This actions returns Joint profile view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult JointProfile()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    //Get Joint Account details for the user
                    var selectedClient = clientBO.GetJointAccountDetails(loginInfo.UserID);
                    var userInformation = selectedClient.User;
                    var clientInformations = selectedClient.JointAccountInformations.FirstOrDefault();
                    var bankInformationList = selectedClient.BankAccountInformations;
                    var clientAccInfo = selectedClient.Client_Account.FirstOrDefault();
                    var bankList = new List<BankAccountModel>();

                    //Assigning properties to JointAccountReviewModel object
                    JointAccountReviewModel model = new JointAccountReviewModel();
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
                    model.PrimaryAccountHoldeTelNumberCountryCode = Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[0]);
                    model.PrimaryAccountHoldeTelNumber = Convert.ToInt64(clientInformations.TelephoneNumber.Split('-')[1]);
                    model.PrimaryAccountHoldeMobileNumberCountryCode = Convert.ToInt64(clientInformations.MobileNumber.Split('-')[0]);
                    model.PrimaryAccountHoldeMobileNumber = Convert.ToInt64(clientInformations.MobileNumber.Split('-')[1]);
                    model.PrimaryAccountHoldeEmailAddress = userInformation.UserEmailID ?? "";

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

                    //Return JointProfile view with above model
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
        /// This actions returns Corporate profile view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult CorporateProfile()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    var selectedResult = clientBO.GetCorporateAccountDetails(loginInfo.UserID);
                    var userInformation = selectedResult.User;
                    var corporateAccountInformation = selectedResult.CorporateAccountInformations.FirstOrDefault();
                    var bankInformationList = selectedResult.BankAccountInformations;
                    var clientAccInfo = selectedResult.Client_Account.FirstOrDefault();
                    var bankList = new List<BankAccountModel>();

                    var corporateAccountReviewModel = new CorporateAccountReviewModel()
                    {
                        //Company Information
                        CompanyName = corporateAccountInformation.CompanyName,
                        CompanyType = companyTypeValuesBO.GetSelectedCompany((int)corporateAccountInformation.FK_CompanyTypeID),
                        CompanyAddLine1 = corporateAccountInformation.CompanyAddress,
                        CompanyAddLine2 = "",
                        CompanyCity = corporateAccountInformation.City,
                        CompanyCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_CompanyCountryID),
                        CompanyPostalCode = corporateAccountInformation.CompanyPostalCode,
                        ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[2] : "",
                        PhoneID = corporateAccountInformation.PhoneID ?? "",

                        //Authorized Officer Information
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

                        //Authorized Officer Contact Information
                        AuthOfficerResidenceCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_AuthOfficerResidenceCountryID),
                        AuthOfficerAddLine1 = corporateAccountInformation.AuthOfficerAddress,
                        AuthOfficerAddLine2 = "",
                        AuthOfficerCity = corporateAccountInformation.AuthOfficerCity,
                        AuthOfficerCountry = countryBO.GetSelectedCountry((int)corporateAccountInformation.FK_AuthOfficerCountryID),
                        AuthOfficerPostalCode = corporateAccountInformation.AuthOfficerPostalCode,
                        AuthOfficerTelNumberCountryCode = string.IsNullOrWhiteSpace(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[0]) ? 000 :
                                                Convert.ToInt64(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[0]),
                        AuthOfficerTelNumber = string.IsNullOrWhiteSpace(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[1]) ? 000 :
                                                Convert.ToInt64(corporateAccountInformation.AuthOfficerTelephoneNumber.Split('-')[1]),
                        AuthOfficerMobileNumberCountryCode = string.IsNullOrWhiteSpace(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[0]) ? 000 :
                                                Convert.ToInt64(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[0]),
                        AuthOfficerMobileNumber = string.IsNullOrWhiteSpace(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[1]) ? 000 :
                                                Convert.ToInt64(corporateAccountInformation.AuthOfficerMobileNumber.Split('-')[1]),
                        AuthOfficerEmailAddress = userInformation.UserEmailID,
                        IsLive = true
                    };

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
                        corporateAccountReviewModel.BankAccountModelList = bankList;
                    }

                    return View(corporateAccountReviewModel);
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
        /// This actions returns Trust profile view with required data
        /// </summary>
        /// <returns></returns>
        public ActionResult TrustProfile()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo((int)SessionManagement.OrganizationID), "PK_RecievingBankID", "RecievingBankName");

                    var selectedResult = clientBO.GetTrustAccountDetails(loginInfo.UserID);
                    var userInformation = selectedResult.User;
                    var trustAccountInformation = selectedResult.TrustAccountInformations.FirstOrDefault();
                    var bankInformationList = selectedResult.BankAccountInformations;
                    var clientAccInfo = selectedResult.Client_Account.FirstOrDefault();
                    var bankList = new List<BankAccountModel>();

                    var trustAccountReviewModel = new TrustAccountReviewModel()
                    {
                        TrustName = trustAccountInformation.TrustName,
                        TrusteeType = trusteeTypeValuesBO.GetSelectedTrusteeType((int)trustAccountInformation.FK_TrusteeTypeID),
                        TrustAddressLine1 = trustAccountInformation.TrustAddress,
                        TrustAddressLine2 = "",
                        TrustCity = trustAccountInformation.TrustCity,
                        TrustCountry = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeCountryID),
                        TrustPostalCode = trustAccountInformation.TrustPostalCode,
                        ClientAccountNumber = clientAccInfo != null ? clientAccInfo.LandingAccount.Split('-')[2] : "",
                        PhoneID = trustAccountInformation.PhoneID ?? "",
                        IsLive = true

                    };

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
                        trustAccountReviewModel.BankAccountModelList = bankList;
                    }


                    if (trustAccountInformation.FK_TrusteeTypeID == Constants.K_TRUSTEETYPE_INDIVIDUAL)
                    {
                        //Individual Information
                        trustAccountReviewModel.IndividualTitle = trustAccountInformation.TrusteeIndividualTitle == "1" ? "Mr" : "Mrs";
                        trustAccountReviewModel.IndividualFirstName = trustAccountInformation.TrusteeIndividualFirstName;
                        trustAccountReviewModel.IndividualLastName = trustAccountInformation.TrusteeIndividualLastName;
                        trustAccountReviewModel.IndividualMiddleName = trustAccountInformation.TrusteeIndividualMiddleName;
                        trustAccountReviewModel.IndividualDobYear = ((DateTime)(trustAccountInformation.TrusteeIndividualBirthDate)).Year;
                        trustAccountReviewModel.IndividualDobMonth = ((DateTime)(trustAccountInformation.TrusteeIndividualBirthDate)).Month.ToString("D2");
                        trustAccountReviewModel.IndividualDobDay = ((DateTime)(trustAccountInformation.TrusteeIndividualBirthDate)).Day.ToString("D2");
                        trustAccountReviewModel.IndividualGender = (bool)trustAccountInformation.TrusteeIndividualGender ? "Male" : "Female";
                        trustAccountReviewModel.IndividualCitizenship = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeIndividualCitizenshipID);
                        trustAccountReviewModel.IndividualIdInfo = idInfoTypeBO.GetSelectedIDInformation((int)trustAccountInformation.FK_TrusteeIndividualIDTypeId);
                        trustAccountReviewModel.IndividualIdNumber = trustAccountInformation.TrusteeIndividualIDNumber;
                        trustAccountReviewModel.IndividualResidenceCountry = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeIndividualResidenceCountryID);

                        //Individual Contact Information
                        trustAccountReviewModel.IndividualResidentialAddLine1 = trustAccountInformation.TrusteeIndividualResidentialAddress;
                        trustAccountReviewModel.IndividualResidentialAddLine2 = "";
                        trustAccountReviewModel.IndividualResidentialCity = trustAccountInformation.TrusteeIndividualResidentialCity;
                        trustAccountReviewModel.IndividualResidentialCountry =
                            countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeIndividualResidentialCountryID);
                        trustAccountReviewModel.IndividualResidentialPostalCode = trustAccountInformation.TrusteeIndividualResidentialPostalCode;
                        trustAccountReviewModel.IndividualYearsInCurrentAdd = (int)(trustAccountInformation.TrusteeIndividualTotalMonthsAtAddress / 12);
                        trustAccountReviewModel.IndividualMonthsInCurrentAdd = (int)(trustAccountInformation.TrusteeIndividualTotalMonthsAtAddress % 12);
                        trustAccountReviewModel.IndividualPreviousAddLine1 = trustAccountInformation.TrusteeIndividualPreviousAddress;
                        trustAccountReviewModel.IndividualPreviousAddLine2 = "";
                        trustAccountReviewModel.IndividualPreviousCity = trustAccountInformation.TrusteeIndividualPreviousCity;
                        trustAccountReviewModel.IndividualPreviousCountry =
                            (trustAccountInformation.FK_TrusteeIndividualPreviousCountryID != null && trustAccountInformation.FK_TrusteeIndividualPreviousCountryID != 0) ?
                            countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeIndividualPreviousCountryID) : "";
                        trustAccountReviewModel.IndividualPreviousPostalCode = trustAccountInformation.TrusteeIndividualPreviousPostalCode;
                        trustAccountReviewModel.IndividualTelNumberCountryCode = string.IsNullOrWhiteSpace(trustAccountInformation.TrusteeIndividualTelephoneNumber.Split('-')[0]) ? 000 :
                            Convert.ToInt64(trustAccountInformation.TrusteeIndividualTelephoneNumber.Split('-')[0]);
                        trustAccountReviewModel.IndividualTelNumber = string.IsNullOrWhiteSpace(trustAccountInformation.TrusteeIndividualTelephoneNumber.Split('-')[1]) ? 000 :
                            Convert.ToInt64(trustAccountInformation.TrusteeIndividualTelephoneNumber.Split('-')[1]);
                        trustAccountReviewModel.IndividualMobileNumberCountryCode = string.IsNullOrWhiteSpace(trustAccountInformation.TrusteeIndividualMobileNumber.Split('-')[0]) ? 000 :
                            Convert.ToInt64(trustAccountInformation.TrusteeIndividualMobileNumber.Split('-')[0]);
                        trustAccountReviewModel.IndividualMobileNumber = string.IsNullOrWhiteSpace(trustAccountInformation.TrusteeIndividualMobileNumber.Split('-')[1]) ? 000 :
                            Convert.ToInt64(trustAccountInformation.TrusteeIndividualMobileNumber.Split('-')[1]);
                        trustAccountReviewModel.IndividualEmailAddress = userInformation.UserEmailID;
                    }
                    else if (trustAccountInformation.FK_TrusteeTypeID == Constants.K_TRUSTEETYPE_COMPANY)
                    {
                        trustAccountReviewModel.TrusteeCompanyName = trustAccountInformation.TrusteeCompanyName;
                        trustAccountReviewModel.TrusteeCompanyType = companyTypeValuesBO.GetSelectedCompany((int)trustAccountInformation.FK_TrusteeCompanyTypeID);
                        trustAccountReviewModel.TrusteeAddressLine1 = trustAccountInformation.TrusteeAddress;
                        trustAccountReviewModel.TrusteeCity = trustAccountInformation.TrusteeCity;
                        trustAccountReviewModel.TrusteeCountry = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeCountryID);
                        trustAccountReviewModel.TrusteePostalCode = trustAccountInformation.TrusteePostalCode;

                        //Trustee Company Authorized Officer Information
                        trustAccountReviewModel.AuthorizedOfficerTitle = trustAccountInformation.TrusteeAuthOfficerTitle == "1" ? "Mr" : "Mrs";
                        trustAccountReviewModel.AuthorizedOfficerFirstName = trustAccountInformation.TrusteeAuthOfficerFirstName;
                        trustAccountReviewModel.AuthorizedOfficerMiddleName = trustAccountInformation.TrusteeAuthOfficerMiddleName;
                        trustAccountReviewModel.AuthorizedOfficerLastName = trustAccountInformation.TrusteeAuthOfficerLastName;
                        trustAccountReviewModel.AuthorizedOfficerDobMonth = ((DateTime)trustAccountInformation.TrusteeAuthOfficerBirthDate).Month.ToString("D2");
                        trustAccountReviewModel.AuthorizedOfficerDobDay = ((DateTime)trustAccountInformation.TrusteeAuthOfficerBirthDate).Day.ToString("D2");
                        trustAccountReviewModel.AuthorizedOfficerDobYear = ((DateTime)trustAccountInformation.TrusteeAuthOfficerBirthDate).Year;
                        trustAccountReviewModel.AuthorizedOfficerGender = (bool)trustAccountInformation.TrusteeAuthOfficerGender ? "Male" : "Female";
                        trustAccountReviewModel.AuthorizedOfficerCitizenship = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeAuthOfficerCitizenshipCountryID);
                        trustAccountReviewModel.AuthorizedOfficerIdInfo = idInfoTypeBO.GetSelectedIDInformation((int)trustAccountInformation.FK_TrusteeAuthOfficerIDType);
                        trustAccountReviewModel.AuthorizedOfficerIdNumber = trustAccountInformation.TrusteeAuthOfficerIDNumber;
                        trustAccountReviewModel.AuthorizedOfficerResidenceCountry = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeAuthOfficerResidenceCountryID);

                        trustAccountReviewModel.AuthorizedOfficerAddressLine1 = trustAccountInformation.TrusteeAuthOfficerAddrerss;
                        trustAccountReviewModel.AuthorizedOfficerAddressLine2 = "";
                        trustAccountReviewModel.AuthorizedOfficerCity = trustAccountInformation.TrusteeAuthOfficerCity;
                        trustAccountReviewModel.AuthorizedOfficerCountry = countryBO.GetSelectedCountry((int)trustAccountInformation.FK_TrusteeAuthOfficerCountryID);
                        trustAccountReviewModel.AuthorizedOfficerPostalCode = trustAccountInformation.TrusteeAuthOfficerPostalCode;
                        trustAccountReviewModel.AuthorizedOfficerTelCountryCode = Convert.ToInt64(trustAccountInformation.TrusteeAuthOfficerTelephoneNumber.Split('-')[0]);
                        trustAccountReviewModel.AuthorizedOfficerTelephoneNumber = Convert.ToInt64(trustAccountInformation.TrusteeAuthOfficerTelephoneNumber.Split('-')[1]);
                        trustAccountReviewModel.AuthorizedOfficerMobCountryCode = Convert.ToInt64(trustAccountInformation.TrusteeAuthOfficerMobileNumber.Split('-')[0]);
                        trustAccountReviewModel.AuthorizedOfficerMobileNumber = Convert.ToInt64(trustAccountInformation.TrusteeAuthOfficerMobileNumber.Split('-')[1]);
                        trustAccountReviewModel.AuthorizedOfficerEmailAddress = userInformation.UserEmailID;
                    }
                    return View(trustAccountReviewModel);
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
        /// This Function Will Update New Bank Information
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
            catch(Exception ex)
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

                //var currentUserInfo = ((LoginInformation)System.Web.HttpContext.Current.Session["UserInfo"]);
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
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        #region INDIVIDUAL EDIT
        /// <summary>
        /// This method updates trust information based on Live or Partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult UpdateIndividualPersonalInformation(PersonalInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;
                    switch (currentUserInfo.LogAccountType)
                    {
                        case LoginAccountType.LiveAccount:
                            clientBO.UpdateIndividualPersonalInformation(currentUserInfo.UserID, model.PhoneID);
                            break;
                        case LoginAccountType.PartnerAccount:
                            introducingBrokerBO.UpdateIndividualPersonalInformation(currentUserInfo.UserID, model.PhoneID);
                            break;
                    }

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
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
                var clientBO = new ClientBO();
                var individualAccountInfo = new IndividualAccountInformation()
                {
                    TelephoneNumber = model.TelephoneCountryCode + '-' + model.TelephoneNumber,
                    MobileNumber = model.MobileCountryCode + '-' + model.MobileNumber,
                    EmailAddress = model.EmailID
                };

                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;
                    switch (currentUserInfo.LogAccountType)
                    {
                        case LoginAccountType.LiveAccount:
                            clientBO.UpdateIndividualContactInforamation(currentUserInfo.UserID, individualAccountInfo);
                            break;
                        case LoginAccountType.PartnerAccount:
                            introducingBrokerBO.UpdateIndividualContactInformation(currentUserInfo.UserID, individualAccountInfo);
                            break;
                    }

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }

                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }
        #endregion

        #region JOINT EDIT

        /// <summary>
        /// This method updates trust information based on Live or Partner account
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
                    switch (currentUserInfo.LogAccountType)
                    {
                        case LoginAccountType.LiveAccount:
                            clientBO.UpdateJointPersonalInformation(currentUserInfo.UserID, model.PhoneID);
                            break;
                        case LoginAccountType.PartnerAccount:
                            introducingBrokerBO.UpdateJointPersonalInformation(currentUserInfo.UserID, model.PhoneID);
                            break;
                    }

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
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
        public ActionResult UpdateJointContactInforamation(ContactInfoEditModel model)
        {
            try
            {
                var clientBO = new ClientBO();
                var jointAccountInformation = new JointAccountInformation()
                {
                    TelephoneNumber = model.TelephoneCountryCode + '-' + model.TelephoneNumber,
                    MobileNumber = model.MobileCountryCode + '-' + model.MobileNumber,
                    EmailAddress = model.EmailID
                };

                if (SessionManagement.UserInfo != null)
                {
                    var currentUserInfo = SessionManagement.UserInfo;
                    switch (currentUserInfo.LogAccountType)
                    {
                        case LoginAccountType.LiveAccount:
                            clientBO.UpdateJointContactInforamation(currentUserInfo.UserID, jointAccountInformation);
                            break;
                        case LoginAccountType.PartnerAccount:
                            introducingBrokerBO.UpdateJointContactInforamation(currentUserInfo.UserID, jointAccountInformation);
                            break;
                    }

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        #endregion

        #region TRUST EDIT

        /// <summary>
        /// This method updates trust information based on Live or Partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateTrustInformation(PersonalInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    //If Live account
                    if (loginInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        clientBO.UpdateTrustInformation(loginInfo.UserID, model.PhoneID);
                    }
                    //If Partner account
                    else
                    {
                        introducingBrokerBO.UpdateTrustInformation(loginInfo.UserID, model.PhoneID);
                    }

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
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
        public ActionResult UpdateTrusteeCmpyAuthOfficerContactInfo(ContactInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    //If Live account
                    if (loginInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        clientBO.UpdateTrusteeCmpyAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }
                    //If Partner account
                    else
                    {
                        introducingBrokerBO.UpdateTrusteeCmpyAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This method updates Trustee Individual Auth Officer Contact Info based on Live or Partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateTrusteeIndividualAuthOfficerContactInfo(ContactInfoEditModel model)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    var loginInfo = SessionManagement.UserInfo;

                    //If Live account
                    if (loginInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        clientBO.UpdateTrusteeIndividualAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }
                    //If Partner account
                    else
                    {
                        introducingBrokerBO.UpdateTrusteeIndividualAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        #endregion

        #region CORPORATE EDIT

        /// <summary>
        /// This method updates company information based on Live or Partner account
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

                    //If Live account
                    if (loginInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        clientBO.UpdateCompanyInformation(loginInfo.UserID, model.PhoneID);
                    }
                    //If Partner account
                    else
                    {
                        introducingBrokerBO.UpdateCompanyInformation(loginInfo.UserID, model.PhoneID);
                    }

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This method updates company auth officer contact info based on Live or Partner account
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

                    //If Live account
                    if (loginInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        clientBO.UpdateCompanyAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }
                    //If Partner account
                    else
                    {
                        introducingBrokerBO.UpdateCompanyAuthOfficerContactInfo(loginInfo.UserID, model.TelephoneCountryCode, model.TelephoneNumber, model.MobileCountryCode, model.MobileNumber, model.EmailID);
                    }

                    //Update session email
                    UpdateSessionEmail(model.EmailID);

                    //Log activity details in db
                    InsertProfileActivityDetails("You have updated your profile information.");

                    return Json(true);
                }
                return Json(false);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        #endregion

        /// <summary>
        /// This actions adds new profile image of user
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
                    if (SessionManagement.UserInfo != null)
                    {
                        var loginInfo = SessionManagement.UserInfo;
                      
                        //If there is a existing file with different extension delete
                        if (Directory.EnumerateFileSystemEntries(Server.MapPath("~/UploadedImages"), loginInfo.UserID + ".*").Any())
                        {
                            System.IO.File.Delete(Directory.EnumerateFileSystemEntries(Server.MapPath("~/UploadedImages"), loginInfo.UserID + ".*").First());
                        }

                        var fileName = loginInfo.UserID + file.FileName.Substring(file.FileName.LastIndexOf('.'));

                        //Specify the path for saving
                        var path = Path.Combine(Server.MapPath("~/UploadedImages"), fileName);
                        //Save the file
                        file.SaveAs(path);

                        //Add to context
                        System.Web.HttpContext.Current.Session["ImageURl"] = Path.Combine("../UploadedImages/", fileName);

                        //Log activity details in db
                        InsertProfileActivityDetails("You have uploaded a new profile image.");

                        return javascriptSerailizer.Serialize(true);
                    }
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
            return javascriptSerailizer.Serialize(false);

        }

        /// <summary>
        /// This action returns true if emailID already exists in Clients or IntroducingBrokers table
        /// </summary>
        /// <param name="emailID"></param>
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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
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
