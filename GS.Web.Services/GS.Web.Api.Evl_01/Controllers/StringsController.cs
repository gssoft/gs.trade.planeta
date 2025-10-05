using System.Collections.Generic;
using System.Web.Http;

namespace GS.Web.Api.Evl_01.Controllers
{
    public class StringsController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new[] { "1", "2" };
        }
    }
}
