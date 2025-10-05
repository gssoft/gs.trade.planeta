using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using GS.Compress;
using GS.Extension;
using GS.Serialization;
using GS.Trade.Dto;
using GS.Trade.Extensions;
using GS.Trade.TimeSeries.FortsTicks3.Dal;
using GS.Trade.TimeSeries.Model;
using NUnit.Framework;

namespace GS.Trade.TimeSeries.Test_01
{
    [TestFixture]
    public class FirstTestFixture
    {
        private IList<Dto.Bar> _bs;
        private DateTime _dt1, _dt2;
        private TimeSpan _ts1, _ts2;

        private IList<FortsTicks3.Model.Bar> _fbs;

        [OneTimeSetUp]
        public void Init()
        {
            Assert.DoesNotThrow(() => {
                using (var tmx = new TimeSeriesContext())
                {
                    _bs = tmx.GetBarsDto(20, 20151001).ToList();
                }
                using (var frts = new FortsTicksContext3())
                {
                    _fbs = frts.Bars.Where(b => b.SeriesID == 3259).ToList();
                }
            });
        }

        [Test]
        public void Test_GetBars()
        {
            using (var tmx = new TimeSeriesContext())
            {
                var dt1 = DateTime.Now;
                var bs = tmx.GetBarsDto(20, 20151013).ToListAsync();
                var dt2 = DateTime.Now;
                var t1 = dt2 - dt1;
                ConsoleAS.ConsoleAsync.WriteLineT("Elapsed: {0}", dt2 - dt1);
                Assert.IsNotNull(bs);
                dt1 = DateTime.Now;
            }
        }

        [Test]
        public void GetStats()
        {
            DateTime dt1,dt2;
            TimeSpan t1, t2;
            using (var tmx = new TimeSeriesContext())
            {
                dt1 = DateTime.Now;
                var st1 = tmx.StatSelect031().ToList();
                dt2 = DateTime.Now;
                t1 = dt2 - dt1; 
                ConsoleAS.ConsoleAsync.WriteLineT("T1: {0}", dt2 - dt1);

                dt1 = DateTime.Now;
                var st2 = tmx.StatSelect032().ToList();
                dt2 = DateTime.Now;
                t2 = dt2 - dt1;
                ConsoleAS.ConsoleAsync.WriteLineT("T2: {0}", dt2 - dt1);

                dt1 = DateTime.Now;
                var st3 = tmx.StatSelect031().ToList();
                dt2 = DateTime.Now;
                t1 = dt2 - dt1;
                ConsoleAS.ConsoleAsync.WriteLineT("T1: {0}", dt2 - dt1);

                Assert.IsTrue(st1.Count > 0);
                Assert.IsTrue(st2.Count > 0);
                Assert.IsTrue(st3.Count > 0);

               // Assert.IsTrue(t2 < t1);
            }
        }

        [Test]
        public void GetBars2()
        {
            using (var tmx = new TimeSeriesContext())
            {
                DateTime dt1, dt2;
                TimeSpan t1, t2;
                try
                {
                    dt1 = DateTime.Now;
                    var bs = tmx.Bars.AsNoTracking().ToList();
                    dt2 = DateTime.Now;
                    t1 = dt2 - dt1;
                    ConsoleAS.ConsoleSync.WriteLineT("T1: {0}", t1);

                    dt1 = DateTime.Now;
                    bs = tmx.Bars.AsNoTracking().ToList();
                    dt2 = DateTime.Now;
                    t2 = dt2 - dt1;
                    ConsoleAS.ConsoleSync.WriteLineT("T2 : {0}", t2);
                    Assert.IsTrue(t2 < t1);
                }
                catch (Exception ex)
                {
                    ConsoleAS.ConsoleSync.WriteLineT(ex.Message);
                    ConsoleAS.ConsoleSync.WriteLineT(ex.ToString());
                }
            }
         }

        [Test]
        public void BinarySerializeBarSeries()
        {
            Assert.DoesNotThrow(() =>
            {
                var bars = _bs;
                
                _dt1 = DateTime.Now;
                var bytesBar = BinarySerialization.SerializeToByteArray(bars);
                var bsBarsZip = Compressor.Compress(bytesBar);
                var bytesBack = Compressor.DeCompress(bsBarsZip);
                var barsBack = BinarySerialization.DeSerialize<List<Dto.Bar>>(bytesBack);
                // barsBack[10].Open = 123;
                _dt2 = DateTime.Now;
                var t1 = Diff(_dt1, _dt2);

                VerifyCollection(bars, barsBack);
           
                _dt1 = DateTime.Now;
                var barsStr = bars.Select(b => b.ToStr()).ToList();
                var bytesBarStr = BinarySerialization.SerializeToByteArray(barsStr);
                var bsBarsStrZip = Compressor.Compress(bytesBarStr);
                var bytesBarStrBack = Compressor.DeCompress(bsBarsStrZip);
                var barsStrBack = BinarySerialization.DeSerialize<List<string>>(bytesBarStrBack);
                var barsDto = barsStrBack.Select(s => s.ToBarDto()).ToList();
                
                _dt2 = DateTime.Now;
                var t2 = Diff(_dt1, _dt2);
                
                VerifyCollection(bars, barsDto);

                _dt1 = DateTime.Now;
                var barsStrPacked = bars.Select(b => b.ToStrPacked()).ToList();
                var bytesBarStrPacked = BinarySerialization.SerializeToByteArray(barsStrPacked);
                var bsBarsStrPackedZip = Compressor.Compress(bytesBarStrPacked);
                var bytesBarStrPackedBack = Compressor.DeCompress(bsBarsStrPackedZip);
                var barsStrPackedBack = BinarySerialization.DeSerialize<List<string>>(bytesBarStrPackedBack);
                var barsDtoPack = barsStrPackedBack.Select(BarSimpleExt.ToBarSimple).ToList();

                _dt2 = DateTime.Now;
                var t3 = Diff(_dt1, _dt2);

                VerifyCollection(bars, barsDtoPack);

                Console.WriteLine("Bars: {0}, BytesBar: {1}, BytesBarStr: {2}, BytesBarStrPacked: {3}",
                                    bars.Count(), bytesBar.Count(), bytesBarStr.Count(), bytesBarStrPacked.Count());

                // Console.Write("Compressor");

                // var bsBarsZip = Compressor.Compress(bytesBar);
                // var bsBarsStrZip = Compressor.Compress(bytesBarStr);
                //var bsBarsStrPackedZip = Compressor.Compress(bytesBarStrPacked);

                Console.WriteLine("BytesBar: {0}, BytesBarStr: {1}, BytesBarStrPacked: {2}",
                                    bsBarsZip.Count(), bsBarsStrZip.Count(), bsBarsStrPackedZip.Count());

                Console.WriteLine("t1: {0} t2: {1}, t3: {2}", t1.ToString("g"), t2.ToString("g"), t3.ToString("g"));
                
            });
        }
        [Test]
        public void BinarySerializeBarSeries2()
        {
            var sw = new Stopwatch();
            Assert.DoesNotThrow(() =>
            {
                sw.Start();
                var bars = _fbs.Select(b=>new BarDto
                {
                    DT = b.DT,
                    Open = b.Open,
                    High = b.High,
                    Low = b.Low,
                    Close = b.Close,
                    Volume = b.Volume
                }).ToList();
                sw.Stop();
                Console.WriteLine($"Select Bars: {sw.Elapsed}");
                sw.Reset();

                _dt1 = DateTime.Now;

                sw.Start();
                var bytesBar = BinarySerialization.SerializeToByteArray(bars);
                var bsBarsZip = Compressor.Compress(bytesBar);
                var bytesBack = Compressor.DeCompress(bsBarsZip);
                var barsBack = BinarySerialization.DeSerialize<List<BarDto>>(bytesBack);
                sw.Stop();
                Console.WriteLine($"Bars: {sw.Elapsed}");
                sw.Reset();
                // barsBack[10].Open = 123;
                _dt2 = DateTime.Now;
                var t1 = Diff(_dt1, _dt2);

                VerifyCollection(bars, barsBack);

                _dt1 = DateTime.Now;
                sw.Start();

                var barsStr = bars.Select(b => b.ToStr()).ToList();
                var bytesBarStr = BinarySerialization.SerializeToByteArray(barsStr);
                var bsBarsStrZip = Compressor.Compress(bytesBarStr);
                var bytesBarStrBack = Compressor.DeCompress(bsBarsStrZip);
                var barsStrBack = BinarySerialization.DeSerialize<List<string>>(bytesBarStrBack);
                var barsDto = barsStrBack.Select(BarBaseExt.StrToBar).ToList();

                // var barsDtos = barsStrBack.Select(new BarDto().StrToBar).ToList();

                sw.Stop();
                Console.WriteLine($"StringBars: {sw.Elapsed}");
                sw.Reset();

                _dt2 = DateTime.Now;
                var t2 = Diff(_dt1, _dt2);

                VerifyCollection(bars, barsDto);

                _dt1 = DateTime.Now;

                sw.Start();
                var barsStrPacked = bars.Select(b => b.ToStrPacked()).ToList();
                var bytesBarStrPacked = BinarySerialization.SerializeToByteArray(barsStrPacked);
                var bsBarsStrPackedZip = Compressor.Compress(bytesBarStrPacked);
                var bytesBarStrPackedBack = Compressor.DeCompress(bsBarsStrPackedZip);
                var barsStrPackedBack = BinarySerialization.DeSerialize<List<string>>(bytesBarStrPackedBack);
                var barsDtoPack = barsStrPackedBack.Select(BarBaseExt.StrPackedToBar).ToList();

                sw.Stop();
                Console.WriteLine($"StrPacked Bars: {sw.Elapsed}");
                sw.Reset();

                _dt2 = DateTime.Now;
                var t3 = Diff(_dt1, _dt2);

                VerifyCollection(bars, barsDtoPack);

                Console.WriteLine("Bars: {0}, BytesBar: {1}, BytesBarStr: {2}, BytesBarStrPacked: {3}",
                                    bars.Count(), bytesBar.Count(), bytesBarStr.Count(), bytesBarStrPacked.Count());

                // Console.Write("Compressor");

                // var bsBarsZip = Compressor.Compress(bytesBar);
                // var bsBarsStrZip = Compressor.Compress(bytesBarStr);
                //var bsBarsStrPackedZip = Compressor.Compress(bytesBarStrPacked);

                Console.WriteLine("BytesBar: {0}, BytesBarStr: {1}, BytesBarStrPacked: {2}",
                                    bsBarsZip.Count(), bsBarsStrZip.Count(), bsBarsStrPackedZip.Count());

                Console.WriteLine("t1: {0} t2: {1}, t3: {2}", t1.ToString("g"), t2.ToString("g"), t3.ToString("g"));

            });
        }

        private TimeSpan Diff(DateTime dt1, DateTime dt2)
        {
            return dt2 - dt1;
        }
        private void VerifyCollection(IReadOnlyList<IBarBase> bs1, IReadOnlyList<IBarBase> bs2)
        {
            Assert.That(bs1.Count(), Is.EqualTo(bs2.Count()));
            var cnt = bs1.Count();
            for (var i = 0; i < cnt; i++)
                Assert.IsTrue(bs1[i].IsEqual(bs2[i]));
        }
        private void VerifyCollection(IList<Dto.Bar> bs1, IList<Dto.Bar> bs2)
        {
            Assert.That(bs1.Count(), Is.EqualTo(bs2.Count()));
            var cnt = bs1.Count();
            for (var i = 0; i < cnt; i++)
                Assert.IsTrue(bs1[i].CompareTo(bs2[i]));
        }
        private void VerifyCollection(IList<Dto.Bar> bs1, IList<IBarSimple> bs2)
        {
            Assert.That(bs1.Count(), Is.EqualTo(bs2.Count()));
            var cnt = bs1.Count();
            for (var i = 0; i < cnt; i++)
                Assert.IsTrue(bs1[i].CompareTo(bs2[i]));
        }

        //private static Dto.Bar ToBarSimple(string sb)
        //{
        //    if(string.IsNullOrWhiteSpace(sb))
        //        throw new Exception("String to Parse to BAr Is Empty");

        //    var split = sb.Split(new[] { ',' });

        //    if (split.Count() < 4)
        //        throw new Exception("Too few items in String to Parse to Bar: " + sb);

        //    var format = Int32.Parse(split[(int)BarPackedParseEnum.Format]);
        //    Dto.Bar b = null;
        //    switch (format)
        //    {
        //        case 0:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P4]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 1:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 2:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 3:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 4:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 5:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 6:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 7:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 8:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 9:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 10:
        //            b = new Dto.Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int)BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
        //                Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
        //                Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
        //            };
        //            break;
        //    }
        //    return b;
        //}
        //public static string ToStrPacked(IBarSimple b)
        //{
        //    string r = string.Empty;

        //    if (b.Open.IsNoEquals(b.Close))
        //    {
        //        if (b.Open != b.High && b.Open != b.Low &&
        //            b.High != b.Low && b.High != b.Close &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "0",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture),
        //                b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( b.Open != b.High && b.High != b.Close && b.Low == b.Close
        //            )
        //        {      // OHL l=c
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "1",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }// olc o=h
        //        else if ( b.Open == b.High && b.Open != b.Low && b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "2",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture),
        //                b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }  // ohc o=l
        //        else if ( b.Open != b.High && b.Open == b.Low && b.High != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "3",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }// ohl h=c
        //        else if ( b.Open != b.Low && b.High == b.Close && b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "4",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                );
        //        } // ol h=o l=c 
        //        else if ( b.Open == b.High && b.Low == b.Close )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "5",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }  // oh o=l h=c
        //        else if ( b.Open == b.Low && b.High == b.Close )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "6",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //    }
        //    else  // 0 == C
        //    {
        //        if (b.Open == b.High && b.Open != b.Low )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "7",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( b.Open != b.High && b.Open == b.Low )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "8",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if (b.Open == b.High && b.Open == b.Low)
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "9",
        //                b.Open.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else
        //        {
        //            r = string.Join(",",
        //               b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //               b.Volume.ToString(CultureInfo.InvariantCulture),
        //               "10",
        //               b.Open.ToString(CultureInfo.InvariantCulture),
        //               b.High.ToString(CultureInfo.InvariantCulture),
        //               b.Low.ToString(CultureInfo.InvariantCulture)
        //               );
        //        }
        //    }
        //    return r;
        //}
    }
}
