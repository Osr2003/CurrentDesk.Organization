using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Models
{
    public class CorporateAccountReviewModel
    {       
        
        public string CompanyName { get; set; }        
        public string  CompanyType { get; set; }        
        public string CompanyAddLine1 { get; set; }
        public string CompanyAddLine2 { get; set; }        
        public string CompanyCity { get; set; }        
        public string  CompanyCountry { get; set; }        
        public string CompanyPostalCode { get; set; }
        public string ClientAccountNumber { get; set; }
        public string PhoneID { get; set; }

        //Authorized Officer        
        public string AuthOfficerTitle { get; set; }        
        public string AuthOfficerFirstName { get; set; }
        public string AuthOfficerMiddleName { get; set; }        
        public string AuthOfficerLastName { get; set; }        
        public string AuthOfficerDobMonth { get; set; }
        public string AuthOfficerDobDay { get; set; }        
        public int AuthOfficerDobYear { get; set; }
        public string AuthOfficerGender { get; set; }
        public string AuthOfficerCitizenship { get; set; }
        public string AuthOfficerIdInfo { get; set; }        
        public string AuthOfficerIdNumber { get; set; }        
        public string  AuthOfficerResidenceCountry { get; set; }

        //Authorized Officer Contact Information        
        public string AuthOfficerAddLine1 { get; set; }
        public string AuthOfficerAddLine2 { get; set; }        
        public string AuthOfficerCity { get; set; }        
        public string AuthOfficerCountry { get; set; }        
        public string AuthOfficerPostalCode { get; set; } 
        public long AuthOfficerTelNumberCountryCode { get; set; } 
        public long AuthOfficerTelNumber { get; set; }
        public long AuthOfficerMobileNumberCountryCode { get; set; }
        public long AuthOfficerMobileNumber { get; set; }
        public string AuthOfficerEmailAddress { get; set; }
        public string AuthOfficerConfirmEmailAddress { get; set; }

        //Banking Information       
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

        public CorporateAccountModel CorporateModel { get; set; }
        public bool IsLive { get; set; }
    }
}