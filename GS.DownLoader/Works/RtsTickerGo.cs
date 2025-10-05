using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.DB.TickModel.FT1203;

namespace GS.Trade.QuoteDownLoader.Works
{
    public class RtsTickerGo : IWorkItem
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string RtsTickerGoUri { get; set; }
        [XmlIgnore]
        public WorkContainer Works { get; set; }

        [XmlIgnore]
        private Dictionary<string, item> TickerGos;

        public RtsTickerGo()
        {
            TickerGos = new Dictionary<string, item>();
        }

        public void DoWork()
        {
            //var uri = @"http://rts.micex.ru/export/derivatives/go.aspx?type=F";
            DownLoadTickerGo();
            UpdateTickerGoDb();
        }
        private void DownLoadTickerGo()
        {
            TickerGos.Clear();
            try
            {
                if (string.IsNullOrWhiteSpace(RtsTickerGoUri))
                {
                    Works.EvlMessage(EvlResult.FATAL,"DownLoadTicker", "RtsTickerUri is Empty");
                    return;
                }
                var xDoc = XDocument.Load(RtsTickerGoUri);
                var dd = xDoc.Elements("rtsdata");
                foreach (var d in dd)
                {
                    var moment = DateTime.Now;
                    var momentAtr = d.Attribute("moment");
                    if (momentAtr != null)
                    {
                        DateTime mom;
                        if (DateTime.TryParse(momentAtr.Value, out mom)) moment = mom;
                    }
                    var cc = d.Elements("contract");
                    foreach (var c in cc)
                    {
                        var ii = c.Elements("item");
                        foreach (var i in ii)
                        {
                            var xe = i;
                            var goi = WorkContainer
                                .DeSerialize<item>(i,s =>Works
                                    .EvlMessage(EvlResult.FATAL,"DeSerialize: " + xe.ToString(),s));
                            if (goi == null) break;
                            goi.MomentDT = moment;
                            goi.ModifiedDT = DateTime.Now;
                            if (!TickerGos.ContainsKey(goi.LongCode)) TickerGos.Add(goi.LongCode, goi);
                        }
                    }
                }
                var noValid = TickerGos.Values.Where(g => !g.Valid);
                
                foreach (var i in noValid)
                {
                    Console.WriteLine(i);
                    Works.EvlMessage(EvlResult.WARNING,"Invalid Ticker", i.ToString());
                }
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, "DownLoad RtsTickerGo", e.Message);
            }
        }
        private void UpdateTickerGoDb()
        {
            if (TickerGos.Count() == 0) return;
            var source = from t in TickerGos.Keys select t;
            using( var db = new M1203())
            {
                var targets = from t in db.TickerStores select t.LongCode.Trim();
                var toInsert = source.Except(targets);
                foreach (var tg in toInsert.Select(key => TickerGos[key]))
                {
                    var t = new TickerStore
                                {
                                    Code = tg.Code,
                                    LongCode = tg.LongCode,
                                    D3 = tg.DeliveryDate
                                };
                   Console.WriteLine("Insert -> " + t);
                }
                // Update D3 - Delivery Date
                var upd = from t in db.TickerStores select t;
                foreach (var t in upd)
                {
                    var key = t.LongCode.Trim();
                    if (!TickerGos.ContainsKey(key)) continue;
                    
                    var tg = TickerGos[key];
                    if( !tg.Valid ) continue;
                    var dt = t.D2;
                    var now = tg.DeliveryDate;
                    if (t.D2 == tg.DeliveryDate) continue;
                    
                    t.D2 = tg.DeliveryDate;
                    Console.WriteLine("Update -> " + t.LongCode + ": Was: " + dt + " Now: " + now );
                }
            }
            //foreach(var t in TickerGos.Values)
            //    Console.WriteLine(t);
        }
        
    }
    public class item
    {
        public item()
        {
        }
        [XmlAttribute(AttributeName = "symbol")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string LongCode { get; set; }
        [XmlAttribute(AttributeName = "delivery_date")]
        public string DeliveryDateStr { get; set; }
        [XmlAttribute(AttributeName = "initial_margin")]
        public float InitialMargin { get; set; }
        [XmlAttribute(AttributeName = "price_limits")]
        public float PriceLimits { get; set; }
        [XmlIgnore]
        public DateTime MomentDT { get; set; }
        [XmlIgnore]
        public DateTime ModifiedDT { get; set;}

        [XmlIgnore]
        public DateTime DeliveryDate
        {
            get
            { 
                DateTime dt;
                return DateTime.TryParse(DeliveryDateStr, out dt) ? dt : DateTime.MinValue.Date;
            }
        }
        public bool Valid
        {
            get
            {
                var s1 = LongCode.Trim().Substring(LongCode.Trim().Length - 2);
                var s2 = DeliveryDateStr.Trim().Substring(DeliveryDateStr.Trim().Length - 2);
                int i1,i2;
                if( int.TryParse(s1, out i1) && int.TryParse(s2, out i2) )
                {
                    return i1 == i2;
                }
                return true;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5}",
                Code, LongCode, DeliveryDateStr, InitialMargin, PriceLimits, MomentDT);
        }
    }
}
