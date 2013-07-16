using CurrentDesk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CurrentDesk.BackOffice.Security
{
    public class BackOfficeUserAlias : IPrincipal
    {
        public BackOfficeUserAlias(string user, LoginAccountType accountType)
        {
            Identity = new BackOfficeUserAliasIdentity(user, accountType);

        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }

    public class BackOfficeUserAliasIdentity : IIdentity
    {

        public BackOfficeUserAliasIdentity(string user, LoginAccountType accountType)
        {

            switch (accountType)
            {
                case LoginAccountType.LiveAccount:
                    Name = user + ":" + "LiveAccount";
                    break;
                case LoginAccountType.PartnerAccount:
                    Name = user + ":" + "PartnerAccount";
                    break;
            }

        }
        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }
    }
}
