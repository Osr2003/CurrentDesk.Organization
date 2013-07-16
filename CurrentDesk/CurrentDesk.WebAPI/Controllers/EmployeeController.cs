using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CurrentDesk.WebAPI.Models;

namespace CurrentDesk.WebAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        private List<Employee> EmpList = new List<Employee>();

        public EmployeeController()
        {
            EmpList.Add(new Employee() { ID = 1, Name = "Employee 1", Department = "Employee Deartment 1", MobileNo = "97786676789" });
            EmpList.Add(new Employee() { ID = 2, Name = "Employee 2", Department = "Employee Deartment 2", MobileNo = "97786676789" });
            EmpList.Add(new Employee() { ID = 3, Name = "Employee 3", Department = "Employee Deartment 3", MobileNo = "97786676789" });
          
        }
        // GET api/EmployeeAPI
        public IEnumerable<Employee> GetEmployees()
        {
            return EmpList;
        }

        // GET api/EmployeeAPI/5
        public Employee GetEmployee(int id)
        {
            return EmpList.Find(e => e.ID == id);

        }

        // POST api/EmployeeAPI
        public IEnumerable<Employee> Post(Employee value)
        {
            //EmpList.Add(value);
            return EmpList;
        }

        // PUT api/EmployeeAPI/5
        public void Put(int id, string value)
        {

        }

        // DELETE api/EmployeeAPI/5        
        public IEnumerable<Employee> Delete(int id)
        {
            EmpList.Remove(EmpList.Find(E => E.ID == id));
            return EmpList;
        }
    }
}
