using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Moex.Interfaces;

namespace Moex
{
    public partial class MoexTest
    {
        public void Serialize<T>(XDocument doc, T entity)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            if (doc == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(doc)).ToString());
                return;
            }
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var writer = doc.CreateWriter();
                serializer.Serialize(writer, entity);
                writer.Close();
            }
            catch (Exception e) { SendMessage(methodname, e.ToString());}
        }
        public void Serialize<T>(XDocument doc, List<T> list)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            if (doc == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(doc)).ToString());
                return;
            }
            if (list == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(list)).ToString());
                return;
            }
            try
            {
                var serializer = new XmlSerializer(list.GetType());
                var writer = doc.CreateWriter();
                serializer.Serialize(writer, list);
                writer.Close();
            }
            catch (Exception e) { SendErrMessage(methodname, e.ToString()); }           
        }
        public T Deserialize<T>(XNode doc)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (doc == null)
            {
                SendErrMessage(m, new ArgumentNullException(nameof(doc)).ToString());
                return default(T);
            }
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var reader = doc.CreateReader();
                var result = (T)serializer.Deserialize(reader);
                reader.Close();
                return result;
            }
            catch (Exception e)
            {
                SendErrMessage(m, e.ToString());
                var typeStr = typeof(T).ToString();
                SendErrMessage(m, typeStr);
            }
            return default(T);
        }     
        public MoexTicker TickerDeserialize(XElement xelement)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            if (xelement == null)
            {
                SendMessage(method, new ArgumentNullException(nameof(xelement)).ToString());
                return null;
            }
            try
            {
                var moexSec = Deserialize<MoexTicker>(xelement);
                moexSec?.Init();
                SendMessage(method, moexSec?.ToString() ?? "Failure");
                return moexSec;
            }
            catch (Exception e)
            {
                SendMessage(method, e.ToString());
            }
            return null;
        }
        public IList<MoexTicker> TickersDeserialize(XElement xelements)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (xelements == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(xelements)).ToString());
                return null;
            }
            var lst = new List<MoexTicker>();
            try
            {
                foreach (var xe in xelements.Elements())
                {
                    var ticker = Deserialize<MoexTicker>(xe);
                    if (ticker == null)
                    {
                        SendMessage(methodname, "Failure");
                        continue;
                    }
                    ticker.Init();
                    lst.Add(ticker);
                }
                return lst;
            }
            catch (Exception e)
            {
                SendMessage(methodname, e.ToString());
            }
            return null;
        }
        public IList<MoexTicker> TickersDeserialize(XNode tickersxmlrows)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            if (tickersxmlrows == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(tickersxmlrows)).ToString());
                return null;
            }
            try
            {
                var tickersHolder = Deserialize<TickersHolder>(tickersxmlrows);
                if (tickersHolder == null) return null;
                if (tickersHolder.Items.Count == 0) return new List<MoexTicker>();
                var lst = tickersHolder.Items;
                tickersHolder.Clear();
                foreach (var t in lst) t?.Init();
                SendMessage(methodname, $"Ok.TickersCnt:{lst.Count}");
                return lst;
            }
            catch (Exception e)
            {
                SendMessage(methodname, e.ToString());
            }
            return null;
        }
        public IList<MoexTrade> TradesDeserialize(XNode tradesxmlrows)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            if (tradesxmlrows == null)
            {
                SendMessage(methodname, new ArgumentNullException(nameof(tradesxmlrows)).ToString());
                return null;
            }
            try
            {
                var tradeHolder = Deserialize<TradeHolder>(tradesxmlrows);
                if (tradeHolder == null) return null;
                if (!tradeHolder.Items.Any()) return new List<MoexTrade>();
                var lst = tradeHolder.Items;
                tradeHolder.Clear();
                foreach (var t in lst) t?.Init();
                return lst;
            }
            catch (Exception e) {SendMessage(methodname, e.ToString());}
            return null;
        }
        public IList<TItem> Deserialize<THolder, TItem>(XNode tradesxmlrows) 
            where THolder : IItemHolder<TItem>
            where TItem : IHaveInit
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            if (tradesxmlrows == null)
            {
                SendErrMessage(m, new ArgumentNullException(nameof(tradesxmlrows)).ToString());
                return null;
            }
            try
            {
                var tradeHolder = Deserialize<THolder>(tradesxmlrows);
                if (tradeHolder == null) return null;
                if (!tradeHolder.Items.Any()) return new List<TItem>();
                var lst = tradeHolder.Items;
                tradeHolder.Clear();
                foreach (var t in lst) t?.Init();
                return lst;
            }
            catch (Exception e) { SendErrMessage(m, e.ToString()); }
            return null;
        }
    }
}
