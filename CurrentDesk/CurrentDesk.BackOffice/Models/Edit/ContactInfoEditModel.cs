#region
/*******************************************************************
 * File Name     : ContactInfoEditModel.cs
 * Author        : Chinmoy Dey
 * Copyright     : Mindfire Solutions
 * Date          : 31st Jan 2013
 * Modified Date : 31st Jan 2013
 * Description   : This file contains model for Contact Informtion update
 * *****************************************************************/
#endregion

#region Namespace Used
#endregion
namespace CurrentDesk.BackOffice.Models.Edit
{
    /// <summary>
    /// This class represents model for Contact Information profile update
    /// </summary>
    public class ContactInfoEditModel
    {
        public string TelephoneCountryCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileCountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
    }
}