#region Header Information
/******************************************************************
 * File Name     : MarketingModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 21st March 2013
 * Modified Date : 21st March 2013
 * Description   : This file contains view model for Marketing view
 * ***************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models
{
    /// <summary>
    /// This class represents view model for Marketing view
    /// </summary>
    public class MarketingModel
    {
        public int ImageID { get; set; }
        public string ImageName { get; set; }
        public string Status { get; set; }
        public string Actions { get; set; }
    }
}