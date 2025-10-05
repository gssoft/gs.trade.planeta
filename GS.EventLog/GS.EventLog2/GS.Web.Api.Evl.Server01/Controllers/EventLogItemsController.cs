using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GS.Web.Api.Evl.Server01.Controllers
{
    public class EventLogItemsController : ApiController
    {
        // GET: api/EventLogItems
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EventLogItems/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EventLogItems
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/EventLogItems/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EventLogItems/5
        public void Delete(int id)
        {
        }
    }
}
