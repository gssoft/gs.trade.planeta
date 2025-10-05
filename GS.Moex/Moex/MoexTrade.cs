using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Extension;
using GS.Moex.Interfaces;
// using GS.Trade.Moex;

namespace Moex

{
    [XmlRoot(ElementName = "row")]
    //[XmlElement("row")]
    public class MoexTrade : IMoexTrade
    {
        [XmlIgnore]
        public MoexTickerTypeEnum TickerType { get; set; }
        [XmlAttribute(AttributeName = "TRADENO")]
        public long TradeNumber { get; set; }
        [XmlAttribute(AttributeName = "BOARDNAME")]
        public string BoardName { get; set; }
        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "TRADEDATE")]
        public string TradeDateStr { get; set; }
        [XmlAttribute(AttributeName = "TRADETIME")]
        public string TradeTimeStr { get; set; }
        [XmlIgnore]
        public DateTime TradeDateTime { get; set; }
        [XmlAttribute(AttributeName = "PRICE")]
        public double Price { get; set; }
        [XmlAttribute(AttributeName = "QUANTITY")]
        public double Quantity { get; set; }
        [XmlAttribute(AttributeName = "SYSTIME")]
        public string SysDateTimeStr { get; set; }
        [XmlIgnore]
        public DateTime SysDateTime { get; set; }
        public bool IsFutures => TickerType == MoexTickerTypeEnum.Futures;
        public bool IsOption => TickerType == MoexTickerTypeEnum.Option;
        //public bool IsFutures => BoardId.Equals("RFUD", StringComparison.InvariantCultureIgnoreCase);
        //public bool IsOption => BoardId.Equals("ROPD", StringComparison.InvariantCultureIgnoreCase);
        private void SetInstrumentType()
        {
            switch (BoardName)
            {
                case "RFUD": TickerType = MoexTickerTypeEnum.Futures; break;
                case "ROPD": TickerType = MoexTickerTypeEnum.Option; break;
                default: TickerType = MoexTickerTypeEnum.Unknown; break;
            }
        }
        private void ParseDateTimeFields()
        {
            try
            {
                TradeDateTime = TradeDateStr.HasValue() && TradeTimeStr.HasValue()
                    ? DateTime.Parse(TradeDateStr + " " + TradeTimeStr) 
                    : DateTime.MinValue;

                SysDateTime = SysDateTimeStr.HasValue() 
                    ? DateTime.Parse(SysDateTimeStr) 
                    : DateTime.MinValue;
            }
            catch (Exception e)
            {
                TradeDateTime = DateTime.MaxValue;
                SysDateTime = DateTime.MaxValue;
                ConsoleSync.WriteLineT(e.Message); 
            }
        }
        public void Init()
        {
            SetInstrumentType();
            ParseDateTimeFields();
        }
        public string ShortInfo =>
            $"Trade:[{TradeNumber}] DT:[{TradeDateTime}] SecId:{SecId} Price:{Price}  Qty:{Quantity}";
        public override string ToString()
        {
            return $"Trade:[{TradeNumber}] DT:[{TradeDateTime.ToString("G")}] Board:{BoardName} SecId:{SecId} " +
                   $"Price:{Price} Qty:{Quantity} SysDT:{SysDateTime.ToString("G")}";
        }
    }
    [XmlRoot("rows")]
    public class TradeList
    {
        public TradeList(){ Items = new List<MoexTrade>();}
        [XmlElement("row")]
        public List<MoexTrade> Items { get; set; }

        public void InitItems()
        {
            foreach (var t in Items) t.Init();
        }      
    }
    [XmlRoot("rows")]
    public class TradeHolder : IItemHolder<MoexTrade>
    {
        public TradeHolder(){}
        [XmlElement("row")]
        public List<MoexTrade> Items { get; set; }
        public void Clear()
        {
            Items = null;
        }
    }
}
