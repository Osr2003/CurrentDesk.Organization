using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Models
{
    public class TrustAccountReviewModel
    {
        //Trust Information

        public string TrustName { get; set; }
        public string TrusteeType { get; set; }
        public string TrustAddressLine1 { get; set; }
        public string TrustAddressLine2 { get; set; }
        public string TrustCity { get; set; }
        public string TrustCountry { get; set; }
        public string TrustPostalCode { get; set; }
        public string ClientAccountNumber { get; set; }
        public string PhoneID { get; set; }

        //Trustee Company Information
        public string TrusteeCompanyName { get; set; }
        public string TrusteeCompanyType { get; set; }
        public string TrusteeAddressLine1 { get; set; }
        public string TrusteeAddressLine2 { get; set; }
        public string TrusteeCity { get; set; }
        public string TrusteeCountry { get; set; }
        public string TrusteePostalCode { get; set; }

        //Trustee Company Authorized Officer Information
        public string AuthorizedOfficerTitle { get; set; }
        public string AuthorizedOfficerFirstName { get; set; }
        public string AuthorizedOfficerMiddleName { get; set; }
        public string AuthorizedOfficerLastName { get; set; }
        public string AuthorizedOfficerDobMonth { get; set; }
        public string AuthorizedOfficerDobDay { get; set; }
        public int AuthorizedOfficerDobYear { get; set; }
        public string AuthorizedOfficerGender { get; set; }
        public string AuthorizedOfficerCitizenship { get; set; }
        public string AuthorizedOfficerIdInfo { get; set; }
        public string AuthorizedOfficerIdNumber { get; set; }
        public string AuthorizedOfficerResidenceCountry { get; set; }

        //Trustee Company Authorized Officer Contact Information
        public string AuthorizedOfficerAddressLine1 { get; set; }
        public string AuthorizedOfficerAddressLine2 { get; set; }
        public string AuthorizedOfficerCity { get; set; }
        public string AuthorizedOfficerCountry { get; set; }
        public string AuthorizedOfficerPostalCode { get; set; }   
        public long AuthorizedOfficerTelCountryCode { get; set; }
        public long AuthorizedOfficerTelephoneNumber { get; set; }
        public long AuthorizedOfficerMobCountryCode { get; set; }
        public long AuthorizedOfficerMobileNumber { get; set; }
        public string AuthorizedOfficerEmailAddress { get; set; }
        public string AuthorizedOfficerConfirmEmailAddress { get; set; }

        //Trust Individual Information
        public string IndividualTitle { get; set; }
        public string IndividualFirstName { get; set; }
        public string IndividualMiddleName { get; set; }
        public string IndividualLastName { get; set; }
        public string IndividualDobMonth { get; set; }
        public string IndividualDobDay { get; set; }
        public int IndividualDobYear { get; set; }
        public string IndividualGender { get; set; }
        public string IndividualCitizenship { get; set; }
        public string IndividualIdInfo { get; set; }
        public string IndividualIdNumber { get; set; }
        public string IndividualResidenceCountry { get; set; }

        //Trust  Individual Contact Information
        public string IndividualResidentialAddLine1 { get; set; }
        public string IndividualResidentialAddLine2 { get; set; }
        public string IndividualResidentialCity { get; set; }
        public string IndividualResidentialCountry { get; set; }
        public string IndividualResidentialPostalCode { get; set; }       
        public int IndividualYearsInCurrentAdd { get; set; }
        public int IndividualMonthsInCurrentAdd { get; set; }
        public string IndividualPreviousAddLine1 { get; set; }
        public string IndividualPreviousAddLine2 { get; set; }
        public string IndividualPreviousCity { get; set; }
        public string IndividualPreviousCountry { get; set; }
        public string IndividualPreviousPostalCode { get; set; }
        public long IndividualTelNumberCountryCode { get; set; }
        public long IndividualTelNumber { get; set; }
        public long IndividualMobileNumberCountryCode { get; set; }
        public long IndividualMobileNumber { get; set; }
        public string IndividualEmailAddress { get; set; }
        public string IndividualConfirmEmailAddress { get; set; }

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

        public TrustAccountModel TrustModel { get; set; }
        public bool IsLive { get; set; }
    }
}