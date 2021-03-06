using System;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.DAL;
using CurrentDesk.Models;
using System.Data.Objects;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template at 6/2/2013 1:16:13 PM
//	   and this file should not get overridden 
//
//     Add your own data access methods.
// </auto-generated>
//------------------------------------------------------------------------------ 
	
namespace CurrentDesk.Repository.CurrentDesk
{   
	public class AccountCreationRuleBO
	{
        /// <summary>
        /// This method gets all account number creation rules rows
        /// </summary>
        /// <returns></returns>
        public List<AccountCreationRule> GetRule()
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var accountCreationRuleRepo =
                        new AccountCreationRuleRepository(new EFRepository<AccountCreationRule>(), unitOfWork);
                    return accountCreationRuleRepo.All().ToList();
                }
            }
            catch(Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This method gets all account number creation rules rows
        /// </summary>
        /// <param name="organizationID">organizationID</param>
        /// <returns></returns>
        public List<AccountCreationRule> GetRule(int organizationID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var accountCreationRuleRepo =
                        new AccountCreationRuleRepository(new EFRepository<AccountCreationRule>(), unitOfWork);

                    ObjectSet<AccountCreationRule> accCreationRuleObjSet =
                        ((CurrentDeskClientsEntities)accountCreationRuleRepo.Repository.UnitOfWork.Context).AccountCreationRules;

                    return accCreationRuleObjSet.Where(rule => rule.FK_OrganizationID == organizationID).ToList();
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
	}

   
}