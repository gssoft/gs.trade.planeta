using System;
using System.Collections.Generic;
using System.Linq;
using GS.Compress;
using GS.ConsoleAS;
using GS.Serialization;
using GS.Trade;
using GS.Trade.Extensions;
using GS.Trade.TimeSeries.Model;
using GS.Trade.TimeSeriesImage01.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarSeries = GS.Trade.TimeSeriesImage01.Models.BarSeries;
using QuoteProvider = GS.Trade.TimeSeriesImage01.Models.QuoteProvider;
using Ticker = GS.Trade.TimeSeriesImage01.Models.Ticker;
using TimeInt = GS.Trade.TimeSeriesImage01.Models.TimeInt;
using TimeSeriesImageContext = GS.Trade.TimeSeriesImage01.Models.TimeSeriesImageContext;
using TradeBoard = GS.Trade.TimeSeriesImage01.Models.TradeBoard;

namespace TimeSeriesUnitTest01
{
    [TestClass]
    public class TimeSeriesUnitTest
    {
        private const string TickerCode = "SiM0";
        private const string BytesSeriesCode = "BytesSeries005";

        public TimeSeriesImageContext TimeSeriesImageContext;
        public TimeSeriesContext TimeSeries01;

        public Ticker Ticker;
        public BytesSeries BytesSeries;

        public List<GS.Trade.Dto.Bar> Bars;
        public List<string> BarsStrList;
        public byte[] BytesBarStr;
        public byte[] BytesBarStrZip;

        [TestInitialize]
        public void Init()
        {
            TimeSeries01 = new TimeSeriesContext();
            Assert.IsNotNull(TimeSeries01, "TimeSeries01 is null");
            TimeSeriesImageContext = new TimeSeriesImageContext();
            Assert.IsNotNull(TimeSeriesImageContext, "TimeSeriesImage is null");
        }
        [TestMethod]
        public void CreateByteSereis()
        {
            var tb = new TradeBoard { Code = "SPBFUT" };
            TimeSeriesImageContext.TradeBoards.Add(tb);
            Ticker = new Ticker { Code = TickerCode, TradeBoardId = tb.Id };
            TimeSeriesImageContext.Tickers.Add(Ticker);
            var ti = new TimeInt { Code = "5Sec", TimeInterval = 5, TimeShift = 0 };
            TimeSeriesImageContext.TimeInts.Add(ti);
            var qpr = new QuoteProvider { Code = "Quik.Real" };
            TimeSeriesImageContext.QuoteProviders.Add(qpr);
            var tms = new BytesSeries
            {
                Code = BytesSeriesCode,
                TickerId = Ticker.Id,
                TimeIntId = ti.Id,
                QuoteProviderId = qpr.Id
            };
            TimeSeriesImageContext.TimeSeries.Add(tms);
            TimeSeriesImageContext.SaveChanges();

            var ticker = TimeSeriesImageContext.Tickers.FirstOrDefault(t => t.Code == TickerCode);
            Assert.IsNotNull(ticker,"Ticker == null");
            BytesSeries = TimeSeriesImageContext.TimeSeries.OfType<BytesSeries>()
                .FirstOrDefault(bs => bs.Code == BytesSeriesCode);
            Assert.IsNotNull(BytesSeries, "ByteSeries == null");
        }
        [TestMethod]
        public void GetBars_Test()
        {
            CreateByteSereis();
            // Bars = TimeSeries01.GetBarsDto(74, 20200529).ToList();
            Bars = TimeSeries01.GetBarsDto(TickerCode, 5).ToList();
            Assert.IsNotNull(Bars, "Bars is null");
            Assert.IsTrue(Bars.Any(), "Bars is Empty");
            ConsoleSync.WriteLineT($"Bars Count: {Bars.Count}");
            BarsStrList = Bars.Select(b => b.ToStr()).ToList();
            Assert.IsNotNull(BarsStrList, "BarsStr is null");
            Assert.IsTrue(BarsStrList.Any(), "BarsStr is Empty");
            foreach (var i in Enumerable.Range(0,10))
            {
                ConsoleSync.WriteLineT(BarsStrList[i]);
            }
            BytesBarStr = BinarySerialization.SerializeToByteArray(BarsStrList);
            Assert.IsNotNull(BytesBarStr, "bytesBarStr != null");
            Assert.IsTrue(BytesBarStr.Any(), "!bytesBarStr.Any()");
            ConsoleSync.WriteLineT($"BytesBarStr: {BytesBarStr.Length}");
            BytesBarStrZip = Compressor.Compress(BytesBarStr);
            Assert.IsNotNull(BytesBarStrZip, "bsBarsStrZip != null");
            Assert.IsTrue(BytesBarStrZip.Any(), "!bsBarsStrZip.Any()");
            ConsoleSync.WriteLineT($"bsBarsStrZip: {BytesBarStrZip.Length}");
        }
        [TestMethod]
        public void PutByteSeriesDataToDataBase_Test()
        {
            GetBars_Test();

            Assert.IsNotNull(BytesBarStrZip, "bsBarsStrZip != null");
            Assert.IsTrue(BytesBarStrZip.Any(), "!bsBarsStrZip.Any()");

            var bs = TimeSeriesImageContext.TimeSeries.OfType<BytesSeries>()
                .FirstOrDefault(ts=>ts.Code == BytesSeriesCode && ts.Ticker.Code == TickerCode);
            Assert.IsNotNull(bs,"BytesSeries == null");
            var bsitem = new BytesSeriesItem
            {
                DT = DateTime.Now,
                Format = "CSV",
                Count = BytesBarStrZip.Length,
                CheckSum = Guid.NewGuid().ToString(),
                Bytes = BytesBarStrZip
            };
            bs.Items.Add(bsitem);
            TimeSeriesImageContext.SaveChanges();
        }

        [TestMethod]
        public void XGetByteSeriesDataFromDataBase_Test()
        {            
            PutByteSeriesDataToDataBase_Test(); 
                       
            Assert.IsNotNull(BarsStrList, "BarStrList is null");

            var bs = TimeSeriesImageContext.TimeSeries.OfType<BytesSeries>()
                .FirstOrDefault(ts => ts.Code == BytesSeriesCode && ts.Ticker.Code == TickerCode);
            Assert.IsNotNull(bs, "ByteSeries not found");
            var bsi = TimeSeriesImageContext.BytesSeries.FirstOrDefault(b => b.BytesSeriesId == bs.Id);
            Assert.IsNotNull(bsi, "ByteSeriesItem not found");
            var byteszip = bsi.Bytes;
            Assert.IsNotNull(byteszip, "BytesZip == null");
            var bytesBarStrBack = Compressor.DeCompress(byteszip);
            Assert.IsNotNull(bytesBarStrBack, "BytesStrBack == null");
            var barsListStrBack = BinarySerialization.DeSerialize<List<string>>(bytesBarStrBack);
            Assert.IsNotNull(barsListStrBack, "BytesListStrBack == null");
            Assert.IsTrue(barsListStrBack.Count == BarsStrList.Count, $"BarsListStrBack.Count: {barsListStrBack.Count} is non Equal BarsStrList: {BarsStrList.Count} ");
            var j = 0;
            foreach (var i in BarsStrList)
            {
                Assert.IsTrue(i == barsListStrBack[j], $"Items non Equal: {i} {barsListStrBack[j]}");
                j++;
            }
            var listofbars = barsListStrBack.Select(bstr => bstr.ToBarDto()).ToList();
            Assert.IsNotNull(listofbars, "listofbars == null");
            Assert.IsTrue(listofbars.Count == Bars.Count, "Barbacks.Count non Equal");
            j = 0;
            foreach (var b in Bars)
            {
                Assert.IsTrue(b.CompareTo(listofbars[j]), $"BarsItem non Equal: {b} {listofbars[j]} ");
                j++;
            }
        }

        //[TestMethod]
            //public void ConvertToStr()
            //{
            //    //Assert.IsNotNull(Bars, "Bars is null");
            //    //Assert.IsTrue(Bars.Any(), "Bars is Empty");

            //}
        }
}
