/* **************************************************************
* File Name     :- JointAccountModel.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 2nd Jan 2013
* Modified Date :- 2nd Jan 2013
* Description   :- This file contains all the property related to JointAccountModel
****************************************************************/

#region Namespace
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Models
{
    /// <summary>
    /// Class Containing properties for Joint Account
    /// </summary>
    public class JointAccountModel
    {       
        //Primary Account Holder Information
        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHolderTitle { get; set; }

        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHolderFirstName { get; set; }

        public string PrimaryAccountHolderMiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHolderLastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderDobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderDobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderDobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool PrimaryAccountHolderGender { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderCitizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderIdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string PrimaryAccountHolderIdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHolderResidenceCountry { get; set; }


        //Secondary Account Holder Information
        [Required(ErrorMessage = "*")]
        public string SecondaryAccountHolderTitle { get; set; }

        [Required(ErrorMessage = "*")]
        public string SecondaryAccountHolderFirstName { get; set; }

        public string SecondaryAccountHolderMiddleName { get; set; }

        [Required(ErrorMessage = "*")]
        public string SecondaryAccountHolderLastName { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderDobMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderDobDay { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderDobYear { get; set; }

        [Required(ErrorMessage = "*")]
        public bool SecondaryAccountHolderGender { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderCitizenship { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderIdInfo { get; set; }

        [Required(ErrorMessage = "*"), RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "*")]
        public string SecondaryAccountHolderIdNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int SecondaryAccountHolderResidenceCountry { get; set; }

        //Primary Account HolderContact Information
        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHolderResidentialAddLine1 { get; set; }

        public string PrimaryAccountHoldeResidentialAddLine2 { get; set; }

        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHoldeResidentialCity { get; set; }

        [Required(ErrorMessage = "*")]
        public int PrimaryAccountHoldeResidentialCountry { get; set; }

        [Required(ErrorMessage = "*")]
        public string PrimaryAccountHoldeResidentialPostalCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Years in current address"), Range(0, 100, ErrorMessage = "Years range is from 0 to 100")]
        public int PrimaryAccountHoldeYearsInCurrentAdd { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Months in current address"), Range(0, 12, ErrorMessage = "Months range is from 0 to 12")]
        public int PrimaryAccountHoldeMonthsInCurrentAdd { get; set; }

        [GreaterThanIf("PrimaryAccountHoldeYearsInCurrentAdd", "PrimaryAccountHoldeMonthsInCurrentAdd", "primaryAccountHoldeYearsInCurrentAdd", "primaryAccountHoldeMonthsInCurrentAdd", ErrorMessage = "*")]
        public string PrimaryAccountHoldePreviousAddLine1 { get; set; }
        
        public string PrimaryAccountHoldePreviousAddLine2 { get; set; }

       [GreaterThanIf("PrimaryAccountHoldeYearsInCurrentAdd", "PrimaryAccountHoldeMonthsInCurrentAdd", "primaryAccountHoldeYearsInCurrentAdd", "primaryAccountHoldeMonthsInCurrentAdd", ErrorMessage = "*")]
        public string PrimaryAccountHoldePreviousCity { get; set; }

        [GreaterThanIf("PrimaryAccountHoldeYearsInCurrentAdd", "PrimaryAccountHoldeMonthsInCurrentAdd", "primaryAccountHoldeYearsInCurrentAdd", "primaryAccountHoldeMonthsInCurrentAdd", ErrorMessage = "*")]
        public int? PrimaryAccountHoldePreviousCountry { get; set; }

        [GreaterThanIf("PrimaryAccountHoldeYearsInCurrentAdd", "PrimaryAccountHoldeMonthsInCurrentAdd", "primaryAccountHoldeYearsInCurrentAdd", "primaryAccountHoldeMonthsInCurrentAdd", ErrorMessage = "*")]
        public string PrimaryAccountHoldePreviousPostalCode { get; set; }

        
        [Display(Name = "Country Code")]
        public long PrimaryAccountHoldeTelNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long PrimaryAccountHoldeTelNumber { get; set; }
        
        [Display(Name = "Country Code")]
        public long PrimaryAccountHoldeMobileNumberCountryCode { get; set; }
        
        [Display(Name = "Number")]
        public long PrimaryAccountHoldeMobileNumber { get; set; }

        //[Required(ErrorMessage="*")]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        //public string PrimaryAccountHoldeEmailAddress { get; set; }

        //[Required(ErrorMessage = "*")]
        //[Compare("PrimaryAccountHoldeEmailAddress", ErrorMessage = "Email and confirm email doesn't match.")]
        //public string PrimaryAccountHoldeConfirmEmailAddress { get; set; }

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

    /// <summary>
    /// This class contains properties for Radio button list
    /// </summary>
    public class RadioButtonOptions
    {
        public string RadioBtnLabel { get; set; }
        public int Id { get; set; }
    }
}