using System;
using System.Xml.Linq;
using GS.Configurations;
using GS.Elements;

namespace WebClients
{
    public class XmlWebRequester : Element1<string>, IConfigurationRequester
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
                var fxs = XmlWebClient?
                    .GetInStream(configurationKey, cnfItemKey);
                return fxs;
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
                var fxs =  XmlWebClient?
                    .GetInStream(configurationKey, cnfItemKey, cnfObjKey);
                return fxs;
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item,obj)","", ex);
            }
            return null;
        }
        public byte[] GetByteArray(string configurationKey, string cnfItemKey, string cnfObjKey)
        {
            try
            {
                var fab = XmlWebClient?
                    .GetInBytes(configurationKey, cnfItemKey, cnfObjKey);
                return fab;
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item,obj)", "", ex);
            }
            return null;
        }

        /// <summary>
        /// With Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configurationKey"></param>
        /// <param name="cnfItemKey"></param>
        /// <returns></returns>
        public XDocument Get(long token, string configurationKey, string cnfItemKey)
        {
            try
            {
                var fxs = XmlWebClient?
                    .GetInStream(token, configurationKey, cnfItemKey);
                return fxs;
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item)", "", ex);
            }
            return null;
        }

        public XDocument Get(long token, string configurationKey, string cnfItemKey, string cnfObjKey)
        {
            try
            {
                var fxs = XmlWebClient?
                    .GetInStream(token, configurationKey, cnfItemKey, cnfObjKey);
                return fxs;
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item,obj)", "", ex);
            }
            return null;
        }
        public byte[] GetByteArray(long token, string configurationKey, string cnfItemKey, string cnfObjKey)
        {
            try
            {
                var fab = XmlWebClient?
                    .GetInBytes(token, configurationKey, cnfItemKey, cnfObjKey);
                return fab;
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, typeof(XmlWebClient).ToString(), "Get(cnf,item,obj)", "", ex);
            }
            return null;
        }
        public override string Key => Code;
    }
}