using System;
using GS.Trade;

namespace CA_Test_Dde_02
{
    public class OptionDeskItem
    {
        public CallOptionInfo CallInfo { get; set; }
        public PutOptionInfo PutInfo { get; set; }
    }
    public class OptionInfo
    {
        public OptionTypeEnum OptionType { get; set; }
        public string BaseAssetCode { get; set; }
        public string Code { get; set; }
        public double Strike { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double BaseAssetPrice { get; set; }
        public double Volatility { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Tetta { get; set; }
        public double Vega { get; set; }
        public double Po { get; set; }
        public double Bid { get; set; }
        public double Offer { get; set; }
        public double TheoryPrice { get; set; }
        public long OpenInterest { get; set; }
        public int TradesAmount { get; set; }
        public override string ToString()
        {
         return  $"{OptionType}:{Code} Strike:{Strike} Expiration:{ExpirationDate.ToString("d")} BaseAssetPrice:{BaseAssetPrice} " +
            // $"{OptionType}:{Code} Expiraration:{ExpirationDate.ToString("d")} BaseAsset:{BaseAssetCode} BaseAssetPrice: {BaseAssetPrice} " +
            $"Volatility:{Volatility} Delta:{Delta} Gamma:{Gamma} Tetta:{Tetta} Vega:{Vega} Po:{Po} " +
            $"Bid:{Bid} Ask:{Offer} TheoryPrice:{TheoryPrice} OI:{OpenInterest} TradesCount:{TradesAmount}";
        }
    }
    public class CallOptionInfo : OptionInfo
    {
        
    }
    public class PutOptionInfo : OptionInfo
    {
    }
}
