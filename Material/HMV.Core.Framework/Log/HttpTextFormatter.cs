using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
//using System.Web.Security;

using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace HMV.Core.Framework.Log
{
    /// <summary>
    /// Represents a template based formatter for <see cref="LogEntry"/> messages with additional HTTP context information.
    /// Uses the default TextFormatter template
    /// </summary>	
    [ConfigurationElementType(typeof(CustomFormatterData))]
    public class HttpTextFormatter : TextFormatter
    {
        public HttpTextFormatter(NameValueCollection attributes) : base()
        {}

        /// <summary>
        /// Adds some HTTP context items, then calls the base class to do the formatting.
        /// </summary>
        /// <param name="log">Log entry to format</param>
        /// <returns>Formatted log entry</returns>
        public override string Format(Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry log)
        {
            // Be VERY careful not to do anything clever in here.  eg if you try to access the DB,
            // but DB access is broken you could end up in an infinite loop of logging!
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                log.ExtendedProperties.Add("Url", request.Url.ToString());
                log.ExtendedProperties.Add("UserHostAddress", request.UserHostAddress);

                foreach (string key in request.Headers.AllKeys)
                {
                    log.ExtendedProperties.Add(String.Format("Request header '{0}'", key), request.Headers[key]);
                }

                IPrincipal user = HttpContext.Current.User;

                if (user != null)
                {
                    log.ExtendedProperties.Add("User name", user.Identity.Name);
                }
            }
            catch
            { }

            return base.Format(log);
        }
    }
}
