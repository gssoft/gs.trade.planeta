using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GS.ConsoleAS;

namespace GS.Web.Api.Service01.Controllers
{
    public class WhoAreYouController : ApiController
    {
        private IMyService _myservice;

        // public WhoAreYouController() {}

        public WhoAreYouController(IMyService myservice)
        {
            var m = MethodBase.GetCurrentMethod().Name;
            //if (_myservice != null)
            //{
            //    _myservice.Greatings += myservice.Greatings;
            //}
            //else
            //    _myservice = myservice;
            ConsoleSync.WriteLineT($"Before: {m} {_myservice}");
            _myservice = myservice;
            _myservice.Resolved = !_myservice.Resolved;
            _myservice.Tries += 1;
            //_myservice.Greatings += "Great!";
            //myservice.Resolved = true;
            //myservice.Greatings += "Great!";
            ConsoleSync.WriteLineT($"After: {m} {_myservice}");
            // ConsoleSync.WriteLineT(m);
            //if (_myservice == null)
            //{
            //    ConsoleSync.WriteLineT($"{m}: {_myservice?.Greatings}");
            //    _myservice = myservice;
            //    // _myservice.Greatings = "HelloWorld";
            //}
            //else
            //{
            //    ConsoleSync.WriteLineT(m);
            //}
        }

        //[HttpGet]
        //[Route("api/whoareyou", Name = "WhoAreYou")]
        //public string WhoAreYou()
        //{
        //    var s = new MyService();
        //    return s.WhoAreYou();
        //}
        [HttpGet]
        [Route("api/whoareyou", Name = "WhoAreYou")]
        public string WhoAreYou()
        {
            //var s = new MyService();
            // _myservice.Greatings += _myservice.Greatings;
            return _myservice == null ? "MyService is Null" : _myservice.WhoAreYou();
        }
    }
    public class ServicesController : ApiController
    {
        [HttpGet]
        [Route("api/services", Name = "services")]
        public string GetServices()
        {
            return ServicesContext.Instance.ToString();
        }
    }
}