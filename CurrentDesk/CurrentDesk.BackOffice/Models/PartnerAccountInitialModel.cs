/* **************************************************************
* File Name     :- PartnerAccountInitialModel.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 2nd Jan 2013
* Modified Date :- 2nd Jan 2013
* Description   :- This file contains all the property related to PartnerAccountInitialModel(short Form)
****************************************************************/


#region Namespace
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Models
{
    /// <summary>
    /// Class containing properties for partner account initial
    /// </summary>
    public class PartnerAccountInitialModel
    {
        [Required(ErrorMessage = "*")]
        public int AccountCode { get; set; }

        [Required(ErrorMessage = "*")]
        public int AccountType { get; set; }

        [Required(ErrorMessage = "*")]
        public int TradingPlatform { get; set; }

        [Required(ErrorMessage = "*")]
        public int AccountCurrency { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        [Remote("CheckIfDuplicateEmail", "AccountSignUp", ErrorMessage = "Email already exists!")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "*")]
        public string Password { get; set; }

        [Required(ErrorMessage = "*")]
        [Compare("Password", ErrorMessage = "Confirm Password doesn't Match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "*")]
        public int LanguageCode { get; set; }

        public bool IsEnglishSpeaking { get; set; }

        [BooleanRequired(ErrorMessage = "*")]
        public bool IsPartnershipCommissionAgreement { get; set; }

        [BooleanRequired(ErrorMessage = "*")]
        public bool IsConfidentialityAgreement { get; set; }
        
        public int WidenSpread { get; set; }

        [RequiredIf("WidenSpread", 7, "widenSpread", "widenSpreadOthers", ErrorMessage = "*")]
        public double WidenSpreadOthers { get; set; }
        
        public int IncreasedCommission { get; set; }

        [RequiredIf("IncreasedCommission", 6, "increasedCommissions", "increasedCommissionOthers", ErrorMessage = "*")]
        public double IncreasedCommissionOthers { get; set; }       
    }
}