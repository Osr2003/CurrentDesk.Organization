#region Header Information
/***************************************************************************
 * File Name     : TransfersModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th Feb 2013
 * Modified Date : 11th Feb 2013
 * Description   : This file contains model for Transfers page
 * ************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.Models;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Models.Transfers
{
    /// <summary>
    /// This class is passed as model in Transfers view
    /// </summary>
    public class TransfersModel
    {
        public List<BankInformation> BankInformation { get; set; }
        public List<LandingAccInformation> LandingAccInformation { get; set; }
        public List<TradingAccountGrouped> TradingAccInformation { get; set; }
        public string ReceivingBankInfoId { get; set; }
        public string BankCountry { get; set; }
        public string AccountNumber { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string AccountID { get; set; }
        public int FundSourceID { get; set; }
    }

    public class BankInformation
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankAccNumber { get; set; }
    }

    public class LandingAccInformation
    {
        public string LCurrencyName { get; set; }
        public string LAccNumber { get; set; }
        public string LAccBalance { get; set; }
        public string LAccCustomName { get; set; }
    }

    public class TradingAccountGrouped
    {
        public string TradingCurrency { get; set; }
        public string TradingCustomName { get; set; }
        public List<Client_Account> TradingAccountList { get; set; }
    }

    public class FundAccountData
    {
        public string AccountNumber { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int FundSourceID { get; set; }
    }

    public class FundWithdrawData
    {
        public string AccountNumber { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int BankInfoID { get; set; }
    }
}