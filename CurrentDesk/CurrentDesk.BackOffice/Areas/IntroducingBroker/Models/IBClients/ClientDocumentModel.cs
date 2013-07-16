#region Header Information
/************************************************************************
 * File Name     : ClientDocumentModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 5th April 2013
 * Modified Date : 5th April 2013
 * Description   : This file view model for ClientDocuments view in MyClients
 * *********************************************************************/
#endregion

#region Namespace Used
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for ClientDocuments
    /// view in MyClients section of IB login
    /// </summary>
    public class ClientDocumentModel
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string Status { get; set; }
        
        public string ClientName { get; set; }
        public int ClientID { get; set; }
        public string AccountID { get; set; }
    }
}