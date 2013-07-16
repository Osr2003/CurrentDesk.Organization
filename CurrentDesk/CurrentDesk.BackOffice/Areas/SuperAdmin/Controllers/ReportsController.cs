#region Header Information
/**********************************************************************
 * File Name     : ReportsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 10th June 2013
 * Modified Date : 10th June 2013
 * Description   : This file contains Reports controller and related
 *                 actions for super admin login
 * ********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.Common;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Reports controller and contains action
    /// methods related to super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
