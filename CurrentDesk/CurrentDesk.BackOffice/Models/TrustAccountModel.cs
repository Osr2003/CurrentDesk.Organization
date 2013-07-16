/* **************************************************************
* File Name     :- TrustAccountModel.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 2nd Jan 2013
* Modified Date :- 5nd Jan 2013
* Description   :- This file contains all the property related to TrustAccountModel
****************************************************************/

#region Namespace
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Models
{

    /// <summary>
    /// Class Containing properties for Trust account
    /// </summary>
    public class TrustAccountModel
    {
        //Trust Information
        [Required(ErrorMessage = "*")]
        public string TrustName { get; set; }

        [Required(ErrorMessage = "*")]
        public int TrusteeType { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrustAddressLine1 { get; set; }

        public string TrustAddressLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrustCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int TrustCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrustPostalCode { get; set; }

        //Trustee Company Information
        [Required(ErrorMessage = "*")]
        public string TrusteeCompanyName { get; set; }

        [Required(ErrorMessage = "*")]
        public int TrusteeCompanyType { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrusteeAddressLine1 { get; set; }

        public string TrusteeAddressLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrusteeCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int TrusteeCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string TrusteePostalCode { get; set; }

        //Trustee Company Authorized Officer Information
        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerTitle { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerFirstName { get; set; }

        public string AuthorizedOfficerMiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerLastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerDobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerDobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerDobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool AuthorizedOfficerGender { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerCitizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerIdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string AuthorizedOfficerIdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerResidenceCountry { get; set; }


        //Trustee Company Authorized Officer Contact Information
        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerAddressLine1 { get; set; }

        public string AuthorizedOfficerAddressLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthorizedOfficerCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthorizedOfficerPostalCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Country Code")]
        public long AuthorizedOfficerTelCountryCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number")]
        public long AuthorizedOfficerTelephoneNumber { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number")]
        public long AuthorizedOfficerMobCountryCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number")]
        public long AuthorizedOfficerMobileNumber { get; set; }

        //[Required(ErrorMessage = "*")]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        //public string AuthorizedOfficerEmailAddress { get; set; }

        //[Required(ErrorMessage = "*")]
        //[Compare("AuthorizedOfficerEmailAddress", ErrorMessage = "Email and confirm email doesn't match.")]
        //public string AuthorizedOfficerConfirmEmailAddress { get; set; }

        //Trust Individual Information
        [Required(ErrorMessage = "*")]
        public string IndividualTitle { get; set; }

        [Required(ErrorMessage = "*")]
        public string IndividualFirstName { get; set; }

        public string IndividualMiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string IndividualLastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualDobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualDobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualDobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool IndividualGender { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualCitizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualIdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string IndividualIdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualResidenceCountry { get; set; }

        //Trust  Individual Contact Information
        [Required(ErrorMessage = "*")]
        public string IndividualResidentialAddLine1 { get; set; }

        public string IndividualResidentialAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string IndividualResidentialCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int IndividualResidentialCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string IndividualResidentialPostalCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Years in current address"), Range(0, 100, ErrorMessage = "Years range is from 0 to 100")]
        public int IndividualYearsInCurrentAdd { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Months in current address"), Range(0, 12, ErrorMessage = "Months range is from 0 to 12")]
        public int IndividualMonthsInCurrentAdd { get; set; }

        [GreaterThanIf("IndividualYearsInCurrentAdd", "IndividualMonthsInCurrentAdd", "individualYearsInCurrentAdd", "individualMonthsInCurrentAdd", ErrorMessage = "*")]
        [Required(ErrorMessage = "*")]
        public string IndividualPreviousAddLine1 { get; set; }

        public string IndividualPreviousAddLine2 { get; set; }

        [GreaterThanIf("IndividualYearsInCurrentAdd", "IndividualMonthsInCurrentAdd", "individualYearsInCurrentAdd", "individualMonthsInCurrentAdd", ErrorMessage = "*")]
        public string IndividualPreviousCity { get; set; }

        [GreaterThanIf("IndividualYearsInCurrentAdd", "IndividualMonthsInCurrentAdd", "individualYearsInCurrentAdd", "individualMonthsInCurrentAdd", ErrorMessage = "*")]
        public int IndividualPreviousCountry { get; set; }

        [GreaterThanIf("IndividualYearsInCurrentAdd", "IndividualMonthsInCurrentAdd", "individualYearsInCurrentAdd", "individualMonthsInCurrentAdd", ErrorMessage = "*")]
        public string IndividualPreviousPostalCode { get; set; }
        
        [Display(Name = "Country Code")]
        public long IndividualTelNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long IndividualTelNumber { get; set; }
       
        [Display(Name = "Country Code")]
        public long IndividualMobileNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long IndividualMobileNumber { get; set; }

        //[Required(ErrorMessage="*")]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        //public string IndividualEmailAddress { get; set; }

        // [Required(ErrorMessage = "*")]
        //[Compare("IndividualEmailAddress", ErrorMessage = "Email and confirm email doesn't match.")]
        //public string IndividualConfirmEmailAddress { get; set; }

        //Banking Information
        [Required(ErrorMessage = "*")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "*")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public string BicOrSwiftCode { get; set; }

        [Required(ErrorMessage = "*")]
        public string BankAddLine1 { get; set; }

        public string BankAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public int ReceivingBankInfoId { get; set; }

        [Required(ErrorMessage = "*")]
        public string ReceivingBankInfo { get; set; }

        [Required(ErrorMessage = "*")]
        public string BankCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int BankCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string BankPostalCode { get; set; }

        //Financial Information
        [Required(ErrorMessage = "*")]
        public int EstimatedAnnualIncome { get; set; }

        [Required(ErrorMessage = "*")]
        public int LiquidAssets { get; set; }

        [Required(ErrorMessage = "*")]
        public int NetWorthEuros { get; set; }

        //Trading Experience      

        [Required(ErrorMessage = "*")]
        public int DrpHaveExperienceTradingSecurities { get; set; }

        [Required(ErrorMessage = "*")]
        public int DrpHaveExperienceTradingOptions { get; set; }

        [Required(ErrorMessage = "*")]
        public int DrpHaveExperienceTradingForeignExchange { get; set; }

        //Other Information
        [Required(ErrorMessage = "*")]
        public bool HaveAccWithFqSecurities { get; set; }

        [Required(ErrorMessage = "*")]
        public bool RequiredToBeRegisteredWithRegulator { get; set; }

        [Required(ErrorMessage = "*")]
        public bool EverDeclaredBankruptcy { get; set; }

        [Required(ErrorMessage = "*")]
        public bool RegisteredPerson { get; set; }

        [Required(ErrorMessage = "*")]
        public bool EmployeeOfExchangeOrRegulatorOperator { get; set; }
    }
}