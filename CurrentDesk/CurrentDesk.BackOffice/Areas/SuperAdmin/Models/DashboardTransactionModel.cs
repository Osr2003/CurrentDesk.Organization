#region Header Information
/*******************************************************************************
 * File Name     : DashboardTransactionModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 4th July 2013
 * Modified Date : 4th July 2013
 * Description   : This file contains view model for Transaction section of
 *                 Admin Dashboard page
 * *****************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Models
{
    /// <summary>
    /// This file contains view model for Transaction
    /// section of Admin Dashboard page
    /// </summary>
    public class DashboardTransactionModel
    {
        public string TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientName { get; set; }
        public string Amount { get; set; }
        public string ToAccount { get; set; }
        public string ToClientName { get; set; }
        public double ExchangeRate { get; set; }
        public string ExchangedAmount { get; set; }
        public string Status { get; set; }
    }
}