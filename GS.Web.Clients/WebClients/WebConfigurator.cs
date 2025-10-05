using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.Configurations;
using GS.Elements;
using GS.Serialization;

namespace WebClients
{
    //public interface IConfigurator
    //{
    //    void Init();
    //    XDocument Get(string configurationKey, string cnfItemKey);
    //    XDocument Get(string configurationKey, string cnfItemKey, string cnfObjKey);
    //}

    public class WebConfigurator : Element1<string>, IConfigurationResourse
    {
        //private XmlWebClient _webClient;

        public XmlWebClient XmlWebClient;
        public void Init()
        {
            try
            {
                //_webClient = Builder.Build<XmlWebClient>(@"Init\WebClients.xml", "XmlWebClient");
                XmlWebClient.Init();

            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof (XmlWebClient).ToString(), "Init()", "", ex);
                return;
            }
            
            //if (_webClient == null)
            //{
            //    SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Init()","",
            //        new NullReferenceException("XmlWebClient Build Error"));
            //    return;
            //}
            //_webClient.Init();
        }

        public XDocument Get(string configurationKey, string cnfItemKey)
        {
            try
            {
                return XmlWebClient == null
                ? null
                : XmlWebClient.GetInStream(configurationKey, cnfItemKey);
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item)", "", ex );
            }
            return null;
        }

        public XDocument Get(string configurationKey, string cnfItemKey, string cnfObjKey)
        {
            try
            {
                return XmlWebClient == null
                ? null
                : XmlWebClient.GetInStream(configurationKey, cnfItemKey, cnfObjKey);
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item,obj)","", ex);
            }
            return null;
        }

         public void LoadAssemblies()
         {
             
         }

         public override string Key{
            get { return Code; }
        }
    }
}

