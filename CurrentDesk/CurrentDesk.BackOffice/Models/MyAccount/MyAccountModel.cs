#region Header  Information
/**************************************************************
 * File Name     : MyAccountModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 8th Feb 2013
 * Modified Date : 8th Feb 2013
 * Description   : This file contains model for MyAccounts view
 * ***********************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Models.MyAccount
{
    /// <summary>
    /// This class represents model for MyAccounts page
    /// </summary>
    public class MyAccountModel
    {
        public List<MyAccountCurrencyModel> CurrencyAccountDetails { get; set; }
        public int Currency { get; set; }
        public int AccountCode { get; set; }
        public int TradingPlatform { get; set; }
    }

    /// <summary>
    /// This class is part of MyAccountModel
    /// </summary>
    public class MyAccountCurrencyModel
    {
        public string CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyImage { get; set; }
        public string LandingAccount { get; set; }
        public string LAccBalance { get; set; }
    }
}