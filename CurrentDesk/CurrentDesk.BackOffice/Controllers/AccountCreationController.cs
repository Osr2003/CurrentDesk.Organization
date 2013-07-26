#region
/******************************************************************
 * File Name     : AccountCreationController.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Creation Date : 6th Feb 2013
 * Modified Date : 6th Feb 2013
 * Description   : This file contains method that creates new account number
 *                 for the user after successful registration
 * ****************************************************************/
#endregion

#region Namespace Used
using CurrentDesk.Repository.CurrentDesk;
using System.Web.Mvc;
using System.Linq;
using CurrentDesk.Models;
using System;
using CurrentDesk.Common;
using CurrentDesk.Logging;
using MT4Wrapper;
using CurrentDesk.BackOffice.Security;
#endregion

namespace CurrentDesk.BackOffice.Controllers
{
    /// <summary>
    /// This controller contains action that creates new account number 
    /// for the user after successful registration
    /// </summary>
    public class AccountCreationController : Controller
    {
        #region Variables
        private static L_CurrencyValueBO currencyBO = new L_CurrencyValueBO();
        private static Client_AccountBO clientAccBO = new Client_AccountBO();
        private static AccountCurrencyBO curencyBO = new AccountCurrencyBO();
        private static AccountCreationRuleBO accountCreationRuleBO = new AccountCreationRuleBO();
        #endregion

        /// <summary>
        /// This action creates new account number for live user after successful
        /// registration and inserts into ClientAccounts table
        /// </summary>
        /// <param name="newClient"></param>
        public static int CreateAccountNumberForUser(Client newClient)
        {
            try
            {
                var tradingAccountNumber = string.Empty;
                var landingAccountNumber = string.Empty;
                var landingAccCurrencyCode = string.Empty;
                var lAccNumber = 0L;

                //Get organization id from session
                var organizationID = (int)SessionManagement.OrganizationID;

                var rulelist = accountCreationRuleBO.GetRule(organizationID).OrderBy(c => c.Position);
                var currencyID = curencyBO.GetCurrencyLookUpID(newClient.FK_AccountCurrencyID);
                var currAccCode = currencyBO.GetCurrencyAccountCode(currencyID);

                //Iterating through each rule/steps of account creation
                foreach (var item in rulelist)
                {
                    //Currency
                    if (item.Meaning == Constants.K_ACC_RULE_CURRENCY)
                    {

                        tradingAccountNumber += currAccCode + "-";
                        landingAccountNumber += currAccCode + "-";
                    }
                    //Account Number Belonging to that Currency
                    else if (item.Meaning == Constants.K_ACC_RULE_CURR_NUMBER)
                    {
                        var template = item.Template;
                        
                        var tradingAccCount = clientAccBO.GetNumberOfSameCurrencyTradingAccountForUser(LoginAccountType.LiveAccount, newClient.PK_ClientID, currencyID);

                        tradingAccountNumber += (tradingAccCount + 1).ToString("D" + template.Length) + "-";

                        //Required for creating landing account
                        for (var ctDigit = 0; ctDigit < template.Length; ctDigit++)
                        {
                            landingAccCurrencyCode += "0";
                        }
                        landingAccountNumber += landingAccCurrencyCode + "-";
                    }
                    //Client Account Number
                    else if (item.Meaning == Constants.K_ACC_RULE_ACC_NUMBER)
                    {
                        var template = item.Template;
                        var existingAccNumber = clientAccBO.GetUserExistingAccountNumber(LoginAccountType.LiveAccount ,newClient.PK_ClientID);
                        var latestAccNumber = clientAccBO.GetLatestAccountNumber(organizationID);

                        //If no previous account exists for the user
                        if (existingAccNumber == "")
                        {
                            //If any acc number exists in db
                            if (latestAccNumber != "")
                            {
                                lAccNumber = Convert.ToInt64(latestAccNumber);
                                tradingAccountNumber += (lAccNumber + 1).ToString("D" + template.Length) + "-";
                                landingAccountNumber += (lAccNumber + 1).ToString("D" + template.Length) + "-";
                                lAccNumber++;
                            }
                            //If no acc number exists in db
                            else
                            {
                                lAccNumber = 1;
                                tradingAccountNumber += lAccNumber.ToString("D" + template.Length) + "-";
                                landingAccountNumber += lAccNumber.ToString("D" + template.Length) + "-";
                            }
                        }
                        //If user has acc number in system
                        else
                        {
                            tradingAccountNumber += existingAccNumber.Split('-')[((int)item.Position - 1)] + "-";
                            landingAccountNumber += existingAccNumber.Split('-')[((int)item.Position - 1)] + "-";

                            lAccNumber = Convert.ToInt64(existingAccNumber.Split('-')[((int)item.Position - 1)]);
                        }
                    }
                }

                //Insert client account information in ClientAccounts table
                return clientAccBO.InsertAccountNumberForUser(LoginAccountType.LiveAccount, newClient.PK_ClientID, newClient.FK_AccountID, currencyID, landingAccountNumber.TrimEnd('-'), tradingAccountNumber.TrimEnd('-'), lAccNumber, organizationID);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This action creates new account number for partner user after successful
        /// registration and inserts into ClientAccounts table
        /// </summary>
        /// <param name="newIB"></param>
        public static int CreateAccountNumberForPartnerUser(IntroducingBroker newIB)
        {
            try
            {
                var tradingAccountNumber = string.Empty;
                var landingAccountNumber = string.Empty;
                var landingAccCurrencyCode = string.Empty;
                var lAccNumber = 0L;

                //Get organization id from session
                var organizationID = (int)SessionManagement.OrganizationID;

                var rulelist = accountCreationRuleBO.GetRule(organizationID).OrderBy(c => c.Position);
                var currencyID = curencyBO.GetCurrencyLookUpID(newIB.FK_AccountCurrencyID);
                var currAccCode = currencyBO.GetCurrencyAccountCode(currencyID);

                //Iterating through each rule/steps of account creation
                foreach (var item in rulelist)
                {
                    //Currency
                    if (item.Meaning == Constants.K_ACC_RULE_CURRENCY)
                    {
                        tradingAccountNumber += currAccCode + "-";
                        landingAccountNumber += currAccCode + "-";
                    }
                    //Account Number Belonging to that Currency
                    else if (item.Meaning == Constants.K_ACC_RULE_CURR_NUMBER)
                    {
                        var template = item.Template;
                        var tradingAccCount = clientAccBO.GetNumberOfSameCurrencyTradingAccountForUser(LoginAccountType.PartnerAccount ,newIB.PK_IntroducingBrokerID, currencyID);
                        tradingAccountNumber += (tradingAccCount + 1).ToString("D" + template.Length) + "-";

                        //Required for creating landing account
                        for (var ctDigit = 0; ctDigit < template.Length; ctDigit++)
                        {
                            landingAccCurrencyCode += "0";
                        }
                        landingAccountNumber += landingAccCurrencyCode + "-";
                    }
                    //Client Account Number
                    else if (item.Meaning == Constants.K_ACC_RULE_ACC_NUMBER)
                    {
                        var template = item.Template;
                        var existingAccNumber = clientAccBO.GetUserExistingAccountNumber(LoginAccountType.PartnerAccount ,newIB.PK_IntroducingBrokerID);
                        var latestAccNumber = clientAccBO.GetLatestAccountNumber(organizationID);

                        //If no previous account exists for the user
                        if (existingAccNumber == "")
                        {
                            //If any acc number exists in db
                            if (latestAccNumber != "")
                            {
                                lAccNumber = Convert.ToInt64(latestAccNumber);
                                tradingAccountNumber += (lAccNumber + 1).ToString("D" + template.Length) + "-";
                                landingAccountNumber += (lAccNumber + 1).ToString("D" + template.Length) + "-";
                                lAccNumber++;
                            }
                            //If no acc number exists in db
                            else
                            {
                                lAccNumber = 1;
                                tradingAccountNumber += lAccNumber.ToString("D" + template.Length) + "-";
                                landingAccountNumber += lAccNumber.ToString("D" + template.Length) + "-";
                            }
                        }
                        //If user has acc number in system
                        else
                        {
                            tradingAccountNumber += existingAccNumber.Split('-')[((int)item.Position - 1)] + "-";
                            landingAccountNumber += existingAccNumber.Split('-')[((int)item.Position - 1)] + "-";
                            lAccNumber = Convert.ToInt64(existingAccNumber.Split('-')[((int)item.Position - 1)]);
                        }
                    }
                }

                //Insert client account information in ClientAccounts table
                return clientAccBO.InsertAccountNumberForUser(LoginAccountType.PartnerAccount ,newIB.PK_IntroducingBrokerID, newIB.FK_AccountID, currencyID, landingAccountNumber.TrimEnd('-'), tradingAccountNumber.TrimEnd('-'), lAccNumber, organizationID);
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// This method creates new user account in meta trader
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public static bool CreateMetaTraderAccountForUser(int pkClientAccID, int? fkPlatformID, User newUser, LoginAccountType accType)
        {
            try
            {
                var user = new MT4ManLibraryNETv03.UserRecordNET();
                user.group = "FQ-IB-One";
                user.name = newUser.UserEmailID;
                user.password = "NewUser";

                var manager = new MetaTraderWrapperManager("mtdem01.primexm.com:443", 900, "!FQS123!!");
                if (manager.IsConnected() == 1)
                {
                    var accStatus = manager.CreateNewUser(user);

                    //If success
                    if (accStatus == 0)
                    {
                        var clientAccBO = new Client_AccountBO();
                        clientAccBO.InsertPlatformLoginForTradingAccount(pkClientAccID, fkPlatformID, user.password, user.login);
                        return true;
                    }
                    else
                    {
                        var error = manager.ErrorDescription(accStatus);
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                CurrentDeskLog.Error(ex.Message, ex);
                throw;
            }
        }


    }
}
