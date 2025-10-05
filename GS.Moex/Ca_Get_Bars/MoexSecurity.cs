using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ca_Get_Bars
{
    [XmlRoot(ElementName = "row")]
    public class MoexSecurityElement
    {
        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "BOARDID")]
        public string  BoardId { get; set; }
        [XmlAttribute(AttributeName = "SHORTNAME")]
        public string ShortName { get; set; }
        [XmlAttribute(AttributeName = "SECNAME")]
        public string SecName { get; set; }
        // public double PrevSettlePrice { get; set; }
        // public string Status { get; set; }
        [XmlAttribute(AttributeName = "DECIMALS")]
        public int Decimals { get; set; }
        [XmlAttribute(AttributeName = "MINSTEP")]
        public double MinStep { get; set; }
        // public string LastTradeDate { get; set; }
        //public string LastDelDate { get; set; }
        [XmlAttribute(AttributeName = "SECTYPE")]
        public string SecType { get; set; }
        [XmlAttribute(AttributeName = "LATNAME")]
        public string LatName { get; set; }
        // public string AssetCode { get; set; }
        // public double PrevOpenPosition { get; set; }
        // public double LotVolume { get; set; }
        [XmlAttribute(AttributeName = "INITIALMARGIN")]
        public double InitialMargin  { get; set; }
        //        public double HighLimit { get; set; }
        //        public double LowLimit { get; set; }
        [XmlAttribute(AttributeName = "STEPPRICE")]
        public double StepPrice { get; set; }
        // public double LastSettlePrice { get; set; }
        // public double PrevPrice { get; set; }
        // public double FirstTradeDate { get; set; }

        public string ShortInfo =>
            $"SecId:{SecId} SecType:{SecType} ShortName:{ShortName} SecName:{SecName} " +
            $"Decimals:{Decimals} MinStep:{MinStep} StepPrice:{StepPrice} InitialMargin:{InitialMargin}";
        public override string ToString()
        {
            return $"SecId:{SecId} BoardId:{BoardId} ShortName:{ShortName} SecName:{SecName} " +
                   $"Decimals:{Decimals} MinStep:{MinStep} SecType:{SecType} LatName:{LatName} " +
                   $"InitialMArgin:{InitialMargin} StepPrice:{StepPrice}";
        }
    }
}
