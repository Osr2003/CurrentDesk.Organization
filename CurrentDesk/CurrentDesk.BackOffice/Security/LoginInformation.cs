#region Header Information
/********************************************************************************
 * File Name     :- SessionManagement.cs
 * Author        :- Mukesh Nayak
 * Copyright     :- Mindfire Solutions 
 * Date          :- 9th Feb 2013
 * Modified Date :- 9th Feb 2013
 * Description   :- This file contains session repository
 * ******************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.Common;
#endregion

namespace CurrentDesk.BackOffice.Security
{
    /// <summary>
    /// This class represents session repository
    /// </summary>
    public class LoginInformation
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public int AccountType { get; set; }//Ind/Joint/Corp/Trust
        public int AccountCode { get; set; }//Trading/Managed/IB/AM
        public LoginAccountType LogAccountType { get; set; }//Live/Partner
    }
}