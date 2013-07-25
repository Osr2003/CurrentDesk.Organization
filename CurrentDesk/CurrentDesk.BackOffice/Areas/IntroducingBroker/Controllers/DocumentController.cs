#region Header Information
/******************************************************************************
 * File Name     : DocumentController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 5th April 2013
 * Modified Date : 5th April 2013
 * Description   : This file contains Document controller and related actions
 *                 to handle IB documents management functionality
 * ****************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Areas.IntroducingBroker.Models.IBClients;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CurrentDesk.Common;
using CurrentDesk.BackOffice.Custom;
#endregion

namespace CurrentDesk.BackOffice.Areas.IntroducingBroker.Controllers
{
    /// <summary>
    /// This class represents DocumentController and contains
    /// actions to handle IB document management functionality
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_IB), NoCache]
    public class DocumentController : Controller
    {
        #region Variables
        private R_UserDocumentBO r_UserDocumentBO = new R_UserDocumentBO();
        private UserDocumentBO userDocumentBO = new UserDocumentBO();
        private UserActivityBO userActivityBO = new UserActivityBO();
        private DocumentActivityBO docActivityBO = new DocumentActivityBO();
        #endregion

        /// <summary>
        /// This action returns Index view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return View("Index");
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
        /// This action returns list of document details related to user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDocumentDetails()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    var lstDocument = new List<ClientDocumentModel>();

                    //Get docs required for that account type
                    var reqDocs = r_UserDocumentBO.GetAllDocumentsOfAccountType(loginInfo.AccountType);

                    //Iterate through each doc type
                    foreach (var doc in reqDocs)
                    {
                        var document = new ClientDocumentModel();
                        document.DocumentName = doc.Document.DocumentName;
                        document.DocumentID = (int)doc.FK_DocumentID;

                        //Get user document details if exists from db
                        var docDetails = userDocumentBO.GetUserDocumentDetails(loginInfo.UserID, (int)doc.FK_DocumentID);
                        if (docDetails != null)
                        {
                            document.Status = docDetails.Status;
                        }
                        else
                        {
                            document.Status = "Missing Documents";
                        }
                        lstDocument.Add(document);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstDocument.Count,
                        rows = lstDocument
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    RedirectToAction("Login", "Account");
                    return View();
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns BrokerForms view
        /// </summary>
        /// <returns></returns>
        public ActionResult BrokerForms()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return View("BrokerForms");
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return View("ErrorMessage");
            }
        }

        /// <summary>
        /// This action returns list of broker forms related to user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBrokerFormDetails()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    var lstDocument = new List<ClientDocumentModel>();

                    //Get broker forms required for that account type
                    var reqBrokerForms = r_UserDocumentBO.GetAllBrokerFormsOfAccountType(loginInfo.AccountType);

                    //Iterate through each broker form
                    foreach (var doc in reqBrokerForms)
                    {
                        var document = new ClientDocumentModel();
                        document.DocumentName = doc.Document.DocumentName;
                        document.DocumentID = (int)doc.FK_DocumentID;

                        lstDocument.Add(document);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstDocument.Count,
                        rows = lstDocument
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
        /// This action method returns required file as attachment
        /// to be downloaded
        /// </summary>
        /// <param name="brokerFormID">brokerFormID</param>
        /// <param name="fileName">fileName</param>
        public void DownloadBrokerForm(int brokerFormID, string fileName)
        {
            try
            {
                //Get extension of the file
                string ext = Path.GetExtension(fileName).ToLower();

                var file = new FileInfo(Server.MapPath("~/BrokerForms/" + brokerFormID + ext));

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AppendHeader("Content-Disposition", "attachment; filename = " + fileName);
                Response.AppendHeader("Content-Length", file.Length.ToString());
                Response.ContentType = GetContentType(fileName);
                Response.WriteFile(file.FullName);
                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method returns extension of a file
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns></returns>
        private string GetContentType(string fileName)
        {

            string contentType = "application/octetstream";

            string ext = Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                contentType = registryKey.GetValue("Content Type").ToString();
            }

            return contentType;
        }

        /// <summary>
        /// This action method make entry in database for 
        /// uploaded doc and save file in file system
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="docID">docID</param>
        /// <returns></returns>
        [HttpPost]
        public string UploadIBDocument(HttpPostedFileBase file, int docID)
        {
            var javascriptSerailizer = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                if (file.ContentLength > 0)
                {
                    if (SessionManagement.UserInfo != null)
                    {
                        LoginInformation loginInfo = SessionManagement.UserInfo;

                        //Make entry in database for uploaded document
                        if (userDocumentBO.UploadDocument(loginInfo.UserID, docID, file.FileName))
                        {
                            //Create file name using userID and docID plus extension
                            var fileName = loginInfo.UserID + "-" + docID + file.FileName.Substring(file.FileName.LastIndexOf('.'));

                            //Specify the path for saving
                            var path = Path.Combine(Server.MapPath("~/UserDocuments"), fileName);

                            //Save the file
                            file.SaveAs(path);

                            //Log document activity details
                            InsertDocumentActivityDetails(docID, "Pending");

                            return javascriptSerailizer.Serialize(true);
                        }
                        else
                        {
                            return javascriptSerailizer.Serialize(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
            return javascriptSerailizer.Serialize(false);
        }

        /// <summary>
        /// This method sends document to be
        /// downloaded as attachment for saving in browser
        /// </summary>
        /// <param name="docID">docID</param>
        public void DownloadDocument(int docID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Get file name from database
                    string fileName = userDocumentBO.GetUploadedDocumentName(loginInfo.UserID, docID);

                    if (fileName != String.Empty)
                    {
                        //Get file extension
                        string fileExt = fileName.Substring(fileName.LastIndexOf('.'));

                        var file = new FileInfo(Server.MapPath("~/UserDocuments/" + loginInfo.UserID + "-" + docID + fileExt));

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.AppendHeader("Content-Disposition", "attachment; filename = " + fileName);
                        Response.AppendHeader("Content-Length", file.Length.ToString());
                        Response.ContentType = GetContentType(file.Name);
                        Response.WriteFile(file.FullName);
                        Response.Flush();
                        Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This action method deletes IB document from
        /// file system and makes IsDeleted = true entry in db
        /// </summary>
        /// <param name="docID">docID</param>
        /// <returns></returns>
        public ActionResult ClearDocument(int docID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;
                    string fileName = userDocumentBO.ClearUserDocument(loginInfo.UserID, docID);
                    string fileExt = fileName.Substring(fileName.LastIndexOf('.'));

                    if (fileName != String.Empty)
                    {
                        //Delete document file from file system
                        System.IO.File.Delete(Directory.EnumerateFileSystemEntries(System.Web.HttpContext.Current.Server.MapPath("~/UserDocuments"), loginInfo.UserID + "-" + docID + fileExt).First());

                        //Log document activity details
                        InsertDocumentActivityDetails(docID, "Missing");

                        return Json(true);
                    }
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action logs document activity details in database
        /// </summary>
        /// <param name="docID">docID</param>
        /// <param name="status">status</param>
        public void InsertDocumentActivityDetails(int docID, string status)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    LoginInformation loginInfo = SessionManagement.UserInfo;

                    //Insert in UserActivity
                    int pkActivityID = userActivityBO.InsertUserActivityDetails(loginInfo.UserID, (int)ActivityType.DocumentActivity);

                    //Insert in ProfileActivity
                    docActivityBO.InsertDocumentActivityDetails(pkActivityID, docID, status);
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
            }
        }

    }
}
