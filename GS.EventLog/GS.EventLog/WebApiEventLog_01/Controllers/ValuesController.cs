using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiEventLog_01.Controllers
{
    //[Authorize]
    
    public class ValuesController : ApiController
    {
        private static List<string> _lst;
        // GET api/values
        public IEnumerable<string> Get()
        {
            // return new string[] { "value1", "value2" };
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
            if(_lst == null)
                _lst = new List<string>();
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
