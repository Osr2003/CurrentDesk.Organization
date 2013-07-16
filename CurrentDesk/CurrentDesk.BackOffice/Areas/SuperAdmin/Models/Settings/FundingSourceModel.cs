#region Header Information
/****************************************************************************
 * File Name     : FundingSourceModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 13th June 2013
 * Modified Date : 13th June 2013
 * Description   : This file contains view model for FundingSourceManagement
 *                 view under Settings section of super admin
 * **************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Models.Settings
{
    /// <summary>
    /// This class represents view model for FundingSourceManagement 
    /// view under Settings section of super admin
    /// </summary>
    public class FundingSourceModel
    {
        public int SourceTypeID { get; set; }
        public int ReceivingBankInfoID { get; set; }
        public int CountryID { get; set; }
        public int CurrencyID { get; set; }
    }

    public class FundingSourceDetail
    {
        public int PK_FundingSourceID { get; set; }
        public string SourceType { get; set; }
        public string SourceName { get; set; }
        public string BankName { get; set; }
        public string BicOrSwiftCode { get; set; }
        public string AccountNumber { get; set; }
        public int FK_ReceivingBankInfoID { get; set; }
        public string ReceivingBankInfo { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public int FK_BankCountryID { get; set; }
        public string BankPostalCode { get; set; }
        public string InterBankName { get; set; }
        public int FK_InterBankCountryID { get; set; }
        public string InterBicOrSwiftCode { get; set; }
        public decimal IncomingWireFeeAmount { get; set; }
        public decimal OutgoingWireFeeAmount { get; set; }
        public int FK_IncomingWireFeeCurr { get; set; }
        public int FK_OutgoingWireFeeCurr { get; set; }
        public string AcceptedCurr { get; set; }
        public string Action { get; set; }
    }
}