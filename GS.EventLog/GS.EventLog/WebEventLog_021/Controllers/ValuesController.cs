using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Ajax.Utilities;

namespace WebEventLog_021.Controllers
{
   // [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values

        private List<string> _lst = new List<string>{ "value1", "value2", "value3"};
        public IEnumerable<string> Get()
        {
            //return new string[] { "value1", "value2" };
            return _lst.AsEnumerable();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            _lst.Add(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

  
}
