#region Header Information
/***********************************************************************
 * File Name     : UserActivityModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 13th May 2013
 * Modified Date : 13th May 2013
 * Description   : This file contains view model for UserActivity list
 * *********************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Models.Dashboard
{
    /// <summary>
    /// This class represents model for UserActivity list
    /// </summary>
    public class UserActivityModel
    {
        public string ActivityTimestamp { get; set; }
        public string ActivityDetails { get; set; }
        public bool IsSeen { get; set; }
    }
}