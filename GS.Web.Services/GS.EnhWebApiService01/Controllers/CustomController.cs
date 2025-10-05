using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GS.EnhWebApiService01.Controllers
{
    public class CustomController : ApiController
    {
        [HttpGet]
        [Route("api/customwhoareyou", Name = "Custom")]
        public string CustomWhoAreYou()
        {
            var m = MethodBase.GetCurrentMethod()?.Name;
            return $"{m} Hello from {GetType().FullName}";
        }
    }
}
