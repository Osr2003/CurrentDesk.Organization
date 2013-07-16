using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurrentDesk.BackOffice.Models
{
    public class ProfileParentModel
    {
        public ProfileResetPwdModel ResetPwd { get; set; }
        public ProfileUserInfoModel UserInfo { get; set; }
    }

    public class ProfileResetPwdModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Profile_lbl_OldPassword")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Profile_lbl_NewPassword")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessageResourceName = "Profile_Error_NotMatch")]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Profile_lbl_ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }
    }

    public class ProfileUserInfoModel
    {
        [Display(Name = "Profile_lbl_FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "Profile_lbl_LastName")]
        public string LastName { get; set; }

        [Display(Name = "Profile_lbl_WorkTitle")]
        public string WorkTitle { get; set; }

        [Display(Name = "Profile_lbl_CompanyName")]
        public string CompanyName { get; set; }

        [Display(Name = "Profile_lbl_CompanyAddress")]
        public string CompanyAddress { get; set; }
    }
}