using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Extension;
using GS.Moex.Interfaces;
// using GS.Trade.Moex;

namespace Moex
{
    public partial class MoexTest
    {
        private string GetTickersApiPrefix(MoexTickerTypeEnum tickerType)
        {
            switch (tickerType)
            {
                case MoexTickerTypeEnum.Futures:
                    return TickersFuturesApiPrefix;
                case MoexTickerTypeEnum.Option:
                    return TickersOptionsApiPrefix;
                default:
                    return string.Empty;
            }
        }
        private string GetTickersApiPrefix(string board)
        {
            switch (board.TrimUpper())
            {
                case "RFUD": // Moex Futures
                case "SPBFUT":
                    return TickersFuturesApiPrefix; // Quik Futures
                case "ROPD": // Moex Options
                case "SPBOPT":
                    return TickersOptionsApiPrefix; // Quik Options
                default:
                    return string.Empty;
            }
        }
        public string CreateTickersLink(MoexTickerTypeEnum tickerType)
        {
            var instrPrefix = GetTickersApiPrefix(tickerType);
            if (instrPrefix.HasNoValue()) return string.Empty;
            return instrPrefix + ApiPostfix; // instr = option/futures, post = .xml
        }
        public string CreateTickersLink(string board)
        {
            var instrPrefix = GetTickersApiPrefix(board);
            if (instrPrefix.HasNoValue()) return string.Empty;
            return instrPrefix + ApiPostfix; // instr = option/futures, post = .xml
        }
        public XNode GetTickersXmlDoc(string board)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (board.HasNoValue()) return null;
            try
            {
                var link = CreateTickersLink(board);
                if (link.HasNoValue()) return null;

                var xmlstrm = WebClient02.GetItemInStream(link);
                var xdoc = XDocument.Load(xmlstrm);
                return xdoc;
            }
            catch (Exception e)
            {
                SendErrMessage(methodname, e.ToString());
                //erraction?.Invoke(e.ToString());
            }
            return null;
        }

        public IList<MoexTicker> GetTickers(string board)
        {
            return GetItems<TickersHolder, MoexTicker>(board);
        }
        public IList<MoexTicker> GetTickers2(string board)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var xnode = GetTickersXmlDoc(board); // Get XDocument
                if (xnode == null) return null;
                var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
                if (tickersxmlrows == null) return null;
                if (!tickersxmlrows.HasElements) return new List<MoexTicker>();
                var holder = Deserialize<TickersHolder>(tickersxmlrows);
                return holder == null ? null : UnPackHolder<TickersHolder, MoexTicker>(holder);
                //if (!holder.Items.Any()) return new List<MoexTicker>();
                //var lst = holder.Items;
                //holder.Clear();
                //InitItems(lst);
            }
            catch (Exception e)
            {
               SendErrMessage(method, e.ToString()); 
            }
            return null;
        }      
        //public IList<MoexTicker> GetTickers1(string board)
        //{
        //    var xnode = GetTickersXmlDoc(board); // Get XDocument
        //    if (xnode == null) return null;
        //    var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
        //    if (tickersxmlrows == null) return null;
        //    if (!tickersxmlrows.HasElements) return new List<MoexTicker>();
        //    // var lst = TickersDeserialize(tickersxmlrows); // Every Ticker
        //    // var lst = TickersDeserialize((XNode)tickersxmlrows); // List<Ticker>
        //    var lst = Deserialize<TickersHolder, MoexTicker>(tickersxmlrows);
        //    return lst;
        //}
        //public IList<MoexTicker> GetTickers2(string board)
        //{
        //    var xnode = GetTickersXmlDoc(board, null); // Get XDocument
        //    if (xnode == null) return null;
        //    var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
        //    if (tickersxmlrows == null) return null;
        //    if (!tickersxmlrows.HasElements) return new List<MoexTicker>();
        //    var tickersHolder = Deserialize<TickersHolder>(tickersxmlrows, null);
        //    // DeSerialize XElement with Ticker Rows
        //    if (tickersHolder == null) return null;
        //    if (tickersHolder.Items.Count == 0) return new List<MoexTicker>();
        //    var lst = tickersHolder.Items;
        //    tickersHolder.Clear();
        //    foreach (var t in lst) t?.Init();
        //    return lst;
        //}
        public IEnumerable<XElement> GetTickersXmlElements(string board)
        {
            var xnode = GetTickersXmlDoc(board); // Get XDocument
            if (xnode == null) return null;
            var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
            if (tickersxmlrows == null) return null;
            if (!tickersxmlrows.HasElements) return null;
            var elements = tickersxmlrows.Elements();
            return elements;
        }
        public IList<MoexTicker> GetTickers(string board, Expression<Func<MoexTicker, bool>> predicate)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var tickers = GetTickers(board);
                if (tickers != null) return tickers.Where(predicate.Compile())?.ToList();
                SendErrMessage(method, "Something wrong with Tickers Receiving. Tickers is null");
                return null;
            }
            catch (Exception e)
            {
                SendErrMessage(method, e.ToString());
            }
            return null;
        }
        public IList<MoexTicker> GetTickers1(string board, Expression<Func<MoexTicker, bool>> predicate)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var xnode = GetTickersXmlDoc(board); // Get XDocument From Web Moex.ISS
                if (xnode == null) return null;
                var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
                if (tickersxmlrows == null) return null;
                if (!tickersxmlrows.HasElements) return new List<MoexTicker>();
                var tickersHolder = Deserialize<TickersHolder>(tickersxmlrows);
                // DeSerialize XElement with Ticker Rows
                if (tickersHolder == null) return null;
                if (tickersHolder.Items.Count == 0) return new List<MoexTicker>();
                var lst = tickersHolder.Items;
                tickersHolder.Clear();
                foreach (var t in lst) t?.Init();
                return lst.Where(predicate.Compile())?.ToList(); // .Select(g => g);
            }
            catch (Exception e)
            {
                SendErrMessage(method, e.ToString());
            }
            return null;
        }
        public IMoexTicker GetCurrentFuturesContract(string assetcode, Action<string> err)
        {
            if (assetcode.HasNoValue())
            {
                err?.Invoke($"Wrong AssetCode:{assetcode}");
                return null;
            }
            try
            {
                var tickers = GetTickers("SPBFUT");
                return tickers
                        .Where(t => t.AssetCode == assetcode && t.LastTradeDate > DateTime.Now.Date)
                        .OrderBy(t => t.LastTradeDate).FirstOrDefault();
            }
            catch (Exception e)
            {
                err?.Invoke(e.ToString());
            }
            return null;
        }
        public IMoexTicker GetCurrentFuturesContractSmart(string assetcode, Action<string> err)
        {
            if (assetcode.HasNoValue())
            {
                err?.Invoke($"Wrong AssetCode:{assetcode}");
                return null;
            }
            try
            {
                var tickers = GetTickers("SPBFUT");
                var ticker = 
                    tickers
                        .Where(t => t.AssetCode == assetcode && t.LastTradeDate > DateTime.Now.Date)
                        .OrderBy(t => t.LastTradeDate).FirstOrDefault() ??
                    tickers
                        .Where(t => t.SecId.Left(2) == assetcode.Left(2) && t.LastTradeDate > DateTime.Now.Date)
                        .OrderBy(t => t.LastTradeDate).FirstOrDefault();
                return ticker;
            }           
            catch (Exception e)
            {
                err?.Invoke(e.ToString());
            }
            return null;
        }
    }
    [XmlRoot("rows")]
    public class TickersHolder : IItemHolder<MoexTicker>
    {
        public TickersHolder()
        {
        }
        [XmlElement("row")]
        public List<MoexTicker> Items { get; set; }
        public void Clear() { Items = null;}
    }
   
    public class ItemHolder<TItem>
    {
        public List<TItem> Items;

        public void InitItems()
        {
            foreach (var i in Items)
            {
            }
        }

        public void Clear()
        {
            Items = null;
        }

    }
    public class FuturesHolder
    {
        public FuturesHolder()
        {
        }
        [XmlElement("row")]
        public List<MoexFuture> Items { get; set; }
        public void InitItems()
        {
            foreach (var t in Items) t.Init();
        }
        public void Clear()
        {
            // Items?.Clear();
            Items = null;
        }
    }
    public class OptionsHolder
    {
        public OptionsHolder()
        {
        }
        [XmlElement("row")]
        public List<MoexOption> Items { get; set; }
        public void InitItems()
        {
            foreach (var t in Items) t.Init();
        }
        public void Clear()
        {
            // Items?.Clear();
            Items = null;
        }
    }
}
