#region Header Information
/********************************************************************
 * File Name     : CommonErrorLogger.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 19th Feb 2013
 * Modified Date : 19th Feb 2013
 * Description   : This file contains common error log function
 *                 to log BO exceptions
 * ******************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using System;
#endregion

namespace CurrentDesk.Repository
{
    /// <summary>
    /// This class contains common error log function 
    /// to log BO exceptions
    /// </summary>
    public class CommonErrorLogger
    {
        /// <summary>
        /// This method logs exceptions in database
        /// </summary>
        /// <param name="ex">ex</param>
        /// <param name="methodName">methodName</param>
        public static void CommonErrorLog(Exception ex, string methodName)
        {
            try
            {
                ErrorLog log = new ErrorLog();
                log.Message = ex.Message;
                log.Exception = ex.ToString();
                log.LoggedDate = DateTime.Now;
                log.Logger = methodName;

                ErrorLogBO errorLogBO = new ErrorLogBO();
                errorLogBO.AddNewError(log);
            }
            catch (Exception exception)
            {
               //Send Mail
            }
        }

        /// <summary>
        /// Info Message
        /// </summary>
        /// <param name="info"></param>
        /// <param name="methodName"></param>
        public static void LogInfo(string info, string methodName)
        {
            try
            {
                ErrorLog log = new ErrorLog();
                log.Message = info;
                log.Exception = "INFO";
                log.LoggedDate = DateTime.Now;
                log.Logger = methodName;

                ErrorLogBO errorLogBO = new ErrorLogBO();
                errorLogBO.AddNewError(log);
            }
            catch (Exception exception)
            {
              //Send mail
            }
        }

    }
}
