//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace TestingDAL
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(AccountType))]
    [KnownType(typeof(L_Account))]
    [KnownType(typeof(L_CommissionIncrementValue))]
    [KnownType(typeof(L_Country))]
    [KnownType(typeof(L_RecievingBank))]
    [KnownType(typeof(L_TradingExperience))]
    [KnownType(typeof(TradingPlatform))]
    [KnownType(typeof(L_WidenSpreadsValue))]
    [KnownType(typeof(CorporateAccountInformation))]
    [KnownType(typeof(IndividualAccountInformation))]
    [KnownType(typeof(JointAccountInformation))]
    [KnownType(typeof(TrustAccountInformation))]
    public partial class Client
    {
        public Client()
        {
            this.CorporateAccountInformations = new HashSet<CorporateAccountInformation>();
            this.IndividualAccountInformations = new HashSet<IndividualAccountInformation>();
            this.JointAccountInformations = new HashSet<JointAccountInformation>();
            this.TrustAccountInformations = new HashSet<TrustAccountInformation>();
        }
    
        [DataMember]
        public int PK_ClientID { get; set; }
        [DataMember]
        public Nullable<int> FK_AccountID { get; set; }
        [DataMember]
        public Nullable<int> FK_AccountTypeID { get; set; }
        [DataMember]
        public Nullable<int> FK_TradingPlatformID { get; set; }
        [DataMember]
        public Nullable<int> FK_WidenSpreadsID { get; set; }
        [DataMember]
        public Nullable<int> FK_CommissionIncrementID { get; set; }
        [DataMember]
        public string BankName { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string BicNumberOrSwiftCode { get; set; }
        [DataMember]
        public string BankingAddress { get; set; }
        [DataMember]
        public Nullable<int> FK_ReceivingBankInformationID { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public Nullable<int> FK_CountryID { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string EstimatedAnnualIncome { get; set; }
        [DataMember]
        public string LiquidAssets { get; set; }
        [DataMember]
        public string NetWorthInEuros { get; set; }
        [DataMember]
        public Nullable<int> FK_TradingSecurityExperienceID { get; set; }
        [DataMember]
        public Nullable<int> FK_TradingOptionExperienceID { get; set; }
        [DataMember]
        public Nullable<int> FK_TradingForeignExchangeExperienceID { get; set; }
        [DataMember]
        public Nullable<bool> IsHavingAccount { get; set; }
        [DataMember]
        public Nullable<bool> IsRegisterdWithEntity { get; set; }
        [DataMember]
        public Nullable<bool> IsRegisteredWithRegulator { get; set; }
        [DataMember]
        public Nullable<bool> IsEmployeeOfExchangeOrRegulator { get; set; }
        [DataMember]
        public Nullable<bool> IsDeclaredBankruptcy { get; set; }
    
        [DataMember]
        public virtual AccountType AccountType { get; set; }
        [DataMember]
        public virtual L_Account L_Account { get; set; }
        [DataMember]
        public virtual L_CommissionIncrementValue L_CommissionIncrementValue { get; set; }
        [DataMember]
        public virtual L_Country L_Country { get; set; }
        [DataMember]
        public virtual L_RecievingBank L_RecievingBank { get; set; }
        [DataMember]
        public virtual L_TradingExperience L_TradingExperience { get; set; }
        [DataMember]
        public virtual L_TradingExperience L_TradingExperience1 { get; set; }
        [DataMember]
        public virtual TradingPlatform TradingPlatform { get; set; }
        [DataMember]
        public virtual L_TradingExperience L_TradingExperience2 { get; set; }
        [DataMember]
        public virtual L_WidenSpreadsValue L_WidenSpreadsValue { get; set; }
        [DataMember]
        public virtual ICollection<CorporateAccountInformation> CorporateAccountInformations { get; set; }
        [DataMember]
        public virtual ICollection<IndividualAccountInformation> IndividualAccountInformations { get; set; }
        [DataMember]
        public virtual ICollection<JointAccountInformation> JointAccountInformations { get; set; }
        [DataMember]
        public virtual ICollection<TrustAccountInformation> TrustAccountInformations { get; set; }
    }
    
}
