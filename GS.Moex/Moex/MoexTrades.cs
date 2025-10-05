using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using GS.ConsoleAS;
using GS.Extension;

namespace Moex
{
    public partial class MoexTest
    {
        public string CreateTradesLink(string board, string tickercode, int start, int limit)
        {
            //?start=0&limit=10
            var instrPrefix = GetInstrApiPrefix(board);
            if (instrPrefix.HasNoValue()) return string.Empty;
            // var tcode = GetTickerCode(tickercode);
            // if (tcode.HasNoValue()) return string.Empty;
            var paramstr = TradeApiParams
                .Replace("StartValue", start.ToString())
                .Replace("LimitValue", limit.ToString());
            return instrPrefix + tickercode + TradeApiPostfix + paramstr; // instr = option/futures, tcode = SiZ9, post = .xm
        }
        public XNode GetTradesXmlDoc(string board, string tickercode, int start, int limit )
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            if (board.HasNoValue() || tickercode.HasNoValue()) return null;
            try
            {
                var link = CreateTradesLink(board, tickercode, start, limit);
                if (link.HasNoValue()) return null;
                var xmlstrm = WebClient02.GetItemInStream(link);
                if (xmlstrm == null)
                {
                    SendMessage(methodname, "Failure in GetXmlDocInStream from Moex.ISS");
                    return null;
                }
                var xdoc = XDocument.Load(xmlstrm);
                return xdoc;
            }
            catch (Exception e){ SendMessage(methodname, e.ToString());}
            return null;
        }
        //public IList<MoexTrade> GetTradesAdOnce(string board, string tickercode, int start, int limit)
        //{
        //    return GetItems<TradeHolder, MoexTrade>(board);
        //}
        public IList<MoexTrade> GetTradesAdOnce(string board, string tickercode, int start, int limit)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var xnode = GetTradesXmlDoc(board, tickercode, start, limit);
                if (xnode == null) return null;
                var tradesxmlrows = GetXmlElementFromXDoc(xnode, TradeRowsXPath);
                if (tradesxmlrows == null) return null;
                if (!tradesxmlrows.HasElements) return new List<MoexTrade>();
                var holder = Deserialize<TradeHolder>(tradesxmlrows);
                return holder == null ? null : UnPackHolder<TradeHolder, MoexTrade>(holder);
                //if (!holder.Items.Any()) return new List<MoexTrade>();
                //var lst = holder.Items;
                //holder.Clear();
                //InitItems(lst);
            }
            catch (Exception e)
            {
                SendMessage(method, e.ToString());
            }
            return null;
        }
        public IList<MoexTrade> GetTradesAdOnce1(string board, string tickercode, int start, int limit)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var xnode = GetTradesXmlDoc(board, tickercode, start, limit);
                if (xnode == null) return null;
                var tradesxmlrows = GetXmlElementFromXDoc(xnode, TradeRowsXPath);
                if (tradesxmlrows == null) return null;
                if (!tradesxmlrows.HasElements) return new List<MoexTrade>();
                //var tradeHolder = Deserialize<TradeHolder>(tradesxmlrows);
                //if (tradeHolder == null) return null;
                //if (!tradeHolder.Items.Any()) return new List<MoexTrade>();
                //var lst = tradeHolder.Items;
                //tradeHolder.Clear();
                //foreach (var t in lst) t?.Init();
                var lst = TradesDeserialize(tradesxmlrows);
                return lst;
            }
            catch (Exception e)
            {
                SendMessage(method, e.ToString());
            }
            return null;
        }
        public IList<MoexTrade> GetTrades(string board, string tickercode, int start, int limit)
        {
            var method = MethodBase.GetCurrentMethod().Name + "()";
            var pass = 0;
            var tradespack = new List<MoexTrade>();
            try
            {
                while (true)
                {
                    var trades = GetTradesAdOnce(board, tickercode, start, limit);
                    if (trades == null)
                    {
                        SendMessage(method, $"Something Wrong with Trades Receiving.Trades is null;");
                        SendMessage(method, $"TradesPackCount:{tradespack.Count}");
                        return null;
                    }
                    if (!trades.Any()) break;
                    tradespack.AddRange(trades);
                    var cnt = trades.Count;
                    start += cnt;
                    SendMessage(method,
                        $"Pass:{++pass} TradesPassCount:{cnt} [{trades[0].TradeNumber}]-[{trades[cnt-1].TradeNumber}]");
                }
                var tradespackcount = tradespack.Count;
                var msg = tradespackcount > 0
                    ? $"Pack: TradesPackCount:{tradespackcount} [{tradespack[0].TradeNumber}]-[{tradespack[tradespackcount - 1].TradeNumber}]"
                    : $"Pack: TradesPackCount:{tradespackcount}";
                SendMessage(method, msg);

                return tradespack;
            }
            catch (Exception e) { SendMessage(method, e.ToString());}
            return null;
        }
    }
}
