using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade;
using GS.Trade.Dto;

namespace WebClients
{
    public class TimeSeriesWebClient01 : Element1<string>
    {
        public string ErrorMessage => _barWebClient?.ErrorMessage;
        public string HttpReasonPhrase => _barWebClient?.HttpReasonPhrase;

        private BarWebClient _barWebClient;
        private TimeSeriesStatWebClient _timeSeriesStatwebClient;

        public override void Init(IEventLog eventLog)
        {
            try
            {
                base.Init(eventLog);

                _barWebClient = Builder.Build<BarWebClient>(@"Init\WebClients.xml", "BarWebClient");

                if (_barWebClient == null)
                    throw new Exception("webClient After Build is null");

                _barWebClient.Init();

                _timeSeriesStatwebClient = Builder.Build<TimeSeriesStatWebClient>(@"Init\WebClients.xml", "TimeSeriesStatWebClient");

                if (_timeSeriesStatwebClient == null)
                    throw new Exception("TimeSeriesStatWebClient After Build is null");

                _timeSeriesStatwebClient.Init();

                // throw  new Exception("Test Excpetion from WebClient"); 
            }
            catch (Exception ex)
            {
                
                SendExceptionMessage3(Code, GetType().ToString(),
                    $"{System.Reflection.MethodBase.GetCurrentMethod()?.Name} {System.Reflection.MethodBase.GetCurrentMethod()?.ReflectedType?.Name}"
                    , ToString(), ex);
            }

            WhoAreYou();
        }          

        public override string Key => Code;

        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt)
        {
            return _barWebClient?.GetSeries(ticker, timeInt);
        }

        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt)
        {
            return _barWebClient?.GetSeries(ticker, timeInt, dt);
        }
        public IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt1, DateTime dt2)
        {
            return _barWebClient?.GetSeries(ticker, timeInt, dt1, dt2);
        }

        public IEnumerable<IBarSimple> GetSeries(long seriesId, DateTime dt)
        {
            return _barWebClient?.GetSeries(seriesId, dt);
        }

        public IEnumerable<IBarSimple> GetSeries(long seriesId, DateTime dt1, DateTime dt2)
        {
            return _barWebClient?.GetSeries(seriesId, dt1, dt2);
        }

        public TimeSeriesStat GetTimeSeriesStat(string ticker, int timeInt)
        {
            return _timeSeriesStatwebClient?.GetTimeSeriesStat(ticker, timeInt);
        }
    }
}