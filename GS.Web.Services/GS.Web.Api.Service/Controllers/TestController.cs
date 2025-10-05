using System;
using System.Web.Http;
using GS.Contexts;

namespace GS.Web.Api.Service01.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("api/test", Name = "test")]
        public string Get()
        {
            var testservice = TestContext.Instance;
            return $"{DateTime.Now.ToString("G")} I'm {GetType().FullName}";
        }
    }
}
