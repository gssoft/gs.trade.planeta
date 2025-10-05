using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Assemblies;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Configurations;
using GS.Serialization;
using GS.Xml;
//using WebClients;

namespace GS.Configurations
{
    public class ConfigurationResourse : Element1<string>, IConfigurationResourse2
    {
        public string ConfigurationKey { get; set; }
        public long Token { get; set; }
        public string RequesterInitPath { get; set; }
        public string RequesterInitName { get; set; }

        [XmlIgnore]
        public IConfigurationRequester Requester { get; set; }
        //[XmlIgnore]
        //public XmlWebRequester Requester { get; set; }

        private AssemblyInfoList AssList { get; set; }

        public void Init()
        {
            Requester = Builder.Build2<IConfigurationRequester>(RequesterInitPath, RequesterInitName);
            //Requester = Build2<XmlWebRequester>(RequesterInitPath, RequesterInitName);
            if (Requester == null)
                throw new NullReferenceException("ConfigurationResourseRequester is Null");
            Requester.Init();

           // Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, Code, "Init()", ConfigurationKey + ": Init()", "");
        }
        private static readonly Lazy<ConfigurationResourse> Lazy =
        new Lazy<ConfigurationResourse>(() => CreateInstance());
        public static ConfigurationResourse Instance { get { return Lazy.Value; } }

        private static ConfigurationResourse CreateInstance()
        {
            var instance = Builder.Build2<ConfigurationResourse>(@"Init\ConfigurationResourse.xml",
                        "ConfigurationResourse");
            if(instance==null)
                throw new NullReferenceException("Build ConfigurationResource Failure");
            instance.Init();
            return instance;
        }

        public XDocument Get(string configurationItem)
        {
            if (Requester != null 
                    && ConfigurationKey.HasValue() && configurationItem.HasValue())
                return Requester.Get(Token, ConfigurationKey, configurationItem);
            return null;
        }

        public XDocument Get(string configurationItem, string configurationObject)
        {
            if (Requester != null
                    && ConfigurationKey.HasValue() && configurationItem.HasValue() && configurationObject.HasValue())
                return Requester.Get(Token, ConfigurationKey, configurationItem, configurationObject);
            return null;
        }
        public byte[] GetByteArr(string configurationItem, string configurationObject)
        {
            if (Requester != null
                    && ConfigurationKey.HasValue() && configurationItem.HasValue() && configurationObject.HasValue())
                return Requester.GetByteArray(Token, ConfigurationKey, configurationItem, configurationObject);
            return null;
        }

        public void LoadAssemblies()
        {
            if (Requester == null)
            {
                // SendExc
                return;
            }
            try
            {
                var xDoc = Get("Assemblies");
                if (xDoc == null)
                {
                    SendExceptionMessage3(Code, GetType().FullName, "GetAssemblyList()", "",
                            new NullReferenceException("Failure in Assembly List Load"));
                    return;
                }
                var assInfoList = Builder.Build2<AssemblyInfoList>(xDoc, "AssemblyInfoList");

                var assToLoad = assInfoList.Assemblies.Where(a => a.Enabled).ToList();
                foreach (var strat in assToLoad)
                {
                    //var strat = assInfoList.GetByKey("GS.Trade.Strategies");

                    if (Assemblies.Assemblies.IsAssemblyFullNameLoaded(strat.FullName))
                        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Strategies", strat.FullName, "Loaded", "");
                    else
                    {
                        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Strategies", strat.FullName,
                            "Does Not Loaded", "");

                        foreach (var dk in strat.Dependencies)
                        {
                            var asi = assInfoList.GetByKey(dk);
                            if (Assemblies.Assemblies.IsAssemblyFullNameLoaded(asi.FullName))
                                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, strat.Key, "Dependencies", asi.FullName,
                                    "Loaded", "");
                            else
                            {
                                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, strat.Key, "Dependencies", asi.FullName,
                                    "Does Not Loaded", "");
                                var barr = GetByteArr("Assemblies", asi.Path);
                                if (barr == null)
                                {
                                    SendExceptionMessage3(Code, GetType().FullName, "GetAssFromWeb", asi.Key,
                                        new NullReferenceException("Failure in Assembly.Load()"));
                                }
                                var a = Assembly.Load(barr);
                                if (a != null)
                                {
                                    if (GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(asi.FullName))
                                    {
                                        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, strat.Key, "Dependencies",
                                            asi.Key, "Loaded", asi.Path);
                                    }
                                    else
                                        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, strat.Key, "Dependencies", asi.Key,
                                            "Failure in Load", asi.Path);
                                }
                            }
                        }
                        if (assInfoList.IsAllDependenciesLoaded(strat))
                        {
                            var barr = GetByteArr("Assemblies", strat.Path);
                            if (barr == null)
                            {
                                SendExceptionMessage3(Code, GetType().FullName, "GetAssFromWeb", strat.Key,
                                    new NullReferenceException("Failure in Assembly.Load()"));
                            }
                            var a = Assembly.Load(barr);
                            if (a != null)
                            {
                                if (GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(strat.FullName))
                                {
                                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, strat.Key, "Assembly", strat.Key,
                                        "Loaded", strat.Path);
                                }
                                else
                                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, strat.Key, "Assembly", strat.Key,
                                        "Failure in Load", strat.Path);
                            }
                        }
                    }
                }
                var aa = 1;
                
                /* 15.11.14
                var lstPaths = xDoc.GetElementValues("ArrayOfString", "string");
                if (lstPaths == null)
                    return;
                foreach (var p in lstPaths)
                {
                    var fname = Path.GetFileNameWithoutExtension(p);
                    if (GS.Assemblies.Assemblies.IsAssemblyShortNameLoaded(p))
                    {
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Already Loaded", p);
                        continue;
                    }
                    // var a = Assembly.LoadFrom(p);
                    var barr = GetByteArr("Assemblies", p);
                    if (barr == null)
                    {
                        SendExceptionMessage3(Code, GetType().FullName, "GetAss", fname,
                            new NullReferenceException("Failure in Assembly.Load()"));
                    }
                    var a = Assembly.Load(barr);
                    if (a != null)
                    {
                        if (GS.Assemblies.Assemblies.IsAssemblyShortNameLoaded(p))
                        {
                            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Loaded", p);
                        }
                        else
                            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Failure in Load", p);
                    }
                }
                */
                // foreach(var a in Assemblies.Assemblies.GetAssembliesLoaded())
                //     Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, a.FullName, a.Location, a.GetType().ToString(), a.CodeBase);
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, GetType().FullName, "LoadAssemblies()", "" , ex);
            }
        }
        public void LoadAssemblies1()
        {
            if (Requester == null)
            {
                // SendExc
                return;
            }
            try
            {
                var xDoc = Get("Assemblies");
                if (xDoc == null)
                {
                    SendExceptionMessage3(Code, GetType().FullName, "GetAssemblyList()", "",
                            new NullReferenceException("Failure in Assembly List Load"));
                    return;
                }
                var lstPaths = xDoc.GetElementValues("ArrayOfString", "string");
                if (lstPaths == null)
                    return;
                foreach (var p in lstPaths)
                {
                    var fname = Path.GetFileNameWithoutExtension(p);
                    if (GS.Assemblies.Assemblies.IsAssemblyShortNameLoaded(p))
                    {
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Already Loaded", p);
                        continue;
                    }
                    // var a = Assembly.LoadFrom(p);
                    var barr = GetByteArr("Assemblies", p);
                    if (barr == null)
                    {
                        SendExceptionMessage3(Code, GetType().FullName, "GetAss", fname,
                            new NullReferenceException("Failure in Assembly.Load()"));
                    }
                    var a = Assembly.Load(barr);
                    if (a != null)
                    {
                        if (GS.Assemblies.Assemblies.IsAssemblyShortNameLoaded(p))
                        {
                            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Loaded", p);
                        }
                        else
                            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, Code, "Assembly", fname, "Failure in Load", p);
                    }
                }

                // foreach(var a in Assemblies.Assemblies.GetAssembliesLoaded())
                //     Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, a.FullName, a.Location, a.GetType().ToString(), a.CodeBase);
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, GetType().FullName, "LoadAssemblies()", "", ex);
            }
        }

        public override string Key { get {
                return Code.HasValue()
                    ? Code
                    : GetType().ToString();
            }
        }
    }
}
