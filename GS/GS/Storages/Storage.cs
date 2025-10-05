using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Containers5;
using GS.Interfaces;
using GS.Elements;

namespace GS.Storages
{
    public abstract class Storage2< TKey, TItem, TList>
        where TItem : class, IHaveKey<TKey>
    {
    }
    public abstract class Storage21<TKey, TItem> : Element21<TKey, TItem, Containers5.DictionaryContainer<TKey,TItem>>
        where TItem : class, IElement1<TKey>, IHaveInit, IHaveKey<string>

    {
    }

    public abstract class Storage<TKey, TItem> : Element1<TKey>, IHaveUri
        where TItem : IElement1<TKey>, IHaveKey<TKey>, IHaveInit
        //where TList : Containers5.Container<TList, TItem, TKey> //where TList : ICollection<TItem>
    {
        public string Uri { get; set; }
        public string XPath { get; set; }
        public string UriXPath { get { return Uri + "@" + XPath;}}

        [XmlIgnore]
        public IContainer<TKey, TItem> Items;

        //[XmlIgnore]
        //public List<TItem> StorageList { get; set; }

        protected Storage()
        {
            //StorageList = new List<TItem>();
            //var t = typeof (TList);
            //var o = Activator.CreateInstance(t);
            //Items = o as Containers5.Container<TList, TItem, TKey>;
        }
        public override void Init(IEventLog evl)
        {
            try
            {
                if (evl == null)
                    throw new NullReferenceException("Init(evl == Null)");
                EventLog = evl;

                DeSerializationCollection(Uri, XPath, Items);
                foreach (var s in Items.Items)
                {
                    s.Init(EventLog);
                    s.Parent = this;
                    Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        s.Name, "Storage", "Init", s.Code, s.ToString());
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name, "Storage.Init()", "", "", e);
                throw;
            }
        }

        private bool DeSerializationCollection(string uri, string xpath, IContainer<TKey, TItem> tempList)
        {
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Storage", "Begin DeSerialization()", uri , xpath );
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
                            SendExceptionMessage3(Name, UriXPath, "Deserialize() Enabled Attribut - Bad Syntax", x.ToString(), e);
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
                            var e = new NullReferenceException();
                            SendExceptionMessage3(Name, UriXPath ,"Deserialize().TypeName - Bad Syntax " + typeName, x.ToString(),e);
                            //Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, uri, typeName, "DeSerializationItem() is Failure ", x.ToString(), "");
                            continue;
                        }

                        tempList.Add((TItem) s);
                        Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, UriXPath, typeName, "DeSerialization().AddElement", s.ToString() , x.ToString());
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(UriXPath, "Storage.DeserializeCollection().Element Failure", "", "", e);
                        throw;
                    }
                }
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Storage", "End DeSerialization(). Count="+ tempList.Count, uri, xpath);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Name + "." + UriXPath, "Storage.DeserializeCollection()", "", "", e);
                throw;
            }
            return true;
        }
    }

    public abstract class StorageItem<TKey> : Element1<TKey>
    {
    }

    //public class TradeStorage : Storage<string, TestItem >
    //{
    //    public TradeStorage()
    //    {
    //        //var t = typeof(TList);
    //        //var o = Activator.CreateInstance(t);
    //        //Items = o as Containers5.Container<TList, TestItem, string>;
    //        Items = new Containers5.DictionaryContainer<string, TestItem>();
    //    }
    //    public override string Key
    //    {
    //        get { return Code; }
    //    }
    //}
}
