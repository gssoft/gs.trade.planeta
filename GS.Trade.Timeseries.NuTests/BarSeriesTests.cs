using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.Dto;
using NUnit.Framework;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClients;
using GS.Trade.Data.Bars;
//using Bar = GS.Trade.Data.Bars.Bar;
using GS.Trade.Extensions;

namespace GS.Trade.Timeseries.NuTests
{
    [TestFixture]
    public class GetBarSeriesFromTheWebTests
    {
        // private TimeSeriesWebClient01 _webClient;

        private BarWebClient _barWebClient;
        private TimeSeriesStatWebClient _timeSeriesStatwebClient;
        private TimeSeriesStat _timeSeriesStat;
        public IEventLog EventLog { get; set; }

        private IEnumerable<IBarSimple> _bs1;
        private IEnumerable<IBarSimple> _bs2;

        [OneTimeSetUp]
        public void Init()
        {
            //var wd = TestContext.CurrentContext.WorkDirectory;
            //var td = TestContext.CurrentContext.WorkDirectory;

            Assert.DoesNotThrow(() =>
            {
                // EventLog = Builder.Build2<IEventLog>(@"D:\VC\1305\gs.trade\GS.Trade.Timeseries.NuTests\Init\EventLog.xml",
                    EventLog = Builder.Build2<IEventLog>(AppDomain.CurrentDomain.BaseDirectory +  @"\Init\EventLog.xml",
                    "EventLogs");
                Assert.IsNotNull(EventLog, "EventLog Should Be Not Null");
                EventLog.Init();

                // _barWebClient = Builder.Build<BarWebClient>(@"D:\VC\1305\gs.trade\GS.Trade.Timeseries.NuTests\Init\WebClients.xml", "BarWebClient");
                _barWebClient = Builder.Build<BarWebClient>(AppDomain.CurrentDomain.BaseDirectory + @"\Init\WebClients.xml", "BarWebClient");
                Assert.IsNotNull(_barWebClient, "WebClient Should Be Not Null");
                _barWebClient.Init();

                //_timeSeriesStatwebClient = Builder.Build<TimeSeriesStatWebClient>(@"D:\VC\1305\gs.trade\GS.Trade.Timeseries.NuTests\Init\WebClients.xml", "TimeSeriesStatWebClient");
                _timeSeriesStatwebClient = Builder.Build<TimeSeriesStatWebClient>(AppDomain.CurrentDomain.BaseDirectory + @"\Init\WebClients.xml", "TimeSeriesStatWebClient");
                Assert.IsNotNull(_timeSeriesStatwebClient, "TimeSeriesWebClient Should Be Not Null");
                _timeSeriesStatwebClient.Init();

                GetSeriesStat();

                _bs1 = GetBarSeries();
                _bs2 = GetBarSeries();
                
            });
        }    
        

        [Test]
        public void VerifyBarSeriesCounts()
        {
            Assert.That(_bs1.Count(), Is.EqualTo(_bs2.Count()));
            Console.WriteLine("BarSeries Count: {0}", _bs2.Count());
        }
        [Test]
        public void VerifyBarSeriesItems()
        {
            //Assert.That(_bs1.Count(), Is.EqualTo(_bs2.Count()));
            //Console.WriteLine("BarSeries Count: {0}", _bs2.Count());

            //var ab1 = _bs1.ToArray();
            //var ab2 = _bs2.ToArray();

            //Assert.That(ab1.Count(), Is.EqualTo(ab2.Count()));

            //var cnt = ab1.Count();
            //for(var i=0;i<cnt;i++)
            //{
            //    // ((Bar) ab1[i]).CompareTo((Bar)ab2[i]);
            //    Assert.IsTrue((ab1[i]).CompareTo(ab2[i]));
            //}
            VerifySeriesItems(_bs1, _bs2);

        }
        [Test]
        public void VerifyBarSeriesItemsSum()
        {
            VerifySumOfBars(_bs1, _bs2);
        }

        [Test]
        public void VerifySerialization()
        {
            Console.WriteLine("BarsToProcess: {0}", _bs1.Count());

            var bytes = Serialize((IEnumerable<Dto.Bar>)_bs1);

            Console.WriteLine("Serialized bytes: {0}", bytes.Count());
            // GZip
            var bytesPacked = Compress(bytes);
            Console.WriteLine("Compressed bytes: {0}", bytesPacked.Count());

            // GZip UnPack
            var bytesUnPacked = DeCompress(bytesPacked);
            Console.WriteLine("Decompressed bytes: {0}", bytesUnPacked.Count());

            Assert.That(bytes.Length, Is.EqualTo(bytesUnPacked.Length));
            
            var bs11 = DeSerialize(bytes);
            Assert.That(_bs1.Count(), Is.EqualTo(bs11.Count()));

            Console.WriteLine("DeSerialized Bars: {0}", bs11.Count());

            VerifySumOfBars(_bs1, bs11);

            VerifySeriesItems(_bs1, bs11);

        }

        [Test]
        public void Verify_Ser_Zip_UnZip_DeSer_InLoop()
        {
            var dt = _timeSeriesStat.FirstDate.Date;
            //var dt = new DateTime(2015,12,11);
            var lastDt = _timeSeriesStat.LastDate.Date;
            while (dt.Date.IsLessThan(lastDt.Date))
            {
                Console.WriteLine("Date: {0}", dt.Date.ToString("yyyy-MM-dd"));
                var bs = GetBarSeries(dt);
                VerifySerializationZipUnZipDeSerialization(bs);
                dt = dt.AddDays(1);
            }
        }

        private void GetSeriesStat()
        {
            _timeSeriesStat = _timeSeriesStatwebClient.GetTimeSeriesStat("SiZ5", 5);
            Assert.IsNotNull(_timeSeriesStat, "SeriesStat should Be not Null");
            Assert.That(_timeSeriesStat.Count, Is.GreaterThan(0));
            Console.WriteLine("SeriesStat: {0}", _timeSeriesStat.ToString());
        }
        private IEnumerable<IBarSimple> GetBarSeries()
        {
            var bs = _barWebClient.GetSeries(_timeSeriesStat.Id, new DateTime(2015,10,01).Date);
            Assert.IsNotNull(bs, "Bar Series should Be not Null");
            return bs;
        }
        private IEnumerable<IBarSimple> GetBarSeries(DateTime dt)
        {
            var bs = _barWebClient.GetSeries(_timeSeriesStat.Id, dt.Date);
            Assert.IsNotNull(bs, "Bar Series should Be not Null");
            return bs;
        }

        private void VerifySeriesItems(IEnumerable<IBarSimple> bs1, IEnumerable<IBarSimple> bs2)
        {
            Console.WriteLine("Verification: Bar Series Items");

            Assert.That(bs1.Count(), Is.EqualTo(bs2.Count()));
            Console.WriteLine("BarSeries Count: {0}", bs2.Count());

            var ab1 = bs1.ToArray();
            var ab2 = bs2.ToArray();

            Assert.That(ab1.Count(), Is.EqualTo(ab2.Count()));

            var cnt = ab1.Count();
            for (var i = 0; i < cnt; i++)
            {
                // ((Bar) ab1[i]).CompareTo((Bar)ab2[i]);
                Assert.IsTrue((ab1[i]).CompareTo(ab2[i]));
            }
        }

        private void VerifySumOfBars(IEnumerable<IBarSimple> bs1, IEnumerable<IBarSimple> bs2)
        {
            Console.WriteLine("Verification: Sum Of Bars");
            var b1 = new Dto.Bar();
            var b2 = new Dto.Bar();

            foreach (var b in bs1)
            {
                b1.Open += b.Open;
                b1.High += b.High;
                b1.Low += b.Low;
                b1.Close += b.Close;
                b1.Volume += b.Volume;
            }
            foreach (var b in bs2)
            {
                b2.Open += b.Open;
                b2.High += b.High;
                b2.Low += b.Low;
                b2.Close += b.Close;
                b2.Volume += b.Volume;
            }

            Assert.That(b1.Open, Is.EqualTo(b2.Open));
            Assert.That(b1.Low, Is.EqualTo(b2.Low));
            Assert.That(b1.High, Is.EqualTo(b2.High));
            Assert.That(b1.Close, Is.EqualTo(b2.Close));
            Assert.That(b1.Volume, Is.EqualTo(b2.Volume));

            Assert.IsTrue((b1).CompareTo(b2));

            StringAssert.AreEqualIgnoringCase(b1.ToString(), b2.ToString());
            //CollectionAssert.AreEqual(bs1, bs2, "Collection should Be Equal");

            // Assert.Pass("Verify Passed...");          
        }

        private void VerifySerializationZipUnZipDeSerialization(IEnumerable<IBarSimple> bs)
        {
            // Console.WriteLine("BarsToProcess: {0}", bs.Count());

            var bytes = Serialize((IEnumerable<Dto.Bar>)bs);

            // Console.WriteLine("Serialized bytes: {0}", bytes.Count());
            // GZip
            var bytesPacked = Compress(bytes);
            //Console.WriteLine("Compressed bytes: {0}", bytesPacked.Count());

            // GZip UnPack
            var bytesUnPacked = DeCompress(bytesPacked);
            //Console.WriteLine("Decompressed bytes: {0}", bytesUnPacked.Count());

            Assert.That(bytes.Length, Is.EqualTo(bytesUnPacked.Length));

            var bs11 = DeSerialize(bytes);
            Assert.That(bs.Count(), Is.EqualTo(bs11.Count()));

            // Console.WriteLine("DeSerialized Bars: {0}", bs11.Count());

            VerifySumOfBars(bs, bs11);

            VerifySeriesItems(bs, bs11);
        }

        private byte[] Serialize(IEnumerable<Dto.Bar> bs)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes;
            MemoryStream ms = new MemoryStream();

            var lst = bs.Select(b => b.ToStr()).ToList();

            bf.Serialize(ms, lst);
            ms.Seek(0, 0);
            
            var bts = ms.ToArray();

            return bts;
        }

        private IEnumerable<IBarSimple> DeSerialize(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, 0);

            var ss = (List<string>)bf.Deserialize(ms);
            var bs = ss.Select(s => s.ToBarDto()).ToList();
            return bs;
        }

        private byte[] Compress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                return ms.ToArray();
            }
        }

        private byte[] DeCompress(byte[] bytes)
        {
            using (GZipStream gzstream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream mstr = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = gzstream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            mstr.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return mstr.ToArray();
                }
            }
        }
    }
}
