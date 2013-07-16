#region Header Information
/***************************************************************************
 * File Name     : IBClientsModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 26th March 2013
 * Modified Date : 26th March 2013
 * Description   : This file contains view model for MyClients view of IB
 * *************************************************************************/
#endregion

#region Namespace Used
#endregion
namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients
{
    /// <summary>
    /// This class represents view model for MyClients view of IB
    /// </summary>
    public class IBClientsModel
    {
        public int PK_ClientID { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string AccountID { get; set; }
        public string Activity { get; set; }
        public string Status { get; set; }
    }
}