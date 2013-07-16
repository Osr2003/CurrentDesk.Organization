using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CurrentDesk.Repository.CurrentDesk;
using CurrentDesk.Models;

namespace CurrentDesk.WebAPI.Controllers
{
    public class DemoLeadController : ApiController
    {
        private DemoLeadBO demoLeadBO;


        // GET ALL DEMOLEAD 
        public IEnumerable<DemoLead> GetDemoLead()
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.GetDemoLeads();   
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
        public bool  Delete(int demoLeadID)
        {
            demoLeadBO = new DemoLeadBO();
            return demoLeadBO.DeleteDemoLead(demoLeadID);   
           
        }
    }
}
