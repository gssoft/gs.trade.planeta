using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Dto;
//using GS.Trade.DB.Q;
//using GS.Trade.DownLoader;
using Bar = GS.Trade.Data.Bars.Bar;

namespace GS.Trade.Data.Options
{
    public class OptionTicker : Ticker, IOptionTicker
    {
        public OptionTicker()
        {
            OptionInfo = new OptionInfo();
        }
        public override double LastPrice => OptionInfo?.TheoryPrice ?? 0d;
        public override double Bid => OptionInfo?.Bid ?? 0d;
        public override double Ask => OptionInfo?.Offer ?? 0d;

        public OptionInfo OptionInfo { get; }
        public OptionTypeEnum OptionType => OptionInfo?.OptionType ?? OptionTypeEnum.Unknown;
        public double Strike => OptionInfo?.Strike ?? 0d;
        public string BaseAssetCode => OptionInfo?.BaseAssetCode ?? string.Empty;
        public double BaseAssetPrice => OptionInfo?.BaseAssetPrice ?? 0d;
        public DateTime ExpirationDate => OptionInfo?.ExpirationDate ?? DateTime.MinValue;
        public double Volatility => OptionInfo?.Volatility ?? 0d;
        public override double Delta => OptionInfo?.Delta ?? 0d;
        public double Gamma => OptionInfo?.Gamma ?? 0d;
        public double Theta => OptionInfo?.Theta ?? 0d;
        public double Vega => OptionInfo?.Vega ?? 0d;
        public double Rho => OptionInfo?.Rho ?? 0d;
        public double TheoryPrice => OptionInfo?.TheoryPrice ?? 0d;
        public long OpenInterest => OptionInfo?.OpenInterest ?? 0;
        public int TradesCount => OptionInfo?.TradesCount ?? 0;
    } 
}