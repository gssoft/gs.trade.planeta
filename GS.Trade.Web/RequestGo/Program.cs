using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Extension;
using GS.Trade.DataBase.Model;
using GS.Trade.Web.Clients;

namespace RequestGo
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
                //if (s.Length < 5)
                //{
                //    var sl = s[1].Right(2) + "." + "0" + s[1].Left(1);
                //    return s[0] + "-" + sl;
                //}
                //else
                //{
                //    return s[0] + "-" + s[1].Right(2) + "." + s[1].Left(2);
                //}

                return s[0] + "-" + delivery_date.Right(2) + "." + delivery_date.Substring(3, 2);


            }

        }

        public DateTime DeliveryDate {
            get { return DateTime.Parse(delivery_date); }
        }

        private string Cresymb()
        {
            var s = code.Split('-');
            if (s.Length < 5)
            {
                var sl = s[1].Right(2) + "." + "0" + s[1].Left(1);
                return s[0] + "-" + sl;
            }
            else
            {
                return s[0] + "-" + s[1].Right(2) + "." + s[1].Left(2);
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            List<item> li = new List<item>();
            var url = @"http://moex.com/export/derivatives/go.aspx?type=F";
            //string xmlResult = null;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader resStream = new StreamReader(response.GetResponseStream());
            //var doc = new XmlDocument();
            //xmlResult = resStream.ReadToEnd();
            //doc.LoadXml(xmlResult);
            //doc.LoadXml(xmlResult);
            //var xx = doc.GetElementsByTagName("item");
            //foreach (var x in xx)
            //{
            //    foreach(var a in x)
            //    Console.WriteLine(x.ToString());
            //}
            var xdoc = XDocument.Load(url);
            var xx = xdoc.Descendants("item");
            foreach (var x in xx)
            {
                var i=  GS.Serialization.Do.DeSerialize<item>(  x, null);
                li.Add(i);
                Console.WriteLine(x.ToString());
            }
            Console.ReadLine();

            var wc = new GS.Trade.Web.Clients.WebClient();
            wc.Init();

            foreach (var i in li)
            {
                var t = wc.GetTicker("SPBFUT", i.symbol);
                if (t == null)
                {
                  //  Console.WriteLine("Error - Ticker Not Found {0}", i.symbol);
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
                        wc.Register(t);
                        Console.WriteLine("Ticker Added: {0}", t);
                    Console.ReadLine();
                    continue;
                }
            //    Console.WriteLine(t);

                if (t.Margin.IsEquals(i.initial_margin) && t.PriceLimit.IsEquals(i.price_limits))
                    continue;

                var id = t.Id;
                t.Margin = i.initial_margin;
                t.PriceLimit = i.price_limits;
                //t.Margin = 100;
                //t.PriceLimit = 250;
                t.Id = 0;
                var ret = wc.UpdateTicker(id, t);
                if( ret > 0)
                    Console.WriteLine("Updated: " + t);
                else
                    Console.WriteLine("Update Error: " + t);

                Console.ReadLine();
            }
            

            Console.ReadLine();

            
            //foreach (var i in li)
            //{
            //    var dt = i.DeliveryDate.AddDays(-183);
            //    var t = new Ticker
            //    {
            //        Name = i.Symb,
            //        Alias = i.Symb,

            //        Decimals = 0,
            //        MinMove = 0.0f,

            //        TradeBoard = "SPBFUT",
            //        Code = i.symbol,
            //        BaseContract = i.symbol.Left(2),

            //        Margin = (decimal) i.initial_margin,
            //        PriceLimit = i.price_limits,

            //        LaunchDate = dt,
            //        ExpireDate = i.DeliveryDate,

            //        Modified = DateTime.Now
            //    };
            //    wc.Register(t);
            //    Console.WriteLine(t.ToString());
            //}
            //Console.ReadLine();


        }
    }
}
