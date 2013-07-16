#region Header Information
/***********************************************************************************
 * File Name     : SettingsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 11th June 2013
 * Modified Date : 11th June 2013
 * Description   : This file contains Settings controller and related action
 *                 methods for super admin login
 * *********************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Areas.SuperAdmin.Models.Settings;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.BackOffice.Extension;
using CurrentDesk.BackOffice.Security;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using CurrentDesk.Models;
using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Settings controller and contains action
    /// methods for super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class SettingsController : Controller
    {
        #region Variables
        L_RecievingBankBO receivingBankInfoBO = new L_RecievingBankBO();
        L_CountryBO countryBO = new L_CountryBO();
        L_CurrencyValueBO currencyBO = new L_CurrencyValueBO();
        FundingSourceBO fundSourceBO = new FundingSourceBO();
        FundingSourceAcceptedCurrencyBO fundSoureAcceptedCurrBO = new FundingSourceAcceptedCurrencyBO();
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserRoleSettings()
        {
            return View();
        }

        public ActionResult ClientAgreement()
        {
            return View();
        }

        public ActionResult FundingSourceManagement()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    FundingSourceModel model = new FundingSourceModel();
                    ViewData["ReceivingBankInfo"] = new SelectList(receivingBankInfoBO.GetReceivingBankInfo(), "PK_RecievingBankID", "RecievingBankName");
                    ViewData["Country"] = new SelectList(countryBO.GetCountries(), "PK_CountryID", "CountryName");
                    ViewData["Currency"] = new SelectList(currencyBO.GetCurrencies(), "PK_CurrencyValueID", "CurrencyValue");
                    ViewData["SourceType"] = new SelectList(ExtensionUtility.GetAllSourceTypes(), "ID", "Value");

                    return View(model);
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
        /// This method returns list of all funding sources
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllFundingSources()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    List<FundingSourceDetail> lstFundingSources = new List<FundingSourceDetail>();
                    
                    //Get all funding sources
                    var allFundSources = fundSourceBO.GetAllFundSources();

                    //Iterate through each source
                    foreach (var source in allFundSources)
                    {
                        FundingSourceDetail fundSource = new FundingSourceDetail();
                        fundSource.PK_FundingSourceID = source.PK_FundingSourceID;
                        fundSource.SourceType = GetSourceTypeValueFromID((int)source.SourceType);
                        fundSource.SourceName = source.SourceName;
                        fundSource.AccountNumber = source.AccountNumber;
                        fundSource.AcceptedCurr = fundSoureAcceptedCurrBO.GetAllAcceptedCurrenciesofSource(source.PK_FundingSourceID);
                        if ((bool)source.IsEnabled)
                        {
                            fundSource.Action = "<input class='icon active tip' title='Disable' type='button' value='Disable' onclick='disableFundingSource(" + source.PK_FundingSourceID + ")'><input type='button' class='icon view-edit tip' data-modal='modalUpdateSource' title='View/Edit' onclick='showUpdateFundSource(" + source.PK_FundingSourceID + ")'><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteFundingSource(" + source.PK_FundingSourceID + ")'>";
                        }
                        else
                        {
                            fundSource.Action = "<input class='icon inactive tip' title='Enable' type='button' value='Disable' onclick='enableFundingSource(" + source.PK_FundingSourceID + ")'><input type='button' class='icon view-edit tip' data-modal='modalUpdateSource' title='View/Edit' onclick='showUpdateFundSource(" + source.PK_FundingSourceID + ")'><input class='icon delete tip' title='Delete' type='button' value='Delete' onclick='deleteFundingSource(" + source.PK_FundingSourceID + ")'>";
                        }

                        lstFundingSources.Add(fundSource);
                    }

                    //Return list
                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstFundingSources.Count,
                        rows = lstFundingSources
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
                throw;
            }
        }

        /// <summary>
        /// This method returns source type value from ID
        /// </summary>
        /// <param name="sourceTypeID">sourceTypeID</param>
        /// <returns></returns>
        public string GetSourceTypeValueFromID(int sourceTypeID)
        {
            var allSourceTypes = ExtensionUtility.GetAllSourceTypes();
            foreach (var source in allSourceTypes)
            {
                if (source.ID == sourceTypeID)
                {
                    return source.Value;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// This action inserts new fund source info into database
        /// </summary>
        /// <param name="newFundSource">newFundSource</param>
        /// <returns></returns>
        public ActionResult AddNewFundingSource(FundingSourceDetail newFundSource)
        {
            int? fkInterBankCountryID = null;
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    FundingSource source = new FundingSource();
                    source.SourceType = Convert.ToInt32(newFundSource.SourceType);
                    source.SourceName = newFundSource.SourceName;
                    source.BankName = newFundSource.BankName;
                    source.BicOrSwiftCode = newFundSource.BicOrSwiftCode;
                    source.AccountNumber = newFundSource.AccountNumber;
                    source.FK_ReceivingBankInfoID = newFundSource.FK_ReceivingBankInfoID;
                    source.ReceivingBankInfo = newFundSource.ReceivingBankInfo;
                    source.BankAddress = newFundSource.BankAddress;
                    source.BankCity = newFundSource.BankCity;
                    source.FK_BankCountryID = newFundSource.FK_BankCountryID;
                    source.BankPostalCode = newFundSource.BankPostalCode;
                    source.InterBankName = newFundSource.InterBankName;
                    source.FK_InterBankCountryID = newFundSource.FK_InterBankCountryID != 0 ? newFundSource.FK_InterBankCountryID : fkInterBankCountryID;
                    source.InterBicOrSwiftCode = newFundSource.InterBicOrSwiftCode;
                    source.IncomingWireFeeAmount = newFundSource.IncomingWireFeeAmount;
                    source.OutgoingWireFeeAmount = newFundSource.OutgoingWireFeeAmount;
                    source.FK_IncomingWireFeeCurrency = newFundSource.FK_IncomingWireFeeCurr;
                    source.FK_OutgoingWireFeeCurrency = newFundSource.FK_OutgoingWireFeeCurr;
                    source.IsEnabled = false;
                    source.IsDeleted = false;

                    //Add accepted currencies if funding source inserted successfully
                    if (fundSourceBO.AddNewFundingSource(source))
                    {
                        return Json(fundSoureAcceptedCurrBO.AddFundingSourceAcceptedCurrency(source.PK_FundingSourceID, newFundSource.AcceptedCurr));
                    }
                    else
                    {
                        return Json(false);
                    }
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
        /// This action returns details of a funding source
        /// </summary>
        /// <param name="pkFundingSourceID">pkFundingSourceID</param>
        /// <returns></returns>
        public JsonResult GetFundingSourceDetails(int pkFundingSourceID)
        {
            try
            {
                //Get details
                var fundSource = fundSourceBO.GetFundingSourceDetails(pkFundingSourceID);

                FundingSourceDetail detail = new FundingSourceDetail();
                detail.PK_FundingSourceID = fundSource.PK_FundingSourceID;
                detail.SourceName = fundSource.SourceName;
                detail.SourceType = fundSource.SourceType.ToString();
                detail.BankName = fundSource.BankName;
                detail.AccountNumber = fundSource.AccountNumber;
                detail.BicOrSwiftCode = fundSource.BicOrSwiftCode;
                detail.FK_ReceivingBankInfoID = (int)fundSource.FK_ReceivingBankInfoID;
                detail.ReceivingBankInfo = fundSource.ReceivingBankInfo;
                detail.BankAddress = fundSource.BankAddress;
                detail.BankCity = fundSource.BankCity;
                detail.FK_BankCountryID = (int)fundSource.FK_BankCountryID;
                detail.BankPostalCode = fundSource.BankPostalCode;
                detail.InterBankName = fundSource.InterBankName;
                detail.FK_InterBankCountryID = fundSource.FK_InterBankCountryID ?? 0;
                detail.InterBicOrSwiftCode = fundSource.InterBicOrSwiftCode;
                detail.FK_IncomingWireFeeCurr = fundSource.FK_IncomingWireFeeCurrency ?? 0;
                detail.IncomingWireFeeAmount = (decimal)fundSource.IncomingWireFeeAmount;
                detail.FK_OutgoingWireFeeCurr = fundSource.FK_OutgoingWireFeeCurrency ?? 0;
                detail.OutgoingWireFeeAmount = (decimal)fundSource.OutgoingWireFeeAmount;
                detail.AcceptedCurr = fundSoureAcceptedCurrBO.GetAllAcceptedCurrenciesofSource(fundSource.PK_FundingSourceID);

                return Json(detail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This actions updates funding source data
        /// </summary>
        /// <param name="fundSource">fundSource</param>
        /// <returns></returns>
        public ActionResult UpdateFundingSource(FundingSourceDetail fundSource)
        {
            int? fkInterBankCountryID = null;
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    FundingSource source = new FundingSource();
                    source.PK_FundingSourceID = fundSource.PK_FundingSourceID;
                    source.SourceType = Convert.ToInt32(fundSource.SourceType);
                    source.SourceName = fundSource.SourceName;
                    source.BankName = fundSource.BankName;
                    source.BicOrSwiftCode = fundSource.BicOrSwiftCode;
                    source.AccountNumber = fundSource.AccountNumber;
                    source.FK_ReceivingBankInfoID = fundSource.FK_ReceivingBankInfoID;
                    source.ReceivingBankInfo = fundSource.ReceivingBankInfo;
                    source.BankAddress = fundSource.BankAddress;
                    source.BankCity = fundSource.BankCity;
                    source.FK_BankCountryID = fundSource.FK_BankCountryID;
                    source.BankPostalCode = fundSource.BankPostalCode;
                    source.InterBankName = fundSource.InterBankName;
                    source.FK_InterBankCountryID = fundSource.FK_InterBankCountryID != 0 ? fundSource.FK_InterBankCountryID : fkInterBankCountryID;
                    source.InterBicOrSwiftCode = fundSource.InterBicOrSwiftCode;
                    source.IncomingWireFeeAmount = fundSource.IncomingWireFeeAmount;
                    source.OutgoingWireFeeAmount = fundSource.OutgoingWireFeeAmount;
                    source.FK_IncomingWireFeeCurrency = fundSource.FK_IncomingWireFeeCurr;
                    source.FK_OutgoingWireFeeCurrency = fundSource.FK_OutgoingWireFeeCurr;
                    source.IsEnabled = false;
                    source.IsDeleted = false;

                    //Call BO method to update
                    if (fundSourceBO.UpdateFundingSource(source))
                    {
                        return Json(fundSoureAcceptedCurrBO.UpdateFundingSourceAcceptedCurrency(fundSource.PK_FundingSourceID, fundSource.AcceptedCurr));
                    }
                    else
                    {
                        return Json(false);
                    }
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
        /// This action enables particular funding source
        /// </summary>
        /// <param name="fundSourceID">fundSourceID</param>
        /// <returns></returns>
        public ActionResult EnableFundingSource(int fundSourceID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(fundSourceBO.EnableFundingSource(fundSourceID));
                }
                else
                {
                    return RedirectToAction("Login", "Action", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action disables particular funding source
        /// </summary>
        /// <param name="fundSourceID">fundSourceID</param>
        /// <returns></returns>
        public ActionResult DisableFundingSource(int fundSourceID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(fundSourceBO.DisableFundingSource(fundSourceID));
                }
                else
                {
                    return RedirectToAction("Login", "Action", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        /// <summary>
        /// This action deletes particular funding source
        /// </summary>
        /// <param name="fundSourceID">fundSourceID</param>
        /// <returns></returns>
        public ActionResult DeleteFundingSource(int fundSourceID)
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    return Json(fundSourceBO.DeleteFundingSource(fundSourceID));
                }
                else
                {
                    return RedirectToAction("Login", "Action", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                return Json(false);
            }
        }

        public ActionResult BrokerFormsManagement()
        {
            return View();
        }

        public ActionResult KYCDocManagement()
        {
            return View();
        }

        public ActionResult FeeGroupManagement()
        {
            return View();
        }

        public ActionResult SymbolManagement()
        {
            return View();
        }

        public ActionResult GlobalSettings()
        {
            return View();
        }
    }
}
