using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Assemblies;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Xml;

namespace GS.Configurations
{
    public class ConfigurationResourse1 : Element1<string>, IConfigurationResourse21
    {
        public string ConfigurationKey { get; set; }
        public long Token { get; set; }
        public string RequesterInitPath { get; set; }
        public string RequesterInitName { get; set; }

        [XmlIgnore]
        public IConfigurationRequester Requester { get; set; }
        //[XmlIgnore]
        //public XmlWebRequester Requester { get; set; }

        private AssemblyInfoList AssInfoList { get; set; }

        public void Init()
        {
            Requester = Build2<IConfigurationRequester>(RequesterInitPath, RequesterInitName);
            //Requester = Build2<XmlWebRequester>(RequesterInitPath, RequesterInitName);
            if (Requester == null)
                throw new NullReferenceException("ConfigurationResourseRequester is Null");
            Requester.Init();

            // Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, Code, "Init()", ConfigurationKey + ": Init()", "");
        }
        private static readonly Lazy<ConfigurationResourse1> Lazy =
            new Lazy<ConfigurationResourse1>(() => CreateInstance());
        public static ConfigurationResourse1 Instance { get { return Lazy.Value; } }

        private static ConfigurationResourse1 CreateInstance()
        {
            var instance = Builder.Build2<ConfigurationResourse1>(@"Init\ConfigurationResourse1.xml",
                "ConfigurationResourse1");
            if (instance == null)
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
// *************************************************************************
// **************************************************************************
// ***************************************************************************
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
                SendExceptionMessage3(Code, GetType().FullName, "LoadAssemblies()", "", ex);
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
        // 30.09.22
        public bool LoadAssembly(string key)
        {
            return true;

            FillAssembliesList();

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, Code, "Assemblies", key, "Try To Load", "");

            var asinfo = AssInfoList.GetByKey(key);
            if (asinfo == null)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Code, "Assemblies", key, "Is Absent in AssInfoList ToLoad", "");
                return true;
            }
            if (Assemblies.Assemblies.IsAssemblyFullNameLoaded(asinfo.FullName))
            {
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Assemblies", asinfo.FullName, "Loaded", "");
                return true;
            }
            if (!AssInfoList.IsAllDependenciesLoaded(asinfo))
            {
                foreach (var dkey in asinfo.Dependencies)
                {
                    var dai = AssInfoList.GetByKey(dkey);

                    if (Assemblies.Assemblies.IsAssemblyFullNameLoaded(dai.FullName))
                        continue;

                    if (LoadAssembly(dkey))
                        continue;
                    // Fail To Load
                    return false;
                }
            }
            else
            {
                var barr = GetByteArr("Assemblies", asinfo.Path);
                if (barr == null)
                {
                    SendExceptionMessage3(Code, GetType().FullName, "GetAssFromWeb", asinfo.Key,
                        new NullReferenceException("Failure in Assembly.Load()"));
                    return false;
                }
                var a = Assembly.Load(barr);
                if (a != null)
                {
                    if (GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(asinfo.FullName))
                    {
                        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, asinfo.Key, "Assembly", asinfo.Key,
                            "Loaded", asinfo.Path);
                        return true;
                    }
                    
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, asinfo.Key, "Assembly", asinfo.Key,
                            "Failure in Loading", asinfo.Path);
                    return false;
                }
            }
            return true;
        }

        private void FillAssembliesList()
        {
            if (AssInfoList != null) return;

            var xDoc = Get("Assemblies");
            if (xDoc == null)
            {
                AssInfoList = new AssemblyInfoList();
                SendExceptionMessage3(Code, GetType().FullName, "GetAssemblyList()", "",
                    new NullReferenceException("Failure in Assembly List Load"));
                return;
            }
            AssInfoList = Builder.Build2<AssemblyInfoList>(xDoc, "AssemblyInfoList");
        }

        public T Build<T>(string configurationItem, /* string uri,*/ string root) where T : class
        {
            var xdoc = Get(configurationItem);
            if (xdoc == null)
                return null;
            T t;
            try
            {
                t = DeSerialization<T>(xdoc, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var x = t as IHaveXDocument;
                if (x != null)
                    x.XDocument = xdoc;
                //var u = t as IHaveUri;
                //if (u != null)
                //    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + root + " " + e.Message);
            }
            return t;
        }
        private T DeSerialization<T>(XContainer xDoc, string root) where T : class
        {
            try
            {
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);
                try
                {
                    try
                    {
                        var enabledAtt = x.Attribute("enabled");
                        if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                            throw new NullReferenceException("xDoc Enable attribute is Null or Disabled");
                    }
                    catch (Exception e)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString());
                        throw new NullReferenceException();
                    }

                    // 15.11.26
                    //bool loadFromAssemblyInMemory = false;
                    //var loadAttr = x.Attribute("load");
                    //if (loadAttr != null && loadAttr.Value == "runtime")
                    //    loadFromAssemblyInMemory = true;

                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");
                    if (ta == null)
                        throw new NullReferenceException(root + " " + x.Name + " Attribut as == null");
                    var assShortName = ta.Value.Trim();

                    var tafn = x.Attribute("afn");
                    if(tafn == null)
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Code, assShortName, "Get Assembly Full Name", "Attribut afn == null", "");
                    
                    // 23.09.30
                    // if (!LoadAssembly(assShortName))
                    //    throw new NullReferenceException(root + " " + x.Name + " " + assShortName + " Failure in Loading");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + "," + assShortName;

                    var typeFullName = tafn == null 
                        ? null 
                        : (tns == null ? "" : tns.Value.Trim() + ".") + tn + "," + tafn.Value.Trim();
                    
                    Type t;
                    t = Type.GetType(typeName, false, true);
                    if (t == null)
                    {
                        if (typeFullName == null)
                        {
                            // for Assemblies in AssInfoList with Assemblies FullName
                            var asfname = AssInfoList.GetFullNameByKey(assShortName);
                            if (asfname == null)
                            {
                                throw new NullReferenceException(root + " " + x.Name + " " + assShortName + " " + Key +
                                                                 ": Assembly FullName is Not Found");
                            }
                            typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + "," + asfname;
                        }
                        else
                            typeName = typeFullName;

                        t = Type.GetType(typeName, false, true) ?? Type.GetType(typeName, name =>
                            AppDomain.CurrentDomain.GetAssemblies()
                                .FirstOrDefault(z => z.FullName == name.FullName),
                            null,
                            true);
                    }

                    if (t == null)
                        throw new NullReferenceException(typeName + " is Not Found");

                    var s = Serialization.Do.DeSerialize<T>(t, x, null);

                    if (s == null)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString());
                        //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                        //continue;
                        throw new NullReferenceException(t + ": Deserialization Failure");
                    }

                    //tempList.Add((TItem)s);
                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", x.ToString(), s.ToString());
                    return s;
                }
                catch (Exception e)
                {
                    //SendExceptionMessage(UriXPath, "Storage.DeserializeCollection().Element Failure", e.Message, e.Source);
                    throw new NullReferenceException();
                }


                //var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                //return GS.Serialization.Do.DeSerialize<T>(t, x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }

        // Builds with Build from GS
        public T Build<T, TKey, TItem>(string configurationItem, string root)
            where T : class
            where TItem : class, IHaveKey<TKey>
        {
            var xdoc = Get(configurationItem);
            if (xdoc == null)
                return null;

            return Builder.Build2<T, TKey, TItem>(xdoc, root);
        }

        public T Build2<T>(string uri, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization2<T>(uri, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var u = t as IHaveUri;
                if (u != null)
                    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + uri.WithRight(" " + root) + e.Message);
            }
            return t;
        }
        private T DeSerialization2<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);
                try
                {
                    try
                    {
                        var enabledAtt = x.Attribute("enabled");
                        if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                            throw new NullReferenceException();
                    }
                    catch (Exception e)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString());
                        throw new NullReferenceException();
                    }
                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + (ta == null ? "" : "," + ta.Value.Trim());
                    var t = Type.GetType(typeName, false, true);
                    if (t == null)
                        throw new NullReferenceException(typeName + " is Not Found");

                    var s = Serialization.Do.DeSerialize<T>(t, x, null);

                    if (s == null)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString());
                        //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                        //continue;
                        throw new NullReferenceException(t + ": Deserialization Failure");
                    }

                    //tempList.Add((TItem)s);
                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", x.ToString(), s.ToString());
                    return s;
                }
                catch (Exception e)
                {
                    //SendExceptionMessage(UriXPath, "Storage.DeserializeCollection().Element Failure", e.Message, e.Source);
                    throw new NullReferenceException();
                }


                //var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                //return GS.Serialization.Do.DeSerialize<T>(t, x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }

        public T Build3<T>(string uri, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization3<T>(uri, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var u = t as IHaveUri;
                if (u != null)
                    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + uri.WithRight(" " + root) + e.Message);
            }
            return t;
        }
        private static T DeSerialization2<T>(XContainer xDoc, string root) where T : class
        {
            try
            {
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);
                try
                {
                    try
                    {
                        var enabledAtt = x.Attribute("enabled");
                        if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                            throw new NullReferenceException();
                    }
                    catch (Exception e)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString());
                        throw new NullReferenceException();
                    }
                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + (ta == null ? "" : "," + ta.Value.Trim());
                    var t = Type.GetType(typeName, false, true);

                    var s = Serialization.Do.DeSerialize<T>(t, x, null);

                    if (s == null)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString());
                        //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                        //continue;
                        throw new NullReferenceException();
                    }

                    //tempList.Add((TItem)s);
                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", x.ToString(), s.ToString());
                    return s;
                }
                catch (Exception e)
                {
                    //SendExceptionMessage(UriXPath, "Storage.DeserializeCollection().Element Failure", e.Message, e.Source);
                    throw new NullReferenceException();
                }


                //var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                //return GS.Serialization.Do.DeSerialize<T>(t, x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }
       
        private T DeSerialization3<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);
                try
                {
                    try
                    {
                        var enabledAtt = x.Attribute("enabled");
                        if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                            throw new NullReferenceException();
                    }
                    catch (Exception e)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString());
                        throw new NullReferenceException();
                    }

                    // 15.11.26
                    //bool loadFromAssemblyInMemory = false;
                    //var loadAttr = x.Attribute("load");
                    //if (loadAttr != null && loadAttr.Value == "runtime")
                    //    loadFromAssemblyInMemory = true;

                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + (ta == null ? "" : "," + ta.Value.Trim());

                    Type t;
                    //if( !loadFromAssemblyInMemory)
                    //    t = Type.GetType(typeName, false, true);
                    //else
                    //    t = Type.GetType(typeName,
                    //    name => 
                    //        AppDomain.CurrentDomain.GetAssemblies()
                    //                    .FirstOrDefault(z => z.FullName == name.FullName),
                    //    null,
                    //    true);

                    t = Type.GetType(typeName, false, true);
                    if (t == null)
                    {
                        t = Type.GetType(typeName, name =>
                            AppDomain.CurrentDomain.GetAssemblies()
                                .FirstOrDefault(z => z.FullName == name.FullName),
                            null,
                            true);
                    }

                    if (t == null)
                        throw new NullReferenceException(typeName + " is Not Found");

                    var s = Serialization.Do.DeSerialize<T>(t, x, null);

                    if (s == null)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString());
                        //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                        //continue;
                        throw new NullReferenceException(t + ": Deserialization Failure");
                    }

                    //tempList.Add((TItem)s);
                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", x.ToString(), s.ToString());
                    return s;
                }
                catch (Exception e)
                {
                    //SendExceptionMessage(UriXPath, "Storage.DeserializeCollection().Element Failure", e.Message, e.Source);
                    throw new NullReferenceException();
                }


                //var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                //return GS.Serialization.Do.DeSerialize<T>(t, x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }
        private T DeSerialization31<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);
                try
                {
                    try
                    {
                        var enabledAtt = x.Attribute("enabled");
                        if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                            throw new NullReferenceException();
                    }
                    catch (Exception e)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString());
                        throw new NullReferenceException();
                    }

                    // 15.11.26
                    //bool loadFromAssemblyInMemory = false;
                    //var loadAttr = x.Attribute("load");
                    //if (loadAttr != null && loadAttr.Value == "runtime")
                    //    loadFromAssemblyInMemory = true;

                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + (ta == null ? "" : "," + ta.Value.Trim());

                    Type t;
                    //if( !loadFromAssemblyInMemory)
                    //    t = Type.GetType(typeName, false, true);
                    //else
                    //    t = Type.GetType(typeName,
                    //    name => 
                    //        AppDomain.CurrentDomain.GetAssemblies()
                    //                    .FirstOrDefault(z => z.FullName == name.FullName),
                    //    null,
                    //    true);

                    t = Type.GetType(typeName, false, true);
                    if (t == null)
                    {
                        t = Type.GetType(typeName, name =>
                            AppDomain.CurrentDomain.GetAssemblies()
                                .FirstOrDefault(z => z.FullName == name.FullName),
                            null,
                            true);
                    }

                    if (t == null)
                        throw new NullReferenceException(typeName + " is Not Found");

                    var s = Serialization.Do.DeSerialize<T>(t, x, null);

                    if (s == null)
                    {
                        //SendExceptionMessage(Name, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString());
                        //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                        //continue;
                        throw new NullReferenceException(t + ": Deserialization Failure");
                    }

                    //tempList.Add((TItem)s);
                    //Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", x.ToString(), s.ToString());
                    return s;
                }
                catch (Exception e)
                {
                    //SendExceptionMessage(UriXPath, "Storage.DeserializeCollection().Element Failure", e.Message, e.Source);
                    throw new NullReferenceException();
                }


                //var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                //return GS.Serialization.Do.DeSerialize<T>(t, x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }
        public override string Key =>
            Code.HasValue()
                ? Code
                : GetType().ToString();
    }
}