#region Header Information
/*****************************************************************************
 * File Name     : InternalTransferModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 24th June 2013
 * Modified Date : 24th June 2013
 * Description   : This file contains view model for InternalTransfers view
 *                 of Transactions section
 * ****************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Models.Transactions
{
    /// <summary>
    /// This class represents view model for InternalTransfers view
    /// of Transactions section
    /// </summary>
    public class InternalTransferModel
    {
        public int TransferCurrencyID { get; set; }
        public int ApprovalOptionID { get; set; }
        public decimal TransferFee { get; set; }
        public decimal? LimitedAmount { get; set; }
        public float MarginRestriction { get; set; }
        public int FromClientUserID { get; set; }
        public int ToClientUserID { get; set; }
        public int ConversionMarkupType { get; set; }
        public double ConversionMarkupValue { get; set; }
    }
}