#region Header Information
/************************************************************************
 * File Name     : InboxModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 1st May 2013
 * Modified Date : 1st May 2013
 * Description   : This file contains view model for Inbox view
 * **********************************************************************/
#endregion

#region Namespace Used
using System;
#endregion

namespace CurrentDesk.BackOffice.Models.Inbox
{
    /// <summary>
    /// This class represents view model for Inbox view
    /// </summary>
    public class InboxModel
    {
        public int PK_MessageID { get; set; }
        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }
        public string Timestamp { get; set; }
        public string FromUserName { get; set; }
        public bool IsRead { get; set; }
        public string LongDateString { get; set; }
        public DateTime MessageTime { get; set; }
    }
}