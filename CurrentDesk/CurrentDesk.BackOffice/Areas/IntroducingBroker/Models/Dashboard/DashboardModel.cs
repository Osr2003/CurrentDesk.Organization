#region Header Information
/***************************************************************************
 * File Name     : DashboardModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 8th April 2013
 * Modified Date : 8th April 2013
 * Description   : This file contains DashboardModel viewmodel for IB
 *                 Dashboard page
 * *************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models.Dashboard;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.Dashboard
{
    /// <summary>
    /// This class represents view model for IB Dashboard page
    /// </summary>
    public class IBDashboardModel
    {
        public List<MarketNewsDataModel> MarketNews { get; set; }
        public List<RebateAccount> RebateAccDetails { get; set; }
        public Dictionary<int, bool> PendingStatus { get; set; }
        public Dictionary<int, bool> MissingStatus { get; set; }
        public Dictionary<int, bool> ApprovedStatus { get; set; }
        public Dictionary<int, bool> DeniedStatus { get; set; }
        public Dictionary<int, bool> NewStatus { get; set; }
        public Dictionary<int, bool> ActiveStatus { get; set; }
        public Dictionary<int, bool> InactiveStatus { get; set; }
        public Dictionary<int, bool> DormantStatus { get; set; }
    }

    public class RebateAccount
    {
        public string RebateAccCurrency { get; set; }
        public string RebateAccNumber { get; set; }
        public string Equity { get; set; }
    }

    //public class MarketNewsDataModel
    //{
    //    public string NewsDateTime { get; set; }
    //    public string Currency { get; set; }
    //    public string Title { get; set; }
    //    public string Impact { get; set; }
    //}

    public class TradesVolume
    {
        public string Day { get; set; }
        public int Volume { get; set; }
    }
}