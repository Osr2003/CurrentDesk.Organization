/* **************************************************************
* File Name     :- CurrentDeskLog.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 7th Feb 2013
* Modified Date :- 7th Feb 2013
* Description   :- This file will instating Current desk Log and Add log to the database
****************************************************************/

#region Namespace

using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

#endregion


namespace CurrentDesk.Logging
{
    public static class CurrentDeskLog
    {
        #region " Constants "

        /// <summary>
        /// Log4Net Configuration file
        /// </summary>
        private const string LOG4NET_CONFIG_FILE = "{0}Configuration\\log4net.config";


        #endregion

        #region " Properties "

        /// <summary>
        /// Connection string for error logging
        /// </summary>
        internal static string ConnectionString { get; private set; }

        private static ILog Log
        {
            get
            {
                try
                {
                    MethodBase method = new StackTrace().GetFrame(2).GetMethod();
                    StringBuilder loggerName = new StringBuilder();
                    loggerName.AppendFormat("{0}.{1}(", method.ReflectedType.FullName, method.Name);

                    ParameterInfo[] parameters = method.GetParameters();
                    string[] parametersStr = new string[parameters.Length];

                    // Get all parameters name
                    if (parameters.Length > 0)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            parametersStr[i] = parameters[i].ToString();
                        }
                        loggerName.Append(String.Join(", ", parametersStr));
                    }

                    loggerName.Append(")");

                    return LogManager.GetLogger(loggerName.ToString());
                    //return LogManager.GetLogger(typeof(CurrentDeskLog));
                }
                catch
                {
                    return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
        }

        #endregion

        #region " Constructor "

        /// <summary>
        /// Initialize VivlioLog
        /// </summary>
        static CurrentDeskLog()
        {
            if (HttpRuntime.AppDomainAppId != null)
            {
                if (File.Exists(String.Format(LOG4NET_CONFIG_FILE, System.Web.HttpRuntime.BinDirectory)))
                {
                    // Set configuration file
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(
                            new FileInfo(String.Format(LOG4NET_CONFIG_FILE, System.Web.HttpRuntime.BinDirectory)));
                }
                else
                {
                    //Send Email
                }
            }
            else
            {
                var configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (!configPath.EndsWith("\\"))
                {
                    configPath = configPath + "\\";
                }

                if (File.Exists(String.Format(LOG4NET_CONFIG_FILE, configPath)))
                {
                    // Set configuration file
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(
                            new FileInfo(String.Format(LOG4NET_CONFIG_FILE, configPath)));
                }
                else
                {
                    //Send Email
                }
            }
        }

        #endregion

        #region " Functions "

        /// <summary>
        /// Log error for Debug
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Debug(object message, Exception exception = null)
        {
            if (exception == null)
            {
                Log.Debug(message);
            }
            else
            {
                Log.Debug(message, exception);
            }
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Error(object message, Exception exception = null)
        {
            if (exception == null)
            {
                Log.Error(message);
            }
            else
            {
                Log.Error(message, exception);
            }
        }

        /// <summary>
        /// Log fatal error
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Fatal(object message, Exception exception = null)
        {
            if (exception == null)
            {
                Log.Fatal(message);
            }
            else
            {
                Log.Fatal(message, exception);
            }
        }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Info(object message, Exception exception = null)
        {
            if (exception == null)
            {
                Log.Info(message);
            }
            else
            {
                Log.Info(message, exception);
            }
        }

        #endregion
    }
}
