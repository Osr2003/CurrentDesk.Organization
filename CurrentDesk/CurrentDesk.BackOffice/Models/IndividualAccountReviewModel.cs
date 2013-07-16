using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Models
{
    public class IndividualAccountReviewModel
    {       
        public string Title { get; set; }        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }        
        public string LastName { get; set; }       
        public string DobMonth { get; set; }
        public string DobDay { get; set; }        
        public int DobYear { get; set; }        
        public string  Gender { get; set; }        
        public string  Citizenship { get; set; }        
        public string  IdInfo { get; set; }       
        public string IdNumber { get; set; }
        public string ResidenceCountry { get; set; }
        public string ClientAccountNumber { get; set; }
        public string PhoneID { get; set; }
        public string ResidentialAddLine1 { get; set; }
        public string ResidentialAddLine2 { get; set; }
        public string ResidentialCity { get; set; }
        public string ResidentialCountry { get; set; }
        public string ResidentialPostalCode { get; set; }
        public int YearsInCurrentAdd { get; set; }        
        public int MonthsInCurrentAdd { get; set; }
        public string PreviousAddLine1 { get; set; }
        public string PreviousAddLine2 { get; set; }
        public string PreviousCity { get; set; }
        public string  PreviousCountry { get; set; }
        public string PreviousPostalCode { get; set; }
        public long TelNumberCountryCode { get; set; }
        public long TelNumber { get; set; }
        public long MobileNumberCountryCode { get; set; }
        public long MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }

        //Bank Information
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

        //List Of Bank Account
        public List<BankAccountModel> BankAccountModelList { get; set; }

        //Financial Information
        public string EstimatedAnnualIncome { get; set; }
        public string LiquidAssets { get; set; }
        public string NetWorthEuros { get; set; }

        //Trading Experience
        public string DrpHaveExperienceTradingSecurities { get; set; }
        public string DrpHaveExperienceTradingOptions { get; set; }
        public string DrpHaveExperienceTradingForeignExchange { get; set; }

        //Other Information
        public string HaveAccWithFqSecurities { get; set; }
        public string RequiredToBeRegisteredWithRegulator { get; set; }
        public string EverDeclaredBankruptcy { get; set; }
        public string RegisteredPerson { get; set; }
        public string EmployeeOfExchangeOrRegulatorOperator { get; set; }

        public IndividualAccountModel IndividualModel { get; set; }
        public bool IsLive { get; set; }
    }
}