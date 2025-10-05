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
    public class BarWebClient : WebClient02<GS.Trade.Dto.Bar>
    {
        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt)
        {
            if (string.IsNullOrWhiteSpace(ticker) || timeInt <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "Ticker=" + ticker +
                              "&TimeInt=" + timeInt;
            return GetItems(queryString);
        }
        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt)
        {
            if (string.IsNullOrWhiteSpace(ticker) || timeInt <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "Ticker=" + ticker +
                              "&TimeInt=" + timeInt +
                              "&Date=" + dt.DateToInt()
                              ;
            return GetItems(queryString);
        }
        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt1, DateTime dt2)
        {
            if (string.IsNullOrWhiteSpace(ticker) || timeInt <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "Ticker=" + ticker +
                              "&TimeInt=" + timeInt +
                              "&DT1=" + dt1.DateToInt() +
                              "&DT2=" + dt1.DateToInt()
                              ;
            return GetItems(queryString);
        }
        public IEnumerable<IBarSimple> GetSeries(long seriesId, DateTime dt)
        {
            if (seriesId <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "SeriesID=" + seriesId +
                              "&Date=" + dt.DateToInt()
                              ;
            return GetItems(queryString);
        }
        public IEnumerable<IBarSimple> GetSeries(long seriesId, DateTime dt1, DateTime dt2)
        {
            if (seriesId <= 0)
                return null;
            var queryString = "?" +
                //"Ticker=" + Server.UrlEncode(ticker) +
                //"&TimeInt=" + Server.UrlEncode(timeInt.ToString());
                              "SeriesID=" + seriesId +
                              "&DT1=" + dt1.DateToInt() +
                              "&DT2=" + dt2.DateToInt()
                              ;
            return GetItems(queryString);
        }

        
       

    }
}
