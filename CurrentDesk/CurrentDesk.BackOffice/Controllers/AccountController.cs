#region Header Information
/*******************************************************************************
 * File Name     : AccountController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 1st February 2013
 * Modified Date : 1st February 2013
 * Description   : This file contains account login and signout related actions
 * *****************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Models;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using System;
using System.Web.Mvc;
using System.Web.Security;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{    
    /// <summary>
    /// This class represents Account controller handling login
    /// and signout related functionality
    /// </summary>
    public class AccountController : Controller
    {
        #region LOGIN

        /// <summary>
        /// This action will bring us Client Login view
        /// </summary>
        /// <param name="returnUrl">returnUrl</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                //Check Whether User is already Authenticated
                if (SessionManagement.IsLoginAuthenticated)
                {
                    if (SessionManagement.UserInfo.LogAccountType == LoginAccountType.LiveAccount)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    //If Introducing Broker
                    else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_IB)
                    {
                        return RedirectToAction("Index", "Dashboard", new { Area = "IntroducingBroker" });
                    }
                    //If Asset Manager
                    else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_AM)
                    {
                        return RedirectToAction("Index", "Profile", new { Area = "AssetManager" });
                    }
                    //If Super Admin
                    else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_SUPERADMIN)
                    {
                        return RedirectToAction("Index", "Dashboard", new { Area = "SuperAdmin" });
                    }
                }

                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }
       
        /// <summary>
        /// On Submission this action will take place
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="returnUrl">returnUrl</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (LoginVerification.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        if (SessionManagement.UserInfo.LogAccountType == LoginAccountType.LiveAccount)
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        //If Introducing Broker
                        else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_IB)
                        {
                            return RedirectToAction("Index", "Dashboard", new { Area = "IntroducingBroker" });
                        }
                        //If Asset Manager
                        else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_AM)
                        {
                            return RedirectToAction("Index", "Profile", new { Area = "AssetManager" });
                        }
                        //If Super Admin
                        else if (SessionManagement.UserInfo.AccountCode == Constants.K_ACCTCODE_SUPERADMIN)
                        {
                            return RedirectToAction("Index", "Dashboard", new { Area = "SuperAdmin" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Some error occurred!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }


                return View(model);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// This action logs user out
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            SessionManagement.Invalidate();
            return RedirectToAction("Login", "Account", FormMethod.Get);
        }
        
    }
}
