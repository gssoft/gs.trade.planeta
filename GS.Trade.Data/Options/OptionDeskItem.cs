using System;

namespace GS.Trade.Data.Options
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
        public double Theta { get; set; }
        public double Vega { get; set; }
        public double Rho { get; set; }
        public double Bid { get; set; }
        public double Offer { get; set; }
        public double TheoryPrice { get; set; }
        public long OpenInterest { get; set; }
        public int TradesCount { get; set; }
        public override string ToString()
        {
            return $"{OptionType}:{Code} Strike:{Strike} Expiration:{ExpirationDate:d} BaseAssetPrice:{BaseAssetPrice} " +
               // $"{OptionType}:{Code} Expiration:{ExpirationDate.ToString("d")} BaseAsset:{BaseAssetCode} BaseAssetPrice: {BaseAssetPrice} " +
               $"Volatility:{Volatility} Delta:{Delta} Gamma:{Gamma} Theta:{Theta} Vega:{Vega} Rho:{Rho} " +
               $"Bid:{Bid} Ask:{Offer} TheoryPrice:{TheoryPrice} OI:{OpenInterest} TradesCount:{TradesCount}";
        }
    }
    public class CallOptionInfo : OptionInfo
    {
        public CallOptionInfo()
        {
            OptionType = OptionTypeEnum.Call;
        }
    }
    public class PutOptionInfo : OptionInfo
    {
        public PutOptionInfo()
        {
            OptionType = OptionTypeEnum.Put;
        }
    }
}
