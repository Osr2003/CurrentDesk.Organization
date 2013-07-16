#region Header Information
/********************************************************************************
 * File Name     : DashboardController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 10th June 2013
 * Modified Date : 10th June 2013
 * Description   : This file contains Dashboard controller and related action
 *                 methods for Super Admin part
 * ******************************************************************************/
#endregion

#region Namespace Used

using System;
using System.Collections.Generic;
using System.Globalization;
using CurrentDesk.BackOffice.Areas.SuperAdmin.Models;
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.Common;
using System.Web.Mvc;
using CurrentDesk.Logging;
using CurrentDesk.Repository.CurrentDesk;
using CurrentDesk.BackOffice.Security;

#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Dashboard controller and contains
    /// related action methods for Super Admin part
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class DashboardController : Controller
    {
        #region Variables
        private AdminTransactionBO adminTransactionBO = new AdminTransactionBO();
        #endregion

        /// <summary>
        /// This action returns Dashboard view of super admin
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
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
                throw;
            }
        }

        /// <summary>
        /// This action returns all withdrawal transaction of clients
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllWithdrawalTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get all withdrawal transactions
                    var outTransactions =
                        adminTransactionBO.GetAllTransactionsOfParticularType((int) AdminTransactionType.OutgoingFunds);

                    var lstWithdrawals = new List<DashboardTransactionModel>();

                    System.Globalization.NumberFormatInfo nfi;
                    nfi = new NumberFormatInfo();
                    nfi.CurrencySymbol = "";

                    //Iterating through each withdrawal transaction
                    foreach (var transaction in outTransactions)
                    {
                        var withdrawal = new DashboardTransactionModel();
                        withdrawal.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        withdrawal.AccountNumber = transaction.AccountNumber;
                        withdrawal.ClientName = transaction.ClientName;
                        withdrawal.Amount = String.Format(nfi, "{0:C}", transaction.TransactionAmount);
                        withdrawal.Status = (bool) transaction.IsApproved ? "Approved" : "Pending";

                        lstWithdrawals.Add(withdrawal);
                    }

                    return Json(new
                        {
                            total = 1,
                            page = 1,
                            records = lstWithdrawals.Count,
                            rows = lstWithdrawals
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
        /// This action returns all deposit transaction of clients
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllDepositTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get all deposit transactions
                    var incTransactions =
                        adminTransactionBO.GetAllTransactionsOfParticularType((int)AdminTransactionType.IncomingFunds);

                    var lstDeposits = new List<DashboardTransactionModel>();

                    System.Globalization.NumberFormatInfo nfi;
                    nfi = new NumberFormatInfo();
                    nfi.CurrencySymbol = "";

                    //Iterating through each deposit transaction
                    foreach (var transaction in incTransactions)
                    {
                        var deposit = new DashboardTransactionModel();
                        deposit.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        deposit.AccountNumber = transaction.AccountNumber;
                        deposit.ClientName = transaction.ClientName;
                        deposit.Amount = String.Format(nfi, "{0:C}", transaction.TransactionAmount);
                        deposit.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstDeposits.Add(deposit);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstDeposits.Count,
                        rows = lstDeposits
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns all internal transfer transactions of clients
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllInternalTransferTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get all internal transfer transactions
                    var internalTransactions =
                        adminTransactionBO.GetAllTransactionsOfParticularType((int)AdminTransactionType.InternalTransfers);

                    var lstInternalTransfers = new List<DashboardTransactionModel>();

                    System.Globalization.NumberFormatInfo nfi;
                    nfi = new NumberFormatInfo();
                    nfi.CurrencySymbol = "";

                    //Iterating through each internal transfer transaction
                    foreach (var transaction in internalTransactions)
                    {
                        var internalTran = new DashboardTransactionModel();
                        internalTran.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        internalTran.AccountNumber = transaction.AccountNumber;
                        internalTran.ClientName = transaction.ClientName;
                        internalTran.ToAccount = transaction.ToAccountNumber;
                        internalTran.ToClientName = transaction.ToClientName;
                        internalTran.Amount = String.Format(nfi, "{0:C}", transaction.TransactionAmount);
                        internalTran.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstInternalTransfers.Add(internalTran);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstInternalTransfers.Count,
                        rows = lstInternalTransfers
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action returns all conversion transactions of clients
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllConversionTransactions()
        {
            try
            {
                if (SessionManagement.UserInfo != null)
                {
                    //Get all conversion transactions
                    var conversionTransactions =
                        adminTransactionBO.GetAllTransactionsOfParticularType((int)AdminTransactionType.ConversionsRequests);

                    var lstConversionTransfers = new List<DashboardTransactionModel>();

                    System.Globalization.NumberFormatInfo nfi;
                    nfi = new NumberFormatInfo();
                    nfi.CurrencySymbol = "";

                    //Iterating through each conversion transaction
                    foreach (var transaction in conversionTransactions)
                    {
                        var convTran = new DashboardTransactionModel();
                        convTran.TransactionDate =
                            Convert.ToDateTime(transaction.TransactionDate).ToString("dd/MM/yyyy hh:mm:ss tt");
                        convTran.AccountNumber = transaction.AccountNumber;
                        convTran.ClientName = transaction.ClientName;
                        convTran.ToAccount = transaction.ToAccountNumber;
                        convTran.ToClientName = transaction.ToClientName;
                        convTran.Amount = String.Format(nfi, "{0:C}", transaction.TransactionAmount);
                        convTran.ExchangeRate = (double)transaction.ExchangeRate;
                        convTran.ExchangedAmount = String.Format(nfi, "{0:C}", Math.Round((decimal)(transaction.TransactionAmount * (decimal)transaction.ExchangeRate), 2));
                        convTran.Status = (bool)transaction.IsApproved ? "Approved" : "Pending";

                        lstConversionTransfers.Add(convTran);
                    }

                    return Json(new
                    {
                        total = 1,
                        page = 1,
                        records = lstConversionTransfers.Count,
                        rows = lstConversionTransfers
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
