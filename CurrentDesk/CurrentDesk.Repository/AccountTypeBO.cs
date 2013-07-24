/* **************************************************************
* File Name     :- AccountTypeBO.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 25thd Dec 2012
* Modified Date :- 2nd Jan 2013
* Description   :- This file with all the buisness logic Related to Account Type
****************************************************************/

#region Namespace Defined
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using CurrentDesk.DAL;
using CurrentDesk.Models;
using CurrentDesk.Repository.Utility;
using System;
#endregion

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template at 12/25/2012 2:56:22 PM
//	   and this file should not get overridden 
//
//     Add your own data access methods.
// </auto-generated>
//------------------------------------------------------------------------------ 
	
namespace CurrentDesk.Repository.CurrentDesk
{   
    /// <summary>
    /// Business Object for Account type
    /// </summary>
	public class AccountTypeBO
	{       

        /// <summary>
        /// This Function Will return all account types
        /// </summary>
        /// <param name="formType">formType</param>
        /// <param name="organizationID">organizationID</param>
        /// <returns></returns>
        public List<AccountType> GetSelectedAccountType(int formType, int organizationID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var accountCurrencyRepo =
                        new AccountTypeRepository(new EFRepository<AccountType>(), unitOfWork);

                    ObjectSet<AccountType> currencyObjSet =
                       ((CurrentDeskClientsEntities)accountCurrencyRepo.Repository.UnitOfWork.Context).AccountTypes;


                    return currencyObjSet.Where(accur => accur.FK_AccountFormType == formType && accur.FK_OrganizationID == organizationID).Include("L_AccountTypeValue").ToList();
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This Function will return account type value
        /// </summary>
        /// <param name="formAccountType">formAccountType</param>
        /// <returns>account type Value</returns>
        public int GetAccountTypeValue(int formAccountType)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var accountCurrencyRepo =
                        new AccountTypeRepository(new EFRepository<AccountType>(), unitOfWork);

                    ObjectSet<AccountType> currencyObjSet =
                       ((CurrentDeskClientsEntities)accountCurrencyRepo.Repository.UnitOfWork.Context).AccountTypes;


                    return
                        currencyObjSet.Where(accur => accur.PK_AccountType == formAccountType)
                                      .Select(x => x.FK_AccountTypeValue)
                                      .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This Function will return account type value and form type value
        /// </summary>
        /// <param name="formAccountType">formAccountType</param>
        /// <returns></returns>
        public AccountType GetAccountTypeAndFormTypeValue(int formAccountType)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var accountCurrencyRepo =
                        new AccountTypeRepository(new EFRepository<AccountType>(), unitOfWork);

                    ObjectSet<AccountType> currencyObjSet =
                       ((CurrentDeskClientsEntities)accountCurrencyRepo.Repository.UnitOfWork.Context).AccountTypes;


                    return
                        currencyObjSet.Where(accur => accur.PK_AccountType == formAccountType).FirstOrDefault();
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