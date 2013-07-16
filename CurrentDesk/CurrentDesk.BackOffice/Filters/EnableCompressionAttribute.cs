#region Header Information
/**********************************************************************
 * File Name     : EnableCompressionAttribute.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 19th March 2013
 * Modified Date : 19th March 2013
 * Description   : This file contains new custom compression filter attribute
 * *******************************************************************/
#endregion

#region Namespace Used
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Filters
{
    /// <summary>
    /// This class represents custom compression filter attribute
    /// </summary>
    public class EnableCompressionAttribute : ActionFilterAttribute
    {
        const CompressionMode compress = CompressionMode.Compress;

        /// <summary>
        /// This method executes during any action execution
        /// and gzip response
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //HttpRequestBase request = filterContext.HttpContext.Request;
            //HttpResponseBase response = filterContext.HttpContext.Response;
            //string acceptEncoding = request.Headers["Accept-Encoding"];
            //if (acceptEncoding == null)
            //    return;
            //else if (acceptEncoding.ToLower().Contains("gzip"))
            //{
            //    response.Filter = new GZipStream(response.Filter, compress);
            //    response.AppendHeader("Content-Encoding", "gzip");
            //}
            //else if (acceptEncoding.ToLower().Contains("deflate"))
            //{
            //    response.Filter = new DeflateStream(response.Filter, compress);
            //    response.AppendHeader("Content-Encoding", "deflate");
            //}
        }
    }  
}