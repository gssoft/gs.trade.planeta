using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GS.Collections;
using GS.Trade.TimeSeries.Model;
using GS.Trade.TimeSeries.Model.View;
using GS.Trade.Dto;
using TimeSeriesStat = GS.Trade.Dto.TimeSeriesStat;

namespace GS.Trade.Web.Api.TimeSeries01.Controllers
{
    public class TimeSeriesStatController : ApiController
    {
        private readonly TimeSeriesContext _db = new TimeSeriesContext();
        public TimeSeriesStat GetSeriesStat(string ticker, int? timeInt)
        {
            var tms = _db.GetSeriesStat(ticker, timeInt);
            if (tms == null)
                return null;

            return new TimeSeriesStat
            {
                Id = tms.Id,
                ProviderId = tms.ProviderId,
                TickerId = tms.TickerId,
                TimeIntId = tms.TimeIntId,

                Provider = tms.Provider,
                Ticker = tms.Ticker,
                TimeInt = tms.TimeInt,

                FirstDate = tms.FirstDate,
                LastDate = tms.LastDate,
                Count = tms.Count
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}