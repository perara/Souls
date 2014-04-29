using Souls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NHibernate.Linq;


using Souls.Client.Classes;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Http;
namespace SoulsClient.Controllers
{
    public class APIController : ApiController
    {
        // GET api/api
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/api/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/api
        public void Post([FromBody]string value)
        {
        }

        // PUT api/api/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/api/5
        public void Delete(int id)
        {
        }


        

    }
}
