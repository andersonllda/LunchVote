using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace HMV.Core.Framework.Web
{
    public class AuthenticationExpiredModule : IHttpModule
    {
        private HttpApplication _app;

        public void Init(HttpApplication context)
        {  
            this._app = context;
            this._app.BeginRequest += new EventHandler(this._app_BeginRequest);
        }

        private void _app_BeginRequest(object sender, EventArgs e)
        {
          if (this._app.Request.Path != FormsAuthentication.LoginUrl)
            {
                HttpCookie authCookie =
                    this._app.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                {
                    FormsAuthenticationTicket ticket =
                        FormsAuthentication.Decrypt(authCookie.Value);

                    if (ticket.Expired)
                    {
                        this._app.CompleteRequest();
                        FormsAuthentication.RedirectToLoginPage("Authentication=Expired");
                    }
                }
            }
        }

        public void Dispose() { }
    }
}
