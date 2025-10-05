using GS.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade;

namespace CA_Test_Dde_02
{
    public class Tickers
    {
        public OptionDeskItem ParseOptionDeskItemStr(string s)
        {
            if (s.HasNoValue())
                return null;
            string[] split = {};
            OptionDeskItemParseFieldPlaceEnum lastSplitIndexToProcessing = 0;
            try
            {
                split = s.Split(';');
                if (split.Any(sp => sp.HasNoValue()))
                    return null;
                // OptionDeskItemParseFieldPlaceEnum.PutCode - MAX Split Index in Input string
                if (split.Length != (int)OptionDeskItemParseFieldPlaceEnum.PutCode  + 1)
                    return null;

                var optiondeskitem = new OptionDeskItem();

                var call = new CallOptionInfo { OptionType = OptionTypeEnum.Call };

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.ExpirationDate;
                call.ExpirationDate = DateTime
                    .ParseExact(split[(int)lastSplitIndexToProcessing], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.BaseAssetPrice;
                call.BaseAssetPrice = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Volatility;
                call.Volatility = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallCode;
                call.Code = split[(int)lastSplitIndexToProcessing];

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallDelta;
                call.Delta = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallGamma;
                call.Gamma = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTetta;
                call.Tetta = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallVega;
                call.Vega = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallPo;
                call.Po = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallBid;
                call.Bid = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallOffer;
                call.Offer = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTheoryPrice;
                call.TheoryPrice = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallOpenInerest;
                call.OpenInterest = long.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.CallTradesAmount;
                call.TradesAmount = int.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Strike;
                call.Strike = int.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                optiondeskitem.CallInfo = call;

                var put = new PutOptionInfo { OptionType = OptionTypeEnum.Put };

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.ExpirationDate;
                put.ExpirationDate = DateTime
                    .ParseExact(split[(int)lastSplitIndexToProcessing], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.BaseAssetPrice;
                put.BaseAssetPrice = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Volatility;
                put.Volatility = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutCode;
                put.Code = split[(int)lastSplitIndexToProcessing];

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutDelta;
                put.Delta = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutGamma;
                put.Gamma = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTetta;
                put.Tetta = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutVega;
                put.Vega = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutPo;
                put.Po = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutBid;
                put.Bid = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutOffer;
                put.Offer = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTheoryPrice;
                put.TheoryPrice = double.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutOpenInerest;
                put.OpenInterest = long.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.PutTradesAmount;
                put.TradesAmount = int.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                lastSplitIndexToProcessing = OptionDeskItemParseFieldPlaceEnum.Strike;
                put.Strike = int.Parse(split[(int)lastSplitIndexToProcessing]
                    .Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);

                optiondeskitem.PutInfo = put;

                return optiondeskitem;
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(e.Message);
                var invalidcontent = (int)lastSplitIndexToProcessing < split.Length
                    ? split[(int)lastSplitIndexToProcessing]
                    : "Unknown";
                ConsoleSync.WriteLineT($"Failure in Parse input string on SplitIndex:{lastSplitIndexToProcessing} " +
                                       $"InvalidValue:{invalidcontent}");
            }
            return null;
        }
    }
}
