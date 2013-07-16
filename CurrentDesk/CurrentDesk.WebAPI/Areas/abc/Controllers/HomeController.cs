using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurrentDesk.WebAPI.Areas.abc.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /abc/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
