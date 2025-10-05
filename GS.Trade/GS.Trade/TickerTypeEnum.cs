namespace GS.Trade
{
    public enum TickerTradeTypeEnum : short
    {
        Unknown = 0, Futures, Option, Shares, Stock, Security, Bond
    }
    //public enum TickerTypeEnum : short
    //{
    //    Unknown = 0, Futures, Option, Shares, Stock, Security, Bond
    //}
    public enum OptionTypeEnum : short
    {
        Unknown = 0, Call, Put
    }

    public enum OptionDeskItemParseFieldPlaceEnum : int
    {
        ExpirationDate = 0, BaseAssetPrice = 1,
        Volatility = 2, CallCode = 3,
        CallDelta = 4, CallGamma = 5, CallTetta = 6, CallVega = 7, CallPo = 8,
        CallBid = 9, CallOffer = 10,
        CallTheoryPrice = 11,
        CallOpenInerest = 12, CallTradesAmount = 13,
        Strike = 14,
        PutCode = 25,
        PutDelta = 24, PutGamma = 23, PutTetta = 22, PutVega = 21, PutPo = 20,
        PutBid = 18, PutOffer = 19,
        PutTheoryPrice = 17,
        PutOpenInerest = 16, PutTradesAmount = 15,
    }
}