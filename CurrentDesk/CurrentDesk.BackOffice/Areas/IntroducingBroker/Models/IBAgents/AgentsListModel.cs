#region Header Information
/*************************************************************************
 * File Name     : AgentsListModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 15th April 2013
 * Modified Date : 15th April 2013
 * Description   : This file contains view model for Agents List view in MyAgents section
 * ***********************************************************************/
#endregion

#region Namespace Used
using System.Collections.Generic;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBAgents
{
    /// <summary>
    /// This class represents view model for Agents List view in MyAgents section
    /// </summary>
    public class AgentsListModel
    {
        public int CountryID { get; set; }
        public int ReceivingBankInfoID { get; set; }
    }

    public class AgentInfo
    {
        public string AgentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Action { get; set; }
    }
}