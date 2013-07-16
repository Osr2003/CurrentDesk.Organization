using CurrentDesk.BackOffice.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurrentDesk.BackOffice.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

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
