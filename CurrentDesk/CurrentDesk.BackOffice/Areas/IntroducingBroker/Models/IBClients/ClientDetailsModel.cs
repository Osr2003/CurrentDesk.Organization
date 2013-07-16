#region Header Information
/**************************************************************************
 * File Name     : ClientDetailsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 26th March 2013
 * Modified Date : 26th March 2013
 * Description   : This file contains view model for ClientDetails view
 * ***********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for ClientDetails view
    /// </summary>
    public class ClientDetailsModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DobMonth { get; set; }
        public string DobDay { get; set; }
        public int DobYear { get; set; }
        public string Gender { get; set; }
        public string Citizenship { get; set; }
        public string IdInfo { get; set; }
        public string IdNumber { get; set; }
        public string ResidenceCountry { get; set; }
        public int? AgentID { get; set; }
        public string ClientAccountNumber { get; set; }
        public string PhoneID { get; set; }
        public string ResidentialAddLine1 { get; set; }
        public string ResidentialAddLine2 { get; set; }
        public string ResidentialCity { get; set; }
        public string ResidentialCountry { get; set; }
        public string ResidentialPostalCode { get; set; }
        public int YearsInCurrentAdd { get; set; }
        public int MonthsInCurrentAdd { get; set; }
        public string PreviousAddLine1 { get; set; }
        public string PreviousAddLine2 { get; set; }
        public string PreviousCity { get; set; }
        public string PreviousCountry { get; set; }
        public string PreviousPostalCode { get; set; }
        public long TelNumberCountryCode { get; set; }
        public long TelNumber { get; set; }
        public long MobileNumberCountryCode { get; set; }
        public long MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }

        //List Of Bank Account
        public List<BankAccountModel> BankAccountModelList { get; set; }

        public int ClientID { get; set; }
        public string AccountID { get; set; }
        public string Status { get; set; }
    }
}