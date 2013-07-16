using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;

namespace CurrentDesk.WebAPI.Filters
{
    public class CommonFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An unhandled exception was thrown by Customer Web API controller."),
                ReasonPhrase = "An unhandled exception was thrown by Customer Web API controller."
            };
            context.Response = msg;
        }
    }
}