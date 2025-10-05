using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Containers5;
using GS.Interfaces;

namespace GS.Elements
{
    public interface IElement2<TKey, TItem> : IElement1<TKey>
    {
        IEnumerable<TItem> Items { get; }
        TItem Register(TItem item);
        TItem GetByKey(TKey key);
        bool Remove(TKey key);
    }

    public abstract class Element2<TKey, TItem, TList> : Element1<TKey> , IElement2<TKey,TItem>, IHaveUri
        where TItem : class, IElement1<TKey>, IHaveKey<TKey>, IHaveInit
        where TList : IContainer<TKey, TItem>
        //where TList : ICollection<TItem>
    {
        [XmlIgnore]
        public TList Collection;

        public IEnumerable<TItem> Items => Collection.Items;

        public string Uri { get; set; }
        public string XPath { get; set; }
        public string UriXPath => Uri + "@" + XPath;

        public override void Init(IEventLog evl)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                EventLog = evl ?? throw new NullReferenceException("Init(evl == Null)");

                DeSerializationCollection(Uri, XPath, Collection);
                foreach (var s in Collection.Items)
                {
                   // s.Init(EventLog);
                    s.Parent = this;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        s.Name, s.GetType().ToString() , m, s.Code, s.ToString());
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullInfo, m, "", "", e);
                throw;
            }
        }
        private void DeSerializationCollection(string uri, string xpath, IContainer<TKey, TItem> tempList)
        {
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, "Element", "Begin DeSerialization()", uri, xpath);
            try
            {
                var xDoc = XDocument.Load(uri);
                var ss = xDoc.Descendants(xpath).FirstOrDefault();
                if (ss == null)
                    throw new NullReferenceException("Bad XPath?! -- nothing to Find");

                var xx = ss.Elements();
                foreach (var x in xx)
                {
                    try
                    {
                        try
                        {
                            var enabledAtt = x.Attribute("enabled");
                            if (enabledAtt != null && !Boolean.Parse(enabledAtt.Value))
                                continue;
                        }
                        catch (Exception e)
                        {
                            SendExceptionMessage3(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString(),e);
                            throw;
                        }
                        var tn = x.Name.ToString().Trim();

                        var tns = x.Attribute("ns");
                        var ta = x.Attribute("as");

                        var typeName = (tns == null ? "" : tns.Value.Trim() + ".") + tn + (ta == null ? "" : "," + ta.Value.Trim());
                        var t = Type.GetType(typeName, false, true);

                        var s = Serialization.Do.DeSerialize(t, x, null);

                        if (s == null)
                        {
                            //SendExceptionMessage3(FullName, UriXPath, "Deserialize().TypeName - Bad Syntax " + typeName, x.ToString(),e);
                            //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                            throw new NullReferenceException("Deserialize().TypeName - Bad Syntax " + typeName);
                            //continue;
                        }

                        tempList.Add((TItem)s);
                        Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", s.ToString(), x.ToString());
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName, UriXPath, "DeserializeCollection().Element Failure", x.ToString(), e);
                        throw;
                    }
                }
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Storage", "End DeSerialization(). Count=" + tempList.Count, uri, xpath);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, UriXPath, "DeserializeCollection()","" ,e);
                throw;
            }
            //return true;
        }

        public virtual TItem Register(TItem item)
        {
            return Collection.AddOrGet(item);
        }

        public virtual TItem GetByKey(TKey key)
        {
            return Collection.GetByKey(key);
        }
        public virtual bool Remove(TKey key)
        {
            return Collection.Remove(key);
        }

        public virtual void GetNotifyEvent(Events.IEventArgs ea)
        {
        }
    }

    public class Test : Element2<string, TestItem , Containers5.DictionaryContainer<string, TestItem>>
    {
        public override string Key => Code;

        public Test()
        {
            Collection = new DictionaryContainer<string, TestItem>();
        }
    }
    public class TestItem : Element1<string>, IHaveKey<string>, IHaveInit
    {
        public override string Key => Code;

        public override void Init(IEventLog ievl)
        {
            base.Init(ievl);
        }
    }
}
