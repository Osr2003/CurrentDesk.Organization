/* **************************************************************
* File Name     :- CustomAppender.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 7th Feb 2013
* Modified Date :- 7th Feb 2013
* Description   :- This file is a custom appender used with log4net for logging error
****************************************************************/



#region Namespace


using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using log4net.Appender;
using System;
using System.Web;

#endregion

namespace CurrentDesk.Logging
{
    /// <summary>
    /// This Interface id dervied from log4Net Iappender
    /// </summary>
    internal class CustomAppender : IAppender
    {
        #region Property

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        #endregion

        #region Methods
        public void Close()
        {
           
        }        

        /// <summary>
        /// Implementation of IAppender to Add the error to the database.
        /// </summary>
        /// <param name="loggingEvent"></param>
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            var httpCtx = HttpContext.Current;
            var req = httpCtx == null ? null : httpCtx.Request;

            var httpEx = loggingEvent.ExceptionObject as HttpException;

            // Skip errors which are less then 500 error codes
            if (httpEx != null && httpEx.GetHttpCode() < 500)
            {
                return;
            }

            //Create a new log
            ErrorLog newError = new ErrorLog()
            {
                LoggedDate = loggingEvent.TimeStamp,
                Level = loggingEvent.Level.Name,
                Logger = loggingEvent.LoggerName,
                Message = loggingEvent.RenderedMessage,
                Thread = loggingEvent.ThreadName,
                Exception = loggingEvent.GetExceptionString(),
                ServerName = httpCtx == null ? Environment.MachineName : httpCtx.Server.MachineName ?? Environment.MachineName,
                IpAddress = req == null ? null : req.UserHostAddress,
                Url = req == null ? null : req.RawUrl,
                Referrer = req == null ? null : req.UrlReferrer == null ? null : req.UrlReferrer.OriginalString,
                UserName = httpCtx == null ? null : httpCtx.User == null ? null : httpCtx.User.Identity.Name,
                UserAgent = req == null ? null : req.UserAgent
            };

            try
            {
                var errorLogBO = new ErrorLogBO();
                errorLogBO.AddNewError(newError);
            }
            catch (Exception)
            {
                // TODO: Send email
                throw;
            }
        }

        #endregion
    }
}
