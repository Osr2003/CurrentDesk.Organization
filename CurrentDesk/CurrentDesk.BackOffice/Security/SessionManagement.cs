/* **************************************************************
* File Name     :- SessionManagement.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 9th Feb 2013
* Modified Date :- 9th Feb 2013
* Description   :- This file will do all the session Management
****************************************************************/

#region Namespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#endregion

namespace CurrentDesk.BackOffice.Security
{
    /// <summary>
    /// Static Class to set and get all the session object
    /// </summary>
    public static class SessionManagement
    {
        /// <summary>
        /// This will store login Information
        /// </summary>
        public static LoginInformation UserInfo
        {
            get
            {
                if (HttpContext.Current.Session["UserInfo"] != null)
                {
                    return (LoginInformation)HttpContext.Current.Session["UserInfo"];
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["UserInfo"] = value;
            }
        }

        /// <summary>
        /// This Will Store User Email
        /// </summary>
        public static string UserEmail
        {
            get
            {
                if (HttpContext.Current.Session["UserEmail"] != null)
                {
                    return HttpContext.Current.Session["UserEmail"].ToString();
                }
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["UserEmail"] = value;
            }
        }

        /// <summary>
        /// This Will store Image URL
        /// </summary>
        public static string ImageURL
        {
            get
            {
                if (HttpContext.Current.Session["ImageURl"] != null)
                {
                    return HttpContext.Current.Session["ImageURl"].ToString();
                }
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ImageURl"] = value;
            }
        }

        /// <summary>
        /// This Will Store User DisplayName
        /// </summary>
        public static string DisplayName
        {
            get
            {
                if (HttpContext.Current.Session["DisplayName"] != null)
                {
                    return HttpContext.Current.Session["DisplayName"].ToString();
                }
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["DisplayName"] = value;
            }
        }


        public static bool IsLoginAuthenticated
        {
            get
            {
                if (HttpContext.Current.Session["IsLoginAuthenticated"] != null)
                {
                    return (bool)HttpContext.Current.Session["IsLoginAuthenticated"];
                }
                return false;
            }
            set
            {
                HttpContext.Current.Session["IsLoginAuthenticated"] = value;
            }
        }


        public static void Invalidate()
        {
            UserInfo = null;
            UserEmail = null;
            DisplayName = null;
            ImageURL = null;
            IsLoginAuthenticated = false;
        }

        /// <summary>
        /// Get The Form OrgnizationID
        /// </summary>
        public static int? OrganizationID
        {
            get
            {
                if (HttpContext.Current.Session["OrganizationID"] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session["OrganizationID"]) ;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["OrganizationID"] = value;
            }
        }


    }
}

