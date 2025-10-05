using System;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace OwinAuthentication.Controllers
{
    [Authorize]
    public class POCController : ApiController
    {
        [HttpGet]
        [Route("api/TestMethod")]
        public string TestMethod()
        {
            // return "Hello, C# Corner Member. ";

            var id1 = User.Identity.GetUserId();
            var id2 = RequestContext.Principal.Identity.GetUserId();
            var name = User.Identity.Name;

           // var user = UserManager.FindByName(User.Identity.Name);

            return $"Hello from method: {System.Reflection.MethodBase.GetCurrentMethod().Name} "  + Environment.NewLine +
                   $"id1: {id1 + Environment.NewLine}, id2: {id2 + Environment.NewLine}, name: {name + Environment.NewLine}";
        }
        [HttpGet]
        [Route("api/User")]
        public string GetUser()
        {
            // return "Hello, C# Corner Member. ";

            var id1 = User.Identity.GetUserId();
            var id2 = RequestContext.Principal.Identity.GetUserId();
            var name = User.Identity.Name;

            // var user = UserManager.FindByName(User.Identity.Name);

            return $"Hello from method: {System.Reflection.MethodBase.GetCurrentMethod().Name} " + Environment.NewLine +
                   $"id1: {id1 + Environment.NewLine}, id2: {id2 + Environment.NewLine}, name: {name + Environment.NewLine}";
        }

    }
}
