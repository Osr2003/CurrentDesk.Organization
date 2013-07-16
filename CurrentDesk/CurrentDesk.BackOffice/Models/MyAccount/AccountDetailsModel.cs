#region Header Information
/*********************************************************************
 * File Name     : AccountDetailsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 5th March 2013
 * Modified Date : 5th March 2013
 * Description   : This file contains model for AccountDetails view
 * *******************************************************************/
#endregion

#region Namespace Used
using System;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Models.MyAccount
{
    /// <summary>
    /// This class represents model for AccountDetails view
    /// </summary>
    public class AccountDetailsModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Balance { get; set; }
        public bool? IsTradingAcc { get; set; }
        public string PlatformLogin { get; set; }
        public string PlatformPassword { get; set; }
        public string Equity { get; set; }
        public List<TransferLogDetails> TransferLogDetails { get; set; }
    }

    public class TransferLogDetails
    {
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string TransactionAmount { get; set; }
    }
}