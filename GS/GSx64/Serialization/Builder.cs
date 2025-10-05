using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Interfaces.Collections;

namespace GS.Serialization
{
    public class Builder
    {
        public static T Build<T>(string uri, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization<T>(uri, root);
                var u = t as IHaveUri;
                if (u != null)
                    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + uri.WithRight(" " + root) +  e.Message);
            }
            return t;
        }
        public static T Build2<T>(string uri, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization2<T>(uri, root);
                if(t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var u = t as IHaveUri;
                if (u != null)
                    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception(string.Join(" ", "Builder Desrialization Failure in", uri, root, e.Message));
            }
            return t;
        }
        public static T Build3<T>(string uri, string root) where T : class
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

        public static T Build2<T>(XDocument xdoc, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization2<T>(xdoc, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var x = t as IHaveXDocument;
                if (x != null)
                    x.XDocument = xdoc;
                //var u = t as IHaveUri;
                //if (u != null)
                //    // u.Uri = uri;
                //{
                //    var ts = GetElements(xdoc, u.XPath);

                //}
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in "  + root +  " " + e.Message);
            }
            return t;
        }
        public static T Build2<T, TKey, TItem>(XDocument xdoc, string root) 
            where T : class
            where TItem : class, IHaveKey<TKey>
        {
            T t;
            try
            {
                t = DeSerialization2<T>(xdoc, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                IHaveUri u = t as IHaveUri;
                IHaveCollection<TKey, TItem> collection = t as IHaveCollection<TKey, TItem>;
                //var coll2 = t as IElement21<TKey, TItem>;
                

                // if (u != null)
                    //u.Uri = uri;
                if(u != null && collection != null)
                {
                    var ts = GetElements(xdoc, u.XPath);
                    foreach (var i in ts)
                    {
                        var ty = Type.GetType(i.TypeName, false, true);
                        var s = Serialization.Do.DeSerialize(ty, i.XElement, null) as TItem;
                        if(s!=null)
                            collection.Register(s);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Builder DeSerialization Failure in " + root + " " + e.Message);
            }
            return t;
        }
        public static T BuildTypeWithCollectionInside<T, TKey, TItem>(XDocument xdoc, string root)
            where T : class, IHaveCollection<TKey, TItem>, IHaveUri
            where TItem : class, IHaveKey<TKey>
        {
            T t = null;
            try
            {
                t = DeSerialization2<T>(xdoc, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                if (t is IHaveUri uri && t is IHaveCollection<TKey, TItem> collection)
                {
                    var items = GetElements(xdoc, uri.XPath);
                    foreach (var i in items)
                    {
                        var ty = Type.GetType(i.TypeName, false, true);
                        if (Serialization.Do.DeSerialize(ty, i.XElement, null) is TItem s)
                            collection.Register(s);
                    }
                }
            }
            catch (Exception e)
            {
                 // throw new Exception("Builder DeSerialization Failure in " + root + " " + e.Message);
                 ConsoleSync.WriteLineT($@"{e}");
            }
            return t;
        }
        private static IEnumerable<XElementTypeInfo> GetElements(XContainer xdoc, string path)
        {
            if (xdoc == null)
                return null;

            //var x = xdoc.Element(path);
            var x = xdoc.Descendants(path).First();
            if (x == null)
                return null;
            if (!x.HasElements)
                return null;
            var xs = x.Elements();
            
            var xes = (from xe in xs
                       where xe.HasAttributes &&
                                ((xe.Attribute("enabled") == null) ||
                                (xe.Attribute("enabled") != null && xe.Attribute("enabled").Value == "true")) &&
                                xe.Attribute("ns") != null && // && xe.Attribute("ns").Value != null &&
                                xe.Attribute("as")!= null // && xe.Attribute("as").Value != null
                       select new XElementTypeInfo()
                       {
                           XElement = xe,
                           NameSpace = xe.Attribute("ns").Value,
                           Assembly = xe.Attribute("as").Value,
                           TypeName = xe.Attribute("ns").Value.Trim() + "." + xe.Name.ToString().Trim() + "," + xe.Attribute("as").Value.Trim()
                       })
                    .ToList();
            //foreach (var i in xes)
            //{
            //    var t = Type.GetType(i.TypeName, false, true);
            //    var s = Serialization.Do.DeSerialize(t, i.Xe, null);
            //}

            return xes;
        }
        // ns="GS.EventLog" as="GS.EventLog"
        // ns = "GS.Trade.Strategies"
        public static IEnumerable<string> GetTypeStrListEnumerable(
            string uri, string path, string namesp)
        {
            var xdoc = XDocument.Load(uri);
            var node = xdoc.Element(path);
            // var namespaceValue = node?.Attribute("ns")?.Value ?? namesp;
            if (node == null) yield break;
            if (!node.HasElements) yield break;
            
            foreach (var s in node.Elements())
            {
                var ns = s.Attribute("ns")?.Value ?? namesp;
                var str = ns + '.' + s.Name.ToString().Trim();
                yield return str;
            }
            //var ss = node.Elements();
            //// var list = ss.Select(xe => namespaceValue + '.' + xe.Name.ToString().Trim()).ToList();
            //foreach (var s in ss.Elements())
            //{
            //    var ns = s.Attribute("ns")?.Value ?? namesp;
            //    var str = ns + '.' + s.Name.ToString().Trim();
            //    yield return str;
            //}
        }
        public static IEnumerable<T> DeSerializeCollection<T>(string uri, string root)
            where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    throw new XmlException(root + " IS NOT FOUND");

                return DeSerializeCollection<T>(xDoc, root);
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + root + " " + e.Message);
            }
        }
        public static IEnumerable<T> DeSerializeCollection<T>(XContainer xdoc, string root)
            where T : class
        {
            try
            {
                var ts = GetElements(xdoc, root);
                if (!ts.Any())
                    return Enumerable.Empty<T>();
                var lst = new List<T>();
                foreach (var i in ts)
                {
                    var ty = Type.GetType(i.TypeName, false, true);
                    if (ty == null)
                        throw new Exception(string.Join(" ", "TypeName:", i.TypeName, "iS NOT FOUND"));

                    var s = Serialization.Do.DeSerialize(ty, i.XElement, null) as T;
                    if (s != null)
                        lst.Add(s);
                }
                return lst;
            }
            catch (Exception e)
            {
                throw new Exception("Builder Desrialization Failure in " + root + " " + e.Message);
            }
        }
        
        public static T DeSerialization<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);

                //   var typeName = GetType().Namespace + '.' + x.Name.ToString().Trim();
                var typeName = x.Name.ToString().Trim();
                //var t = Type.GetType(typeName, false, true);
                return GS.Serialization.Do.DeSerialize<T>(x, null);

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
        private static T DeSerialization2<T>(string uri, string root) where T : class
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
                    var tName = x.Name.ToString().Trim();
                    // Attributes namespace, assembly_name
                    var tNamespace = x.Attribute("ns");
                    var tAssemblyName = x.Attribute("as");

                    var typeName = (
                        tNamespace == null 
                            ? "" 
                            : tNamespace.Value.Trim() + ".") + tName + 
                                   (tAssemblyName == null 
                                        ? "" 
                                        : "," + tAssemblyName.Value.Trim());

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
        public static T Build21<T>(string uri, string root) where T : class
        {
            T t;
            try
            {
                t = DeSerialization21<T>(uri, root);
                if (t == null)
                    throw new NullReferenceException("Deserialization Failure");

                var u = t as IHaveUri;
                if (u != null)
                    u.Uri = uri;
            }
            catch (Exception e)
            {
                throw new Exception(string.Join(" ", "Builder Desrialization Failure in", uri, root, e.Message));
            }
            return t;
        }
        private static T DeSerialization21<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                //var x = xDoc.Descendants(root).FirstOrDefault();
                var x = (from xe in xDoc.Descendants(root)
                    where xe.Name.ToString() == root && xe.Attribute("enabled").Value == "true"
                    select xe).FirstOrDefault();
                                
                if (x == null)
                    return default(T);
                try
                {
                    var tn = x.Name.ToString();

                    var tns = x.Attribute("ns");
                    var ta = x.Attribute("as");
                    var cl = x.Attribute("cl");

                    var typeName = (tns == null ? "" : tns.Value.Trim() + ".") +
                                   (cl == null ? "" : cl.Value.Trim()) + 
                                   (ta == null ? "" : "," + ta.Value.Trim());

                    var t = Type.GetType(typeName, false, true);
                    if (t == null)
                        throw new NullReferenceException(typeName + " is Not Found");

                    x.Name = cl.Value.Trim();

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
        private static T DeSerialization3<T>(string uri, string root) where T : class
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
                    var tafn = x.Attribute("afn");

                    var typeName = ta == null
                        ? null
                        : (tns == null ? "" : tns.Value.Trim() + ".") + tn + "," + ta.Value.Trim();
                    var typeFullName = tafn == null
                       ? null
                       : (tns == null ? "" : tns.Value.Trim() + ".") + tn + "," + tafn.Value.Trim();
                    
                    if(typeName == null && typeFullName == null)
                        throw new NullReferenceException(root + " " + x.Name + " Attributes as & anf is Null");
                    
                    Type t = null;
                    if (typeName != null)
                    {
                        t = Type.GetType(typeName, false, true);
                    }
                    if (t == null)
                    {
                        if(typeFullName == null)
                            throw new NullReferenceException(root + " " + x.Name + " Attributes anf is Null");

                        t = Type.GetType(typeFullName, false, true) ?? Type.GetType(typeFullName, name =>
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
    }
}
