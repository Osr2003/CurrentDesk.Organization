#region Header Information
/****************************************************************************
 * File Name     : AuthorizeRoles.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 26th June 2013
 * Modified Date : 26th June 2013
 * Description   : This file contains custom authorize attribute to authorize
 *                 users based upon their roles
 * **************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Logging;
using System;
using System.Web;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Custom
{
    /// <summary>
    /// This class represents custom attribute to authorize
    /// users based upon their roles
    /// </summary>
    public class AuthorizeRoles : AuthorizeAttribute
    {
        public int AccountCode { get; set; }

        /// <summary>
        /// This methods compares session account code with parameter
        /// passed from controller to authorize roles
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                if (SessionManagement.UserInfo.AccountCode == AccountCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return false;
            }
        }
    }
}