#region Header Information
/***************************************************************
 * File Name     : IndividualAccountModel.cs
 * Purpose       : This file contains model for Individual Account creation form
 * Creation Date : 3rd Jan 2013
 * *************************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Models
{
    /// <summary>
    /// This is a model for Individual Account creation form
    /// </summary>
    public class IndividualAccountModel
    {        
        //Personal Information        
        [Required(ErrorMessage = "*")]
        public string Title { get; set; }

        [Required(ErrorMessage = "*")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int DobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int DobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int DobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "*")]
        public int Citizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int IdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int ResidenceCountry { get; set; }

        //Contact Information
        [Required(ErrorMessage = "*")]
        public string ResidentialAddLine1 { get; set; }

        public string ResidentialAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string ResidentialCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int ResidentialCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string ResidentialPostalCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Years at current address"), Range(0, 100, ErrorMessage = "Years range is from 0 to 100")]
        public int YearsInCurrentAdd { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Months at current address"), Range(0, 12, ErrorMessage = "Months range is from 0 to 12")]
        public int MonthsInCurrentAdd { get; set; }

        [GreaterThanIf("YearsInCurrentAdd", "MonthsInCurrentAdd", "yearsInCurrentAdd", "monthsInCurrentAdd",ErrorMessage ="*" )]
        public string PreviousAddLine1 { get; set; }

        public string PreviousAddLine2 { get; set; }

        [GreaterThanIf("YearsInCurrentAdd", "MonthsInCurrentAdd", "yearsInCurrentAdd", "monthsInCurrentAdd", ErrorMessage = "*")]
        public string PreviousCity { get; set; }

        [GreaterThanIf("YearsInCurrentAdd", "MonthsInCurrentAdd", "yearsInCurrentAdd", "monthsInCurrentAdd", ErrorMessage = "*")]
        public int? PreviousCountry { get; set; }

        [GreaterThanIf("YearsInCurrentAdd", "MonthsInCurrentAdd", "yearsInCurrentAdd", "monthsInCurrentAdd", ErrorMessage = "*")]
        public string PreviousPostalCode { get; set; }
        
        [Display(Name = "Country Code")]
        public long TelNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long TelNumber { get; set; }
       
        [Display(Name = "Country Code")]
        public long MobileNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long MobileNumber { get; set; }
        
        //[Required(ErrorMessage ="*")]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        //public string EmailAddress { get; set; }

        //[Required(ErrorMessage = "*")]
        //[Compare("EmailAddress", ErrorMessage = "Email and confirm email doesn't match.")]
        //public string ConfirmEmailAddress { get; set; }

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