#region Header Information
/****************************************************************************
 * File Name     : AgentDetailsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions 
 * Creation Date : 15th April 2013
 * Modified Date : 15th April 2013
 * Description   : This file contains view model for AgentDetails view
 * *************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBAgents
{
    /// <summary>
    /// This class represents view model for AgentDetails view
    /// </summary>
    public class AgentDetailsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneID { get; set; }
        public string Password { get; set; }
        public string AgentAddress { get; set; }
        public string AgentCity { get; set; }
        public string AgentCountry { get; set; }
        public string AgentPostalCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BicOrSwiftCode { get; set; }
        public string ReceivingBankInfo { get; set; }
        public string ReceivingBankInfoDetail { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankCountry { get; set; }
        public string BankPostalCode { get; set; }
        public int AgentID { get; set; }
    }
}