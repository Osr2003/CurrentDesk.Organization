using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Models
{
    public class BankAccountModel
    {
        public int BankAccountID { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BicOrSwiftCode { get; set; }
        public string BankAddLine1 { get; set; }
        public string BankAddLine2 { get; set; }
        public string ReceivingBankInfoId { get; set; }
        public string ReceivingBankInfo { get; set; }
        public string BankCity { get; set; }
        public string BankCountry { get; set; }
        public string BankPostalCode { get; set; }
        public int ClientID { get; set; }
    }
}