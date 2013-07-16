#region Header Information
/******************************************************************************
 * File Name     : AgentReferralFeesModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 16th April 2013
 * Modified Date : 16th April 2013
 * Description   : This file contains view model for AgentReferralFees view
 * ****************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBAgents
{
    /// <summary>
    /// This class represents view model for AgentReferralFees view
    /// </summary>
    public class AgentReferralFeesModel
    {
        public int PerformanceFee { get; set; }
        public int ManagementFee { get; set; }
        public int RebatePercentage { get; set; }

        public int AgentID { get; set; }
        public string AgentName { get; set; }
    }
}