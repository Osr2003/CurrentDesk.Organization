#region Header Information
/****************************************************************
 * File Name     : ClientAccountDetailsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 27th March 2013
 * Modified Date : 27th March 2013
 * Description   : This file contains view model for ClientAccountDetails view
 * *************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models.MyAccount;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents model for ClientAccountDetails view
    /// </summary>
    public class ClientAccountDetailsModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Balance { get; set; }
        public string Equity { get; set; }
        public bool? IsTradingAcc { get; set; }
        public string PlatformLogin { get; set; }
        public string PlatformPassword { get; set; }
        public List<TransferLogDetails> TransferLogDetails { get; set; }

        public string AccountID { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
    }
}