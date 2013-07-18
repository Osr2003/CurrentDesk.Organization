#region Header Information
/***********************************************************************
 * File Name     : ReportsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 7th May 2013
 * Modified Date : 7th May 2013
 * Description   : This file contains Reports controller and related actions
 *                 to handle different reporting functionality of Traders
 * *********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Logging;
using System;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This class represents Reports controller and contains related
    /// actions to handle reporting functionality of Traders
    /// </summary>
    [AuthorizeTrader, NoCache]
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        public ActionResult ReportGroups()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

    }
}
