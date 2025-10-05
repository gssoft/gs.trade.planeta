using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;

namespace GS.Web.Api.Service01
{
    public interface IMyService
    {
        string WhoAreYou();
        string Greatings { get; set; }
        int Tries { get; set; }
        bool Resolved { get; set; }
        string ToString();
    }
    public class MyService : IMyService
    {
        public string Greatings { get; set; }
        public int Tries { get; set; }
        public bool Resolved { get; set; }

        public MyService()
        {
        }
        public override string ToString()
        {
            return $"Type:{GetType().Name} Greatings:{Greatings} Tries:{Tries} Resolved: {Resolved}";
        }
        public string WhoAreYou()
        {
            var m = MethodBase.GetCurrentMethod().Name;
            var fullName = GetType().FullName;
            if (fullName == null) return null;
            var prstr = $"I'M {m} {this}";
            ConsoleSync.WriteLineT(prstr);
            return prstr;
        }
    }
    public interface IServicesContext
    {
        IMyService GetDeafaultService(string key);
    }
    public class ServicesContext : IServicesContext
    {
        public IMyService GetDeafaultService(string key)
        {
            return new MyService();
        }
        private static readonly Lazy<IServicesContext> Lazy = new Lazy<IServicesContext>(CreateInstance);
        public static IServicesContext Instance => Lazy.Value;
        private static IServicesContext CreateInstance()
        {
            return new ServicesContext();
        }
        public override string ToString()
        {
            return $"{GetType().FullName}";
        }
    }
}
