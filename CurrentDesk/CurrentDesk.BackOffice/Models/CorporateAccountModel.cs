#region Header Information
/***********************************************************
 * File Name     : CorporateAccountModel.cs
 * Purpose       : This file contains model for Corporate Account creation form
 * Creation Date : 3rd Jan 2013
 * ********************************************************/
#endregion

#region Namespace Used
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Models
{
    public class CorporateAccountModel
    {
        //Company Information
        [Required(ErrorMessage = "*")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "*")]
        public int CompanyType { get; set; }

        [Required(ErrorMessage = "*")]
        public string CompanyAddLine1 { get; set; }

        public string CompanyAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string CompanyCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int CompanyCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string CompanyPostalCode { get; set; }

        //Authorized Officer
        [Required(ErrorMessage = "*")]
        public string AuthOfficerTitle { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthOfficerFirstName { get; set; }

        public string AuthOfficerMiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthOfficerLastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerDobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerDobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerDobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool AuthOfficerGender { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerCitizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerIdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string AuthOfficerIdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerResidenceCountry { get; set; }

        //Authorized Officer Contact Information
        [Required(ErrorMessage = "*")]
        public string AuthOfficerAddLine1 { get; set; }

        public string AuthOfficerAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthOfficerCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int AuthOfficerCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string AuthOfficerPostalCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Country Code")]
        public long AuthOfficerTelNumberCountryCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number")]
        public long AuthOfficerTelNumber { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Country Code")]
        public long AuthOfficerMobileNumberCountryCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number")]
        public long AuthOfficerMobileNumber { get; set; }

        //[Required(ErrorMessage = "*")]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        //public string AuthOfficerEmailAddress { get; set; }

        //[Required(ErrorMessage = "*")]
        //[Compare("AuthOfficerEmailAddress", ErrorMessage = "Email and confirm email doesn't match.")]
        //public string AuthOfficerConfirmEmailAddress { get; set; }

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