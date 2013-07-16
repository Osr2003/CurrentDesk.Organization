using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Models
{
    public class JointAccountReviewModel
    {        
        public string PrimaryAccountHolderTitle { get; set; }        
        public string PrimaryAccountHolderFirstName { get; set; }
        public string PrimaryAccountHolderMiddleName { get; set; }        
        public string PrimaryAccountHolderLastName { get; set; }        
        public string PrimaryAccountHolderDobMonth { get; set; }
        public string PrimaryAccountHolderDobDay { get; set; }        
        public int PrimaryAccountHolderDobYear { get; set; }        
        public string PrimaryAccountHolderGender { get; set; }
        public string PrimaryAccountHolderCitizenship { get; set; }
        public string PrimaryAccountHolderIdInfo { get; set; }        
        public string PrimaryAccountHolderIdNumber { get; set; }
        public string PrimaryAccountHolderResidenceCountry { get; set; }
        public string ClientAccountNumber { get; set; }
        public string PhoneID { get; set; }

        public string SecondaryAccountHolderTitle { get; set; }        
        public string SecondaryAccountHolderFirstName { get; set; }
        public string SecondaryAccountHolderMiddleName { get; set; }        
        public string SecondaryAccountHolderLastName { get; set; }
        public string SecondaryAccountHolderDobMonth { get; set; }
        public string SecondaryAccountHolderDobDay { get; set; }        
        public int SecondaryAccountHolderDobYear { get; set; }        
        public string SecondaryAccountHolderGender { get; set; }        
        public string  SecondaryAccountHolderCitizenship { get; set; }        
        public string SecondaryAccountHolderIdInfo { get; set; }        
        public string SecondaryAccountHolderIdNumber { get; set; }        
        public string SecondaryAccountHolderResidenceCountry { get; set; }  
 
        public string PrimaryAccountHolderResidentialAddLine1 { get; set; }
        public string PrimaryAccountHoldeResidentialAddLine2 { get; set; }        
        public string PrimaryAccountHoldeResidentialCity { get; set; }        
        public string  PrimaryAccountHoldeResidentialCountry { get; set; }        
        public string PrimaryAccountHoldeResidentialPostalCode { get; set; }  
        public int PrimaryAccountHoldeYearsInCurrentAdd { get; set; }  
        public int PrimaryAccountHoldeMonthsInCurrentAdd { get; set; }
        public string PrimaryAccountHoldePreviousAddLine1 { get; set; }
        public string PrimaryAccountHoldePreviousAddLine2 { get; set; }       
        public string PrimaryAccountHoldePreviousCity { get; set; }        
        public string PrimaryAccountHoldePreviousCountry { get; set; }        
        public string PrimaryAccountHoldePreviousPostalCode { get; set; } 
        public long PrimaryAccountHoldeTelNumberCountryCode { get; set; }      
        public long PrimaryAccountHoldeTelNumber { get; set; }       
        public long PrimaryAccountHoldeMobileNumberCountryCode { get; set; }
        public long PrimaryAccountHoldeMobileNumber { get; set; }
        public string PrimaryAccountHoldeEmailAddress { get; set; }
        public string PrimaryAccountHoldeConfirmEmailAddress { get; set; }

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

        public JointAccountModel JointModel { get; set; }
        public bool IsLive { get; set; }
    }
}