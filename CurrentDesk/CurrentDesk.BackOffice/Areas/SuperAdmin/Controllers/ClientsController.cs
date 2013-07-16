#region Header Information
/***************************************************************************
 * File Name     : ClientsController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 10th June 2013
 * Modified Date : 10th June 2013
 * Description   : This file contains Clients controller and related actions
 *                 for super admin login
 * ************************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.BackOffice.Custom;
using CurrentDesk.Common;
using System.Web.Mvc;
#endregion

namespace CurrentDesk.BackOffice.Areas.SuperAdmin.Controllers
{
    /// <summary>
    /// This class represents Clients controller and contains action methods
    /// for super admin login
    /// </summary>
    [AuthorizeRoles(AccountCode = Constants.K_ACCTCODE_SUPERADMIN), NoCache]
    public class ClientsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region Traders Section
        public ActionResult TraderDetails()
        {
            return View();
        }

        public ActionResult TraderAccounts()
        {
            return View();
        }

        public ActionResult TraderNoteHistory()
        {
            return View();
        }
        #endregion

        #region IB Section
        public ActionResult IntroducingBrokerDetails()
        {
            return View();
        }

        public ActionResult IntroducingBrokerAccounts()
        {
            return View();
        }

        public ActionResult IntroducingBrokerClients()
        {
            return View();
        }

        public ActionResult IntroducingBrokerAgents()
        {
            return View();
        }

        public ActionResult IntroducingBrokerFeeGroups()
        {
            return View();
        }

        public ActionResult IntroducingBrokerDocuments()
        {
            return View();
        }

        public ActionResult IntroducingBrokerNoteHistory()
        {
            return View();
        }

        public ActionResult IBAgentDetails()
        {
            return View();
        }

        public ActionResult IBAccountDetails()
        {
            return View();
        }
        #endregion

        #region AM Section
        public ActionResult AssetManagerDetails()
        {
            return View();
        }

        public ActionResult AssetManagerAccounts()
        {
            return View();
        }

        public ActionResult AssetManagerClients()
        {
            return View();
        }

        public ActionResult ManageAccPrograms()
        {
            return View();
        }

        public ActionResult AssetManagerRebateAccDetails()
        {
            return View();
        }

        public ActionResult AssetManagerMasterAccDetails()
        {
            return View();
        }
        #endregion

    }
}
