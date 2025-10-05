using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Extension;
using GS.Trade.DataBase.Model;
using GS.Trade.Web.Clients;
using GS.Serialization;

namespace RtsMarginUpdate_01
{
    public class item
    {
        public item()
        {

        }

        [XmlAttribute(AttributeName = "symbol")]
        public string symbol { get; set; }  // RIZ

        [XmlAttribute(AttributeName = "code")]  // ALSI-12.13
        public string code { get; set; }

        [XmlAttribute(AttributeName = "delivery_date")]
        public string delivery_date { get; set; }

        [XmlAttribute(AttributeName = "initial_margin")]
        public decimal initial_margin { get; set; }

        [XmlAttribute(AttributeName = "price_limits")]
        public float price_limits { get; set; }

        public string Symb
        {
            get
            {
                var s = code.Split('-');
                return s[0] + "-" + delivery_date.Right(2) + "." + delivery_date.Substring(3, 2);

            }
        }

        public DateTime DeliveryDate
        {
            get { return DateTime.Parse(delivery_date); }
        }
      
    }
    class Program
    {
        static void Main(string[] args)
        {
            var margin = Builder.Build<RtsMargin.RtsMargin>(@"Init\RtsMargin.xml", "RtsMargin");
            if (margin == null)
                throw new NullReferenceException("RtsMargin is Null");
            var li = margin.Load();

            var wcs = new WebClients();
            wcs.Init(@"Init\WebClients.xml");

            var wc = wcs["Tickers"] as TickerWebClient;
            if(wc==null)
                throw new NullReferenceException("TickerWebClient is Null");

            //var li = new List<item>();
            //var url = @"http://moex.com/export/derivatives/go.aspx?type=F";

            //var xdoc = XDocument.Load(url);
            //var xx = xdoc.Descendants("item");
            //foreach (var x in xx)
            //{
            //    var i = GS.Serialization.Do.DeSerialize<item>(x, null);
            //    li.Add(i);
            //    Console.WriteLine(x.ToString());
            //}
            //Console.ReadLine();

            //var wc = new GS.Trade.Web.Clients.TickerWebClient2();
            
            //wc.Init();
            var updateCount = 0;
            foreach (var i in li)
            {
                //var t = wc.Get(@"?board=SPBFUT&code="+i.symbol);
                var t = wc.Get(@"SPBFUT@" + i.symbol);
                if (t == null)
                {
                    var dt = i.DeliveryDate.AddDays(-183);
                    t = new Ticker
                    {
                        Name = i.Symb,
                        Alias = i.Symb,

                        Decimals = 0,
                        MinMove = 0.0f,

                        TradeBoard = "SPBFUT",
                        Code = i.symbol,
                        BaseContract = i.symbol.Left(2),

                        Margin = (decimal)i.initial_margin,
                        PriceLimit = i.price_limits,

                        LaunchDate = dt,
                        ExpireDate = i.DeliveryDate,

                        Modified = DateTime.Now
                    };
                    var ti = wc.Add(t); 
                    //if(ti != null)
                    //    Console.WriteLine("Ticker Added: {0}", ti);
                    //else
                    //    Console.WriteLine("Ticker Added Error: {0}", t);
                    //Console.ReadLine();
                    continue;
                }

                if (t.Margin.IsEquals(i.initial_margin) && t.PriceLimit.IsEquals(i.price_limits))
                    continue;
                
                updateCount++;

                var id = t.Id;
                t.Margin = i.initial_margin;
                t.PriceLimit = i.price_limits;
                //t.Margin = 100;
                //t.PriceLimit = 250;
               // t.Id = 0;
                var ret = wc.Update(id, t);
                if (ret)
                    Console.WriteLine("Updated: " + t);
                else
                    Console.WriteLine("Update Error: " + t);

             //   Console.ReadLine();
            }
            Console.WriteLine("{0} Total Records Processed.", li.Count());
            Console.WriteLine("{0} Total Records Updated.", updateCount);
            Console.WriteLine("Press Any Key to Exit");
            Console.ReadLine();
        }
    }
}
