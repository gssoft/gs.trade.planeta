using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GS.ConsoleAS;

namespace GS.Web.Service01.Controllers
{
    public class TestStringController : ApiController
    {
        [HttpGet]
        [ActionName("TestString")]
        public string Get()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ConsoleAsync.WriteLineT(m);
            return @"TestString";
        }
        [HttpGet]
        [ActionName("TestString")]
        public string GetStrings(int id)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ConsoleAsync.WriteLineT(m);
            return $"String {new List<string> {"1","2","3","4","5"}[id]}";
        }
    }
}
