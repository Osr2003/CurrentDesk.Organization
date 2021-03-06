using System;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.DAL;
using CurrentDesk.Models;
using System.Data.Objects;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template at 20/3/2013 8:25:50 PM
//	   and this file should not get overridden 
//
//     Add your own data access methods.
// </auto-generated>
//------------------------------------------------------------------------------ 
	
namespace CurrentDesk.Repository.CurrentDesk
{   
	public class PartnerCommissionBO
	{
		// Add your own data access methods here.  If you wish to
		// expose your public method to a WCF service, marked them with
		// the attribute [NCPublish], and another T4 template will generate your service contract

        /// <summary>
        /// This function will insert new partner spread details in PartnerCommission table
        /// </summary>
        /// <returns></returns>
        public void AddNewPartnerSpread(PartnerCommission partComm)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var partCommRepo =
                        new PartnerCommissionRepository(new EFRepository<PartnerCommission>(), unitOfWork);

                    partCommRepo.Add(partComm);
                    partCommRepo.Save();
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This method returns all fee structures of a particular IB user
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns></returns>
        public List<PartnerCommission> GetAllFeeStructureForUser(int userID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var partCommRepo =
                        new PartnerCommissionRepository(new EFRepository<PartnerCommission>(), unitOfWork);

                    ObjectSet<PartnerCommission> partCommObjSet =
                     ((CurrentDeskClientsEntities)partCommRepo.Repository.UnitOfWork.Context).PartnerCommissions;

                    //Return list of fee structure of user
                    return partCommObjSet.Where(part => part.FK_UserID == userID).ToList();
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This method disables currently active fee and makes selected fee active
        /// </summary>
        /// <param name="userID">userID</param>
        /// <param name="feeID">feeID</param>
        /// <returns></returns>
        public bool MakeFeeStructureDefault(int userID, int feeID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var partCommRepo =
                            new PartnerCommissionRepository(new EFRepository<PartnerCommission>(), unitOfWork);

                    ObjectSet<PartnerCommission> partCommObjSet =
                     ((CurrentDeskClientsEntities)partCommRepo.Repository.UnitOfWork.Context).PartnerCommissions;

                    //Get currently active fee
                    var activeFee = partCommObjSet.Where(part => part.FK_UserID == userID && part.IsDefault == true).FirstOrDefault();
                    if (activeFee != null)
                    {
                        activeFee.IsDefault = false;

                        //Get selected fee to be made active
                        var selectedFee = partCommObjSet.Where(part => part.PK_PartnerCommID == feeID).FirstOrDefault();
                        if (selectedFee != null)
                        {
                            selectedFee.IsDefault = true;

                            partCommRepo.Save();
                            return true;
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
		
	}
}