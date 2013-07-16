#region Header Information
/***************************************************************************
 * File Name     : ClientActivityModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 8th May 2013
 * Modified Date : 8th May 2013
 * Description   : This file contains view model for Client Activity view
 * *************************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for Client Activity view
    /// </summary>
    public class ClientActivityModel
    {
        public int ClientID { get; set; }
        public string AccountID { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientActivity
    {
        public string ActivityTimestamp { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDetails { get; set; }
    }
}