using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade;
using GS.Trade.Dto;

namespace WebClients
{
    public class TimeSeriesStatWebClient : WebClient02<GS.Trade.Dto.TimeSeriesStat>
    {
        public TimeSeriesStat GetTimeSeriesStat(string ticker, int timeInt)
        {
            if (string.IsNullOrWhiteSpace(ticker) || timeInt <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "Ticker=" + ticker +
                              "&TimeInt=" + timeInt;
            return GetItem(queryString);
        }
    }
}
