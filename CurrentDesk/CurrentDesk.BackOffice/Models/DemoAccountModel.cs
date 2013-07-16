/* **************************************************************
* File Name     :- DemoAccountModel.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 2nd Jan 2013
* Modified Date :- 2nd Jan 2013
* Description   :- This file contains all the property related to DemoAccount
****************************************************************/

#region Namespace
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Models
{
    /// <summary>
    /// Class with properties of demo account
    /// </summary>
    public class DemoAccountModel
    {
        [Required(ErrorMessage = "*")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "*")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email.")]
        [Remote("CheckIfDuplicateDemoAccountEmail", "AccountSignUp", ErrorMessage = "Email already exists!")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "*")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public int CountryCode { get; set; }

        public string IntroducingBrokerOrAgent { get; set; }

        [Required(ErrorMessage = "*")]
        public int AccountType { get; set; }

        [Required(ErrorMessage = "*")]
        public int AccountCurrency { get; set; }

        [Required(ErrorMessage = "*")]
        public int TradingPlatform { get; set; }

        [Required(ErrorMessage = "*")]
        public int InitialInvestment { get; set; }

        [Required(ErrorMessage = "*")]
        public int TradingExperience { get; set; }

        [Required(ErrorMessage = "*")]
        public int TicketSize { get; set; }

        [Required(ErrorMessage = "*")]
        public int LanguageCode { get; set; }

        [Required(ErrorMessage = "*")]
        public bool IsEnglishSpeaking { get; set; }
    }
}