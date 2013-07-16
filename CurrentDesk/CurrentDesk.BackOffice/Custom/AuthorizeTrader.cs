#region Header Information
/****************************************************************************
 * File Name     : AuthorizeRoles.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 4th July 2013
 * Modified Date : 4th July 2013
 * Description   : This file contains custom authorize attribute to authorize
 *                 trader
 * **************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using System;
using System.Web;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Custom
{
    /// <summary>
    /// This class represents custom attribute to authorize
    /// traders
    /// </summary>
    public class AuthorizeTrader : AuthorizeAttribute
    {
        /// <summary>
        /// This methods compares session account code with constant values
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                if (SessionManagement.UserInfo.AccountCode == Constants.K_MANAGED_ACCOUNT || SessionManagement.UserInfo.AccountCode == Constants.K_TRADING_ACCOUNT)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return false;
            }
        }
    }
}