#region Header Information
/**************************************************************************
 * File Name     : ManagedAccountProgramModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 25th April 2013
 * Modified Date : 25th April 2013
 * Description   : This file contains view model for ManagedAccountProgram view
 * ************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.AssetManager.Models.Profile
{
    /// <summary>
    /// This class represents viewmodel for ManagedAccountProgram view
    /// </summary>
    public class ManagedAccountProgramModel
    {
        public string ProgramName { get; set; }
        public decimal MinimumDeposit { get; set; }
        public int CurrencyID { get; set; }
        public int FeeGroupID { get; set; }
        public int PlatformID { get; set; }
        public int PerformanceFeePeriod { get; set; }
        public int ManagementFeePeriod { get; set; }
        public int DepositAcceptance { get; set; }
        public float MinimumStopOutLevel { get; set; }
        public float PerformanceFee { get; set; }
        public float ManagementFee { get; set; }
    }

    public class ManagedAccountProgramDetail
    {
        public int PK_ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string MinimumDeposit { get; set; }
        public string Currency { get; set; }
        public string Platform { get; set; }
        public string PerformanceFeePeriod { get; set; }
        public string ManagementFeePeriod { get; set; }
        public string DepositAcceptance { get; set; }
        public float MinimumStopOutLevel { get; set; }
        public float PerformanceFee { get; set; }
        public float ManagementFee { get; set; }
        public string Action { get; set; }
    }
}