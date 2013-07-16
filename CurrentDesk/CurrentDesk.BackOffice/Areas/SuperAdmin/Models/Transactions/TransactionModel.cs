#region Header Information
/**************************************************************************
 * File Name     : TransactionModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 14th June 2013
 * Modified Date : 14th June 2013
 * Description   : This file contains view model for Transaction section
 *                 of super admin
 * ************************************************************************/
#endregion

#region Namespace Used
#endregion

using System.Collections.Generic;
using CurrentDesk.Models;
namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Models.Transactions
{
    /// <summary>
    /// This clas represents view model for Transactions section
    /// of super admin
    /// </summary>
    public class TransactionModel
    {
        public int PK_TransactionID { get; set; }
        public string TransactionDate { get; set; }
        public int TransactionID { get; set; }
        public string AccountNumber { get; set; }
        public string ClientName { get; set; }
        public string FundingSourceName { get; set; }
        public string Currency { get; set; }
        public string FeeCurrency { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionFee { get; set; }
        public string Notes { get; set; }
        public string WithdrawSource { get; set; }
        public string Actions { get; set; }
        public string ToAccount { get; set; }
        public string ToClientName { get; set; }
        public double ExchangeRate { get; set; }
        public string ToCurrency { get; set; }
        public string ExchangedAmount { get; set; }
    }

    public class NewTransactionModel
    {
        public int FundingSourceID { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public int ClientUserID { get; set; }
        public string ClientAccountNumber { get; set; }
        public string Notes { get; set; }
        public string ClientName { get; set; }
        public int BankID { get; set; }
        public string AdminPassword { get; set; }
    }

    public class LandingAccountDetails
    {
        public string LandingCurrency { get; set; }
        public string LandingAccount { get; set; }
        public string LandingBalance { get; set; }
        public bool IsLanding { get; set; }
    }

    public class BankInfoDetails
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BicOrSwiftCode { get; set; }
        public string ReceivingBankInfo { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankCountry { get; set; }
        public string BankPostalCode { get; set; }
    }

    public class OutgoingTransactionApproveModel
    {
        public int PK_TransactionID { get; set; }
        public string ClientName { get; set; }
        public string Currency { get; set; }
        public string FeeCurrency { get; set; }
        public string Notes { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BicOrSwiftCode { get; set; }
        public string ReceivingBankInfo { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankCountry { get; set; }
        public string BankPostalCode { get; set; }

        public decimal TransactionAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public string TotalAmount { get; set; }
        public int FundSourceID { get; set; }
        public string AdminPassword { get; set; }
    }

    public class BankAccountDetails
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
    }

    public class NewTransferTransaction
    {
        public int FromClientUserID { get; set; }
        public int ToClientUserID { get; set; }
        public string FromClientAccount { get; set; }
        public string ToClientAccount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TransactionFee { get; set; }
        public string Notes { get; set; }
        public string AdminPassword { get; set; }
        public string FromClientName { get; set; }
        public string ToClientName { get; set; }
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
        public string ToCurrency { get; set; }
    }

    public class TradingAccountGrouped
    {
        public string TradingCurrency { get; set; }
        public List<LandingAccountDetails> TradingAccountList { get; set; }
    }
}