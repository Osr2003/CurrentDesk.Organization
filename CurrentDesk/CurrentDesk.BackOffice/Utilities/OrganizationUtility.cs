using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Utilities
{
    /// <summary>
    /// This is the organization utility class.
    /// </summary>
    public class OrganizationUtility
    {
        /// <summary>
        /// This Function will return the organization
        /// ID for the organization URL
        /// </summary>
        /// <param name="organizationURL">organizationURL</param>
        /// <returns>organizationID</returns>
        public static int? GetOrganizationID(string organizationURL)
        {
            try
            {
                var OrganizationBO = new OrganizationBO();
                return OrganizationBO.GetOrganizationIDFromURL(organizationURL);
            }
            catch
            {
                //TODO Log Error
                return null;
            }

        }

    }
}