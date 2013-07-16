#region Header Information
/************************************************************************
 * File Name     : ClientAccountsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 27th March 2013
 * Modified Date : 27th March 2013
 * Description   : This file contains view model for ClientAccounts view
 * **********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models.MyAccount;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for ClientAccounts view
    /// </summary>
    public class ClientAccountsModel
    {
        public List<MyAccountCurrencyModel> CurrencyAccountDetails { get; set; }
        public int Currency { get; set; }
        public int AccountCode { get; set; }
        public int TradingPlatform { get; set; }
        public int ClientID { get; set; }
        public string AccountID { get; set; }
        public string ClientName { get; set; }
    }
}