#region Header Information
/********************************************************************************
 * File Name     : RequestsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th June 2013
 * Modified Date : 11th June 2013
 * Description   : This file contains Requests controller and related actions
 *                 methods for super admin login
 * ******************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.Common;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Requests controller and contains actions
    /// methods for super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class RequestsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
