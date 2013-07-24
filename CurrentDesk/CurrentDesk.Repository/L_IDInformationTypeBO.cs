using System;
using System.Linq;
using System.Collections.Generic;
using CurrentDesk.Models;
using System.Data.Objects;
using CurrentDesk.DAL;
using CurrentDesk.Repository.Utility;

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
    public class L_IDInformationTypeBO
    {
        /// <summary>
        /// This Function Will return all the ID Info types
        /// </summary>
        /// <returns></returns>
        public List<L_IDInformationType> GetIdInfoType()
        {

            try
            {
                var idInformationKey = CacheKey.CDS_ID_INFORMATION;
                var idInformationList = new List<L_IDInformationType>();

                if (StaticCache.Exist(idInformationKey))
                {
                    idInformationList = (List<L_IDInformationType>)StaticCache.Get(idInformationKey);
                }
                else
                {
                    using (var unitOfWork = new EFUnitOfWork())
                    {
                        var lIdInfoTypeRepo =
                            new L_IDInformationTypeRepository(new EFRepository<L_IDInformationType>(), unitOfWork);

                        //Returning list of ID info type values
                        idInformationList = lIdInfoTypeRepo.All().ToList();

                        StaticCache.Max(idInformationKey, idInformationList);
                    }
                }
                return idInformationList;
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This Function Will return all the ID Info types
        /// </summary>
        /// <param name="organizationID">organizationID</param>
        /// <returns></returns>
        public List<L_IDInformationType> GetIdInfoType(int organizationID)
        {

            try
            {
                using (var unitOfWork = new EFUnitOfWork())
                {
                    var lIdInfoTypeRepo =
                        new L_IDInformationTypeRepository(new EFRepository<L_IDInformationType>(), unitOfWork);

                    ObjectSet<L_IDInformationType> idInformationTypesObjectSet =
                  ((CurrentDeskClientsEntities)lIdInfoTypeRepo.Repository.UnitOfWork.Context).L_IDInformationType;

                    return idInformationTypesObjectSet.Where(idInfo => idInfo.FK_OrganizationID == organizationID).ToList();
                }

            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// This method returns id type value from ID
        /// </summary>
        /// <param name="selectedID">selectedID</param>
        /// <returns></returns>
        public string GetSelectedIDInformation(int selectedID)
        {
            try
            {
                return GetIdInfoType().Where(idInfo => idInfo.PK_IDTypeID == selectedID)
                    .Select(idInfo => idInfo.IDValue).FirstOrDefault();
                
            }
            catch (Exception ex)
            {
                CommonErrorLogger.CommonErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

    }
}