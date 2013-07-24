using System;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.Models;
using System.Data.Objects;
using CurrentDesk.DAL;
using CurrentDesk.Common;
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template at 12/25/2012 2:56:23 PM
//	   and this file should not get overridden 
//
//     Add your own data access methods.
// </auto-generated>
//------------------------------------------------------------------------------ 
	
namespace CurrentDesk.Repository.CurrentDesk
{   
	public class JointAccountInformationBO
	{
        /// <summary>
        /// This Function will get primary individual Name
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public string GetLiveIndividualName(int clientID, LoginAccountType accType)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var jointDetailsRepo =
                        new JointAccountInformationRepository(new EFRepository<JointAccountInformation>(), unitOfWork);

                    ObjectSet<JointAccountInformation> jointObjSet =
                        ((CurrentDeskClientsEntities)jointDetailsRepo.Repository.UnitOfWork.Context).JointAccountInformations;

                    if (accType == LoginAccountType.LiveAccount)
                    {
                        var res = jointObjSet.Where(ind => ind.FK_ClientID == clientID).FirstOrDefault();
                        return res != null ? res.PrimaryAccountHolderFirstName + " " + res.PrimaryAccountHolderLastName : null;
                    }
                    else if (accType == LoginAccountType.PartnerAccount)
                    {
                        var res = jointObjSet.Where(ind => ind.FK_IntroducingBrokerID == clientID).FirstOrDefault();
                        return res != null ? res.PrimaryAccountHolderFirstName + " " + res.PrimaryAccountHolderLastName : null;
                    }
                    return string.Empty;
                }
            }
            catch(Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This Function will get primary individual Name
        /// </summary>
        /// <param name="introducingBrokerID">introducingBrokerID</param>
        /// <returns></returns>
        public string GetPartnerIndividualName(int introducingBrokerID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var jointDetailsRepo =
                        new JointAccountInformationRepository(new EFRepository<JointAccountInformation>(), unitOfWork);

                    ObjectSet<JointAccountInformation> jointObjSet =
                        ((CurrentDeskClientsEntities)jointDetailsRepo.Repository.UnitOfWork.Context).JointAccountInformations;

                    var res = jointObjSet.Where(ind => ind.FK_IntroducingBrokerID == introducingBrokerID).FirstOrDefault();

                    return res != null ? res.PrimaryAccountHolderFirstName + " " + res.PrimaryAccountHolderLastName : null;
                }
            }
            catch(Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This method returns joint account details of a particular client
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public JointAccountInformation GetJointAccountDetails(int clientID)
        {
            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var jointDetailsRepo =
                        new JointAccountInformationRepository(new EFRepository<JointAccountInformation>(), unitOfWork);

                    ObjectSet<JointAccountInformation> jointObjSet =
                        ((CurrentDeskClientsEntities)jointDetailsRepo.Repository.UnitOfWork.Context).JointAccountInformations;

                    return jointObjSet.Where(jnt => jnt.FK_ClientID == clientID).FirstOrDefault();
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