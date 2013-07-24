#region HEADER
/***************************************************************************
 * File Name     : LoginVerification.cs
 * Author        : Mukesh Nayak
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th Feb 2013
 * Modified Date : 11th Feb 2013
 * Description   : This file contains logic for validating user.
 * ************************************************************************/
#endregion

#region NAMESPACE

using CurrentDesk.Common;
using CurrentDesk.Repository.CurrentDesk;
using System.IO;
using System.Linq;
using System.Web;

#endregion

namespace CurrentDesk.BackOffice.Security
{
    /// <summary>
    /// This Function Will do Login Verification
    /// </summary>
    public static class LoginVerification
    {
        /// <summary>
        /// This action returns true if user is valid user
        /// and sets required session values
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public static bool ValidateUser(string username, string password)
        {
            bool isValidated = false;
            int accountType = 0;
            int userType = 0;
            int userID = 0;
            int accountCode = 0;
            int organizationID = 0;
            string userDisplayName = string.Empty;           

            try
            {               

                var userBO = new UserBO();
                isValidated = userBO.ValidateUser(username, password,ref userID , ref userType, ref accountType, ref accountCode, ref userDisplayName, ref organizationID);
                
                //if true
                if (isValidated)
                {
                    var loginInfo = new LoginInformation()
                    {
                        UserID = userID,
                        UserEmail = username,
                        AccountType = accountType,
                        AccountCode = accountCode
                    };

                    if (userType == Constants.K_BROKER_LIVE)
                    {
                        loginInfo.LogAccountType =  LoginAccountType.LiveAccount;
                    }
                    else if (userType == Constants.K_BROKER_PARTNER)
                    {
                        loginInfo.LogAccountType = LoginAccountType.PartnerAccount;
                    }
                    else if (userType == Constants.K_BROKER_ADMIN)
                    {
                        loginInfo.LogAccountType = LoginAccountType.AdminAccount;
                    }

                    SessionManagement.UserInfo = loginInfo;
                    SessionManagement.DisplayName = userDisplayName;
                    SessionManagement.ImageURL = GetImageURL(loginInfo.UserID);
                    SessionManagement.IsLoginAuthenticated = true;
                    SessionManagement.OrganizationID = organizationID;

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This Function Will Validate User 
        /// With the Organization ID
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ValidateUser(string username, string password, int organizationID)
        {
            bool isValidated = false;
            int accountType = 0;
            int userType = 0;
            int userID = 0;
            int accountCode = 0;           
            string userDisplayName = string.Empty;

            try
            {

                var userBO = new UserBO();
                isValidated = userBO.ValidateUser(username, password,organizationID, ref userID, ref userType, ref accountType, ref accountCode, ref userDisplayName);

                //if true
                if (isValidated)
                {
                    var loginInfo = new LoginInformation()
                    {
                        UserID = userID,
                        UserEmail = username,
                        AccountType = accountType,
                        AccountCode = accountCode
                    };

                    if (userType == Constants.K_BROKER_LIVE)
                    {
                        loginInfo.LogAccountType = LoginAccountType.LiveAccount;
                    }
                    else if (userType == Constants.K_BROKER_PARTNER)
                    {
                        loginInfo.LogAccountType = LoginAccountType.PartnerAccount;
                    }
                    else if (userType == Constants.K_BROKER_ADMIN)
                    {
                        loginInfo.LogAccountType = LoginAccountType.AdminAccount;
                    }

                    SessionManagement.UserInfo = loginInfo;
                    SessionManagement.DisplayName = userDisplayName;
                    SessionManagement.ImageURL = GetImageURL(loginInfo.UserID);
                    SessionManagement.IsLoginAuthenticated = true;
                    SessionManagement.OrganizationID = organizationID;

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This action returns image path of an user
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns></returns>
        public static string GetImageURL(int userID)
        {
            try
            {

                if (Directory.EnumerateFileSystemEntries(HttpContext.Current.Server.MapPath("~/UploadedImages"), userID + ".*").Any())
                {
                    var res = Directory.EnumerateFileSystemEntries(HttpContext.Current.Server.MapPath("~/UploadedImages"), userID + ".*").First();
                    var fileInfo = new FileInfo(res);
                    return "../UploadedImages/" + fileInfo.Name;
                }
            }
            catch
            {

            }
            return "../Images/avatar.png";
        }
        
    }
}