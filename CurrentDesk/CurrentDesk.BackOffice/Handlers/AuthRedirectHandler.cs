using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Handlers
{
    public class AuthRedirectHandler : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += Context_EndRequest;
        }

        void Context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if (app.Response.StatusCode == 302)
            {
                app.Response.ClearHeaders();
                app.Response.ClearContent();
                app.Response.StatusCode = 401;
            }
        }
    }
}