using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrentDesk.WebAPI.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string MobileNo { get; set; }       
    }
}