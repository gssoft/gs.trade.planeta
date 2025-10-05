using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;
using GS.Storages;
using GS.Trade;
using GS.Trade.Storage2;
using GS.Trade.Trades.Storage;

namespace Test_Types_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = typeof (EmptyStorage);
            var ns = t.Namespace;
            var a = t.Assembly;
            var aqf = t.AssemblyQualifiedName;
            var fn = t.FullName;

            var myT = Type.GetType(aqf);
            var myT2 = Type.GetType("GS.Trade.Trades.Storage.EmptyStorage,GS.Trade.Trades");

            var st = t.ToString();

            var xe = XElement.Load(@"Init/Type.xml");
            if (xe == null) return;

            var tn = xe.Name.ToString();

            var tns = xe.Attribute("ns");
            var ta = xe.Attribute("as");

            var stype = (tns == null ? "" : tns.Value + ".") + tn + (ta==null ? "" : "," + ta.Value);

            var myT3 = Type.GetType(stype);

            var evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            evl.Init();

            // var trs = Builder.Build<TradeStorage>(@"Init/Type.xml", "TradeStorage");
            var trs = Builder.Build2<ITradeStorage>(@"Init/Type.xml", "TradeStorage");
            trs.Init(evl);
        }
        
    }
}
