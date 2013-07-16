#region Header Information
/**********************************************************************
 * File Name     : InboxController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 2nd May 2013
 * Modified Date : 2nd May 2013
 * Description   : This file contains Inbox controller and related actions
 *                 to handle Inbox functionality of AM
 * ********************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Models.Inbox;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.AssetManager.Controllers
{
    /// <summary>
    /// This class represents Inbox controller and contains related
    /// actions to handle Inbox functionality of AM
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_AM), NoCache]
    public class InboxController : Controller
    {
        #region Variables
        private InternalUserMessageBO intUsrMsgBO = new InternalUserMessageBO();
        #endregion

        /// <summary>
        /// This action returns Index view of Inbox menu
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    ViewData["UserDisplayName"] = SessionManagement.DisplayName;

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns list of user messages as per status
        /// </summary>
        /// <param name="status">status</param>
        /// <returns></returns>
        public ActionResult GetUserMessages(string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    List<InboxModel> lstMessages = new List<InboxModel>();
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    string sortColName = System.Web.HttpContext.Current.Request["sidx"];
                    string sortOrder = System.Web.HttpContext.Current.Request["sord"];

                    //Get all messages of user as per status
                    var userMsgs = intUsrMsgBO.GetUserMessages(loginInfo.UserID, status, sortColName, sortOrder);

                    foreach (var msg in userMsgs)
                    {
                        InboxModel userMsg = new InboxModel();
                        userMsg.PK_MessageID = msg.PK_MessageID;
                        userMsg.MessageSubject = msg.MessageSubject;
                        userMsg.MessageBody = msg.MessageContent;
                        userMsg.Timestamp = Convert.ToDateTime(msg.Timestamp).ToString("dd/MM/yyyy HH:mm:ss tt");
                        userMsg.FromUserName = msg.FromUserName;
                        userMsg.IsRead = (bool)msg.IsRead;
                        userMsg.LongDateString = Convert.ToDateTime(msg.Timestamp).ToLongDateString() + " " + Convert.ToDateTime(msg.Timestamp).ToLongTimeString();

                        lstMessages.Add(userMsg);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstMessages.Count,
                        rows = lstMessages
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action changes status of selected msg to archive
        /// </summary>
        /// <param name="pkMsgID">pkMsgID</param>
        /// <returns></returns>
        public ActionResult ArchiveSelectedMessage(string msgIDs)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(intUsrMsgBO.ArchiveSelectedMessage(msgIDs));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action deletes selected message
        /// </summary>
        /// <param name="pkMsgID">pkMsgID</param>
        /// <returns></returns>
        public ActionResult DeleteSelectedMessage(string msgIds)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(intUsrMsgBO.DeleteSelectedMessage(msgIds));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action sets IsRead value of a msg to true
        /// </summary>
        /// <param name="msgID">msgID</param>
        public void SetMessageIsReadTrue(int msgID)
        {
            try
            {
                intUsrMsgBO.SetMessageIsReadTrue(msgID);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns no. of new msg exists
        /// for user
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckNewUserMessage()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    return Json(intUsrMsgBO.CheckNewUserMessage(loginInfo.UserID));
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }
    }
}
