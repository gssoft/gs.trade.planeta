using System;
using System.Web.Http;
namespace GS.WebApiControllers.Controllers
// namespace GS.Web.Api.Services.Lib.Controllers
{
    public class OtherController : ApiController
    {
        [HttpGet]
        [Route("api/other", Name = "Other")]
        public string Get()
        {
            return $"{DateTime.Now.ToString("G")} I'm {GetType().FullName}";
        }
    }
    public class CmdController : ApiController
    {
        [HttpGet]
        [Route("api/cmd/id/command", Name = "cmd")]
        public string Get(string id, string command )
        {
            return $"{DateTime.Now.ToString("G")} I'm {GetType().FullName} Service:{id} Command:{command}";
        }
    }

}
