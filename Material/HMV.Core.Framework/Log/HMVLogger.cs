using System.Diagnostics;
using HMV.Core.Framework.Extensions;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using OmarALZabir.AspectF;

namespace HMV.Core.Framework.Log
{
    public class HMVLogger : ILogger
    {

        #region ILogger Members

        public void Log(string message)
        {
            if (Logger.IsLoggingEnabled())
            {
                var newLogEntry = new LogEntry
                {
                    Message = message,
                    Severity = TraceEventType.Information
                };
                               
                if (Logger.ShouldLog(newLogEntry))
                    Logger.Write(newLogEntry);
               
            }
        }

        public void Log(string[] categories, string message)
        {
            if (Logger.IsLoggingEnabled())
            {
                var newLogEntry = new LogEntry
                {
                    Message = message,
                    Severity = TraceEventType.Information
                };
                categories.Each((category) => newLogEntry.Categories.Add(category));

                if (Logger.ShouldLog(newLogEntry))
                    Logger.Write(newLogEntry);
            }
        }

        #endregion

        #region ILogger Members

        public void LogException(System.Exception x)
        {
            System.Exception outException;
            bool rethrow = ExceptionPolicy.HandleException(x, "Log Only", out outException);
            if (rethrow)
                throw outException;
        }
        #endregion
    }
}
