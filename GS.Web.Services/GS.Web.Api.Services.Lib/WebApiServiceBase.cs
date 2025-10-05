using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using GS.Elements;
using GS.Interfaces;
using GS.Interfaces.Service;
using GS.Serialization;
using Microsoft.Owin.Hosting;

namespace GS.Web.Api.Services.Lib
{
    public class WebApiServiceBase  : Element1<string>, IService
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
                Evlm2(EvlResult.INFO, EvlSubject.INIT, ParentTypeName, TypeName, $"{m} Finish", Url, ToString());
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
        public void Serialize()
        {
            // var file = AppDomain.CurrentDomain.BaseDirectory + @"\Init\" + GetType().FullName + @".xml";
            var file = AppDomain.CurrentDomain.BaseDirectory + @"\Xml\" + GetType().Name + @".xml";
            // var file = @"D:\" + GetType().Name + @".xml";
            Do.Serialize<WebApiServiceBase>(file, this);
        }
        public WebApiServiceBase Deserialize()
        {
            var file = AppDomain.CurrentDomain.BaseDirectory + @"\Xml\" + GetType().Name + @".xml";
            var x = XDocument.Load(file);
            var el = x.Elements(GetType().Name).FirstOrDefault();
            var obj =  Do.DeSerialize(GetType(), el, null);
            return obj as WebApiServiceBase;
        }
    }
}
