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
        public static bool ValidateUser(string username, string password)
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
                isValidated = userBO.ValidateUser(username, password,ref userID , ref userType, ref accountType, ref accountCode, ref userDisplayName);
                
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

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

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