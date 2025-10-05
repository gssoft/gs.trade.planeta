using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.TimeSeries.FortsTicks.Dal;
using GS.Trade.TimeSeries.FortsTicks2.Dal;
using NUnit.Framework;

namespace GS.Trade.TimeSeries.Test_01.FortsTicks
{
    public class FortsTick2Tests
    {
        [OneTimeSetUp]
        public void CreateDataBaseTest()
        {
            //SourcePath = @"F:\Forts\2016\Txt\";
            //SourceFileFilter = @"FT*.CSV";

            //var appDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../App_Data");
            //AppDomain.CurrentDomain.SetData("DataDirectory", appDataDir);

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var appDataDirectory = Path.Combine(baseDirectory.Replace("\\bin\\Debug", ""), "");
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataDirectory);

            Database.SetInitializer(new TimeSeries.FortsTicks2.Init.Initializer());
        }
        [Test]
        public void GetTickers()
        {
            using (var db = new FortsTicksContext2())
            {
                var ts = db.Tickers.ToList();
                foreach (var t in ts)
                    Console.WriteLine("Ticker: {0}", t);
            }
        }
        [Test]
        public void AddTicks()
        {
            using (var db = new FortsTicksContext2())
            {
                db.AddTicks2(@"F:\Forts\2016\Txt\FTTest\FT160825.F.CSV");
            }
        }
        [Test]
        public void UpdateTickSeriesStat_Test()
        {
            using (var db = new FortsTicksContext2())
            {
                db.UpdateTickSeriesStat();
            }
        }
        [Test]
        public void UpdateTickStatDaily_Test()
        {
            using (var db = new FortsTicksContext2())
            {
                db.UpdateTicksStatDaily();
            }
        }
        [Test]
        public void UpdateTickStatAll_Test()
        {
            using (var db = new FortsTicksContext2())
            {
                db.UpdateTicksStatAll();
            }
        }

        [Test]
        public void AddTicks_BatchWork_Test()
        {
            var _evl = Builder.Build2<IEventLog>(
                AppDomain.CurrentDomain.BaseDirectory + @"\Init\EventLog.xml", "EventLogs");
            _evl.Init();

            var ws = new BatchWork();
            ws.Init(_evl);

            ws.Init(AppDomain.CurrentDomain.BaseDirectory + @"\Init\Forts_2016.xml");
           

            ws.DoWorks();
        }

        [Test]
        public void RemoveEmptyTickSeries_Test()
        {
            using (var db = new FortsTicksContext2())
            {
                db.RemoveEmptyTickSeries();
            }
        }
    }
}
