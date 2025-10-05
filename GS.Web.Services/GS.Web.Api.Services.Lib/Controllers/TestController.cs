using System;
using System.Web.Http;

namespace GS.Web.Api.Services.Lib.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("api/test", Name = "test")]
        public string Get()
        {
            return $"{DateTime.Now.ToString("G")} I'm {GetType().FullName}";
        }
    }
}
