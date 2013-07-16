#region Header Information
/******************************************************************************
 * File Name     : IntroducingBrokerProfitShareModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 29th April 2013
 * Modified Date : 29th April 2013
 * Description   : This file contains IntroducingBrokerProfitShareModel view model
 *                 for IntroducingBrokerProfitShare view of AM
 * ****************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.AssetManager.Models.Profile
{
    /// <summary>
    /// This class represents IntroducingBrokerProfitShareModel view model
    /// for IntroducingBrokerProfitShare view of AM
    /// </summary>
    public class IntroducingBrokerProfitShareModel
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public double IBPerformanceFeeShare { get; set; }
        public double IBManagemantFeeShare { get; set; }
        public double IBCommissionMarkupShare { get; set; }
        public double IBSpreadMarkupShare { get; set; }
        public bool IBMaskedOffering { get; set; }
    }
}