#region Header Information
/*******************************************************************************
 * File Name     : ClientNoteHistoryModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 30th April 2013
 * Modified Date : 30th April 2013
 * Description   : This file contains view model for ClientNoteHistory view of IB
 * ****************************************************************************/
#endregion

#region Namespace Used
using System;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for ClientNoteHistory view of IB
    /// </summary>
    public class ClientNoteHistoryModel
    {
        public int ClientID { get; set; }
        public string AccountID { get; set; }
        public string ClientName { get; set; }
        public string PartnerDisplayName { get; set; }
    }

    public class ClientNoteDetails
    {
        public string Subject { get; set; }
        public string Note { get; set; }
        public string Timestamp { get; set; }
        public string TimestampDay { get; set; }
        public string TimestampLong { get; set; }
    }
}