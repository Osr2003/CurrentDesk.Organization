using CurrentDesk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.BackOffice.Security
{
    public class LoginInformation
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public int AccountType { get; set; }//Ind/Joint/Corp/Trust
        public int AccountCode { get; set; }//Trading/Managed/IB/AM
        public LoginAccountType LogAccountType { get; set; }//Live/Partner
    }
}