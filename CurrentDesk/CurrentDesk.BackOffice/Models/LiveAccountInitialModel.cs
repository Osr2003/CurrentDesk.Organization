/* **************************************************************
* File Name     :- LiveAccountInitialModel.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 2nd Jan 2013
* Modified Date :- 2nd Jan 2013
* Description   :- This file contains all the property related to LiveAccountInitialModel(short Form)
****************************************************************/


#region Namespace
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CurrentDesk.BackOffice.Custom;
#endregion


namespace CurrentDesk.BackOffice.Models
{
    /// <summary>
    /// Class containing properties for live account initial
    /// </summary>
    public class LiveAccountInitialModel
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

        [Required(ErrorMessage="*")]
        public string Password{ get; set; }

        [Required(ErrorMessage="*")]
        [Compare("Password",ErrorMessage="Confirm Password doesn't Match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "*")]
        public int LanguageCode { get; set; }
                
        public bool IsEnglishSpeaking { get; set; }
        
        [BooleanRequired(ErrorMessage="*")]
        public bool IsCustomerAgreement { get; set; }
        
        [BooleanRequired(ErrorMessage = "*")]
        public bool IsBuisnessTerm { get; set; }

        [BooleanRequired(ErrorMessage = "*")]
        public bool IsRiskAcknowledgementAndDisclosure { get; set; }
        
        [BooleanRequired(ErrorMessage = "*")]
        public bool IsArbitartionClauseCustomerAgreement { get; set; }

        
        [BooleanRequired(ErrorMessage = "*")]
        public bool IsResident { get; set; }
    }
}