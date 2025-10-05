using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ca_OwinSelfHost_Auth_01.Controllers
{
    public class ValuesController : ApiController
    {
        public string GetValues()
        {
            return "Values";
        }
    }
}
