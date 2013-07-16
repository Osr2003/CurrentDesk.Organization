#region Header Information
/**************************************************************
 * File Name     : DashboardModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 15th March 2013
 * Modified Date : 15th March 2013
 * Description   : This file contains model for Dashboard view
 * ************************************************************/
#endregion

#region NamespaceUsed
using CurrentDesk.Models;
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Models.Dashboard
{
    /// <summary>
    /// This class represents model for Dashboard view
    /// </summary>
    public class DashboardModel
    {
        public List<UserAccountGrouped> UserAccInformation { get; set; }
        public List<MarketNewsDataModel> MarketNews { get; set; }
        public string BrokerPromoImgName { get; set; }
    }

    public class UserAccountGrouped
    {
        public string AccountCurrency { get; set; }
        public List<Client_Account> UserAccountList { get; set; }
    }

    public class MarketNewsDataModel
    {
        public string NewsDateTime { get; set; }
        public string Currency { get; set; }
        public string Title { get; set; }
        public string Impact { get; set; }
    }
}