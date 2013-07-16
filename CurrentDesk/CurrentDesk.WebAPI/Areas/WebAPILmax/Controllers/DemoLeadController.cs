using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CurrentDesk.Repository.CurrentDesk;
using System.Web.Mvc;
using CurrentDesk.Models;

namespace CurrentDesk.WebAPI.Areas.WebAPILmax.Controllers
{
    public class DLeadController : ApiController
    {
        private DemoLeadBO demoLeadBO;


        // GET ALL DEMOLEAD 
       [HandleError(View="Error")]
        public IEnumerable<DemoLead> GetDemoLead()
        {
            
            demoLeadBO = new DemoLeadBO();
            var obj = demoLeadBO.GetDemoLeads();

            

            if (obj == null)
            {
                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("No customer with ID = {0}", 1)),
                    ReasonPhrase = "CustomerID Not Found in Database!"
                };
                throw new HttpResponseException(msg);
            }
            else
            {
                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("No customer with ID = {0}", 1)),
                    ReasonPhrase = "CustomerID Not Found in Database!"
                };
                throw new HttpResponseException(msg);
                //return obj;
            }
        }

        // GET SELECTED DEMOLEAD
        public DemoLead GetDemoLead(int demoLeadID)
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.GetSelectedDemoLead(demoLeadID);

        }

        // POST ADD NEW DEMOLEAD
        public bool Post(DemoLead demoLead)
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.AddNewDemoLead(demoLead);
        }

        // PUT UPDATE DEMOLEAD
        public bool Put(DemoLead demoLead)
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.EditDemoLead(demoLead);
        }

        // DELETE  SELECTED DEMOLEAD        
        public bool Delete(int demoLeadID)
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.DeleteDemoLead(demoLeadID);

        }
    }
}
