#region Header Information
/*************************************************************************
 * File Name     : ErrorController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 1st April 2013
 * Modified Date : 1st April 2013
 * Description   : This file contains error controller and related actions
 * ***********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models.Error;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This class represents error controller
    /// and contains related actions
    /// </summary>
    public class ErrorController : Controller
    {
        public ActionResult Index(ErrorModel errorModel)
        {
            return View(errorModel);
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult ErrorMessage()
        {
            return View();
        }
    }
}
