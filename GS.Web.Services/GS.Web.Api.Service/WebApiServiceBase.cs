using System;
using System.Reflection;
using GS.Elements;
using GS.Interfaces;
using GS.Interfaces.Service;
using Microsoft.Owin.Hosting;

namespace GS.Web.Api.Service01
{
    public class WebApiServiceBase : Element1<string>, IService
    {
        protected IDisposable WebApi;
        public string BaseAddress { get; set; } = "http://localhost";
        public int Port { get; set; } = 8082;
        public string Url  { get; set; }  = "http://localhost:8082";

        public void Start()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m} Begin", Url, ToString());
                WebApi?.Dispose();
                WebApi = WebApp.Start<Startup>(Url);
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m} Finish", Url, ToString() );
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, m, "Exception", ex.Message);
                Stop();
            }
        }
        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m} Begin", Url, ToString());
                WebApi?.Dispose();
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m} Finish", Url, ToString());
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, m, "Exception", ex.Message);
            }
        }
        public override string Key => Code ?? TypeFullName;
    }
}
