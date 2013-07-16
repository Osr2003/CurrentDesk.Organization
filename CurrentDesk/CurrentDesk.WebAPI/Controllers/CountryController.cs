using CurrentDesk.Repository.CurrentDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CurrentDesk.Models;

namespace CurrentDesk.WebAPI.Controllers
{
    public class CountryController : ApiController
    {
        private L_CountryBO countryBO;
        public List<L_Country> GetCountry()
        {
            countryBO = new L_CountryBO();
            return countryBO.GetCountries();

        }
    }
}
