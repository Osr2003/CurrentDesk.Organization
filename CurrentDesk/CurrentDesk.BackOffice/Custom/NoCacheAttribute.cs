#region Header Information
/*************************************************************************
 * File Name     : NoCacheAttribute.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 10th June 2013
 * Modified Date : 10th June 2013
 * Description   : This file contains custom filter attribute to clear all
 *                 cache before ActionResult is executed
 * **********************************************************************/
#endregion

#region Namespace Used
using System;
using System.Web;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Custom
{
    /// <summary>
    /// This class represents custom attribute to clear all
    /// cache before ActionResult is executed
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// This method is called before ActionResult is executed
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}