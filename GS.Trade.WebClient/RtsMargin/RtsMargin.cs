using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Extension;

namespace RtsMargin
{
    public class item
    {
        private CultureInfo _provider;

        public item()
        {
            _provider = CultureInfo.InvariantCulture;
        }

        [XmlAttribute(AttributeName = "symbol")]
        public string symbol { get; set; } // RIZ

        [XmlAttribute(AttributeName = "code")] // ALSI-12.13
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
            get { return DateTime.ParseExact(delivery_date.Trim(),   "yyyyMMdd", _provider); }
        }

    }

    public class RtsMargin
    {
        public string Uri { get; set; }

        [XmlIgnore]
        public List<item> Items { get; private set; }

        public RtsMargin()
        {
            Items = new List<item>();
        }

        public IEnumerable<item> Load()
        {
            try
            {
                var xdoc = XDocument.Load(Uri);
                var xx = xdoc.Descendants("item");
                foreach (var x in xx)
                {
                    var i = GS.Serialization.Do.DeSerialize<item>(x, null);
                    Items.Add(i);
                }
                return Items;
            }
            catch (Exception e)
            {
                throw new Exception(ToString() + " RtsMargin Load Failure: " + e.Message);
            }
        }
        public override string ToString()
        {
            return String.Format("[Type:{0} Uri:{1}", GetType(), Uri);
        }
    }


}
