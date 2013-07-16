using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using CurrentDesk.Repository.CurrentDesk;
using CurrentDesk.Common;

namespace CurrentDesk.BackOffice.Security
{
    public class BackOfficeUser : IPrincipal
    {
        public BackOfficeUser(string user, LoginAccountType accountType)
        {
            Identity = new BackOfficeUserIdentity(user, accountType);
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

    public class BackOfficeUserIdentity : IIdentity
    {
        public BackOfficeUserIdentity(string user, LoginAccountType accountType)
        {
            IsAuthenticated = true;
            UserEmail = user;
            Name = user;

            

            try
            {
                //switch (accountType)
                //{
                //    case LoginAccountType.LiveAccount:
                //        var clientBO = new ClientBO();                       
                //        AccountType = (int)clientBO.GetClientInformation(user).FK_AccountTypeID;
                        
                //        break;
                //    case LoginAccountType.PartnerAccount:
                //        var introducingBrokerBO = new IntroducingBrokerBO();
                //        AccountType = (int)introducingBrokerBO.GetClientInformation(user).FK_AccountTypeID;
                        
                //        break;
                //}
            }
            catch
            {
                throw;
            }
        }

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; private set; }

        public int AccountType { get; set; }

        public string UserEmail { get; set; }

        public int UserID { get; set; }

    }



}