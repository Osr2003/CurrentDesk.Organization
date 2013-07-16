#region Header Information
/*****************************************************************************
 * File Name     : InboxController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 10th June 2013 
 * Modified Date : 10th June 2013
 * Description   : This file contains Inbox controller and related action
 *                 methods for super admin login
 * ***************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.Common;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Inbox controller and contains action
    /// methods for super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class InboxController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
