using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.Dto;

namespace GS.Trade.Extensions
{
    public static class BarBaseExt
    {
        public static string ToStr(this IBarBase b)
        {
            return string.Join(";",
                b.DT.ToString("yyyyMMddHHmmss"),
                b.Open.ToString(CultureInfo.InvariantCulture),
                b.High.ToString(CultureInfo.InvariantCulture),
                b.Low.ToString(CultureInfo.InvariantCulture),
                b.Close.ToString(CultureInfo.InvariantCulture),
                b.Volume.ToString(CultureInfo.InvariantCulture)
                );
        }
        public static string ToStr(this IBarBase b, string memberseparator, string endseparator)
        {
            return string.Join(memberseparator,
                b.DT.ToString("yyyyMMddHHmmss"),
                b.Open.ToString(CultureInfo.InvariantCulture),
                b.High.ToString(CultureInfo.InvariantCulture),
                b.Low.ToString(CultureInfo.InvariantCulture),
                b.Close.ToString(CultureInfo.InvariantCulture),
                b.Volume.ToString(CultureInfo.InvariantCulture)
                ) + endseparator;
        }


        public static IBarBase StrToBar(string sb)
        {
            var split = sb.Split(';');

            if (split.Length < 6)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            return new BarDto
            {
                DT = DateTime.ParseExact(split[(int)BarSimpleParseFieldsEnum.DateTime],
                                        "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture),
                // DT = DateTime.Parse(split[(int)BarSimpleParseFieldsEnum.DateTime]),
                Open = double.Parse(split[(int)BarSimpleParseFieldsEnum.Open]),
                High = double.Parse(split[(int)BarSimpleParseFieldsEnum.High]),
                Low = double.Parse(split[(int)BarSimpleParseFieldsEnum.Low]),
                Close = double.Parse(split[(int)BarSimpleParseFieldsEnum.Close]),
                Volume = double.Parse(split[(int)BarSimpleParseFieldsEnum.Volume])
            };
        }
        public static IBarBaseRW StrToBar(this IBarBaseRW bar, string sb)
        {
            var split = sb.Split(';');

            if (split.Length < 6)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            bar.DT = DateTime.ParseExact(split[(int) BarSimpleParseFieldsEnum.DateTime],
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture);
            bar.Open = double.Parse(split[(int) BarSimpleParseFieldsEnum.Open]);
            bar.High = double.Parse(split[(int) BarSimpleParseFieldsEnum.High]);
            bar.Low = double.Parse(split[(int) BarSimpleParseFieldsEnum.Low]);
            bar.Close = double.Parse(split[(int) BarSimpleParseFieldsEnum.Close]);
            bar.Volume = double.Parse(split[(int) BarSimpleParseFieldsEnum.Volume]);

            return bar;
        }

        public static bool IsEqual(this IBarBase b1, IBarBase b2)
        {
            return
                b1.DT == b2.DT &&
                b1.Open == b2.Open &&
                b1.High == b2.High &&
                b1.Low == b2.Low &&
                b1.Close == b2.Close &&
                b1.Volume == b2.Volume;
        }
        public static IBarBase StrPackedToBar(string sb)
        {
            if (string.IsNullOrWhiteSpace(sb))
                throw new Exception("String to Parse to BAr Is Empty");

            var split = sb.Split(';');

            if (split.Length < 4)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            var format = Int32.Parse(split[(int) BarPackedParseEnum.Format]);
            BarDto b = null;
            switch (format)
            {
                case 0:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P4]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 1:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 2:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 3:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 4:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 5:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 6:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 7:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 8:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 9:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
                case 10:
                    b = new BarDto
                    {
                        DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int) BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int) BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int) BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
                    };
                    break;
            }
            return b;
        }
        public static string ToStrPacked(this IBarBase b)
        {
            string r = string.Empty;
            string dlmt = ";";

            if (b.Open.IsNoEquals(b.Close))
            {
                if (b.Open != b.High && b.Open != b.Low &&
                    b.High != b.Low && b.High != b.Close &&
                    b.Low != b.Close
                    )
                {
                    r = string.Join(dlmt,
                        // b.DT.ToString("yyyyMMddHHmmss"),
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "0",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture),
                        b.Close.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else if (b.Open != b.High && b.High != b.Close && b.Low == b.Close
                    )
                {      // OHL l=c
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "1",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                }// olc o=h
                else if (b.Open == b.High && b.Open != b.Low && b.Low != b.Close
                    )
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "2",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture),
                        b.Close.ToString(CultureInfo.InvariantCulture)
                        );
                }  // ohc o=l
                else if (b.Open != b.High && b.Open == b.Low && b.High != b.Close
                    )
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "3",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture),
                        b.Close.ToString(CultureInfo.InvariantCulture)
                        );
                }// ohl h=c
                else if (b.Open != b.Low && b.High == b.Close && b.Low != b.Close
                    )
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "4",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                } // ol h=o l=c 
                else if (b.Open == b.High && b.Low == b.Close)
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "5",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                }  // oh o=l h=c
                else if (b.Open == b.Low && b.High == b.Close)
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "6",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture)
                        );
                }
            }
            else  // 0 == C
            {
                if (b.Open == b.High && b.Open != b.Low)
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "7",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else if (b.Open != b.High && b.Open == b.Low)
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "8",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else if (b.Open == b.High && b.Open == b.Low)
                {
                    r = string.Join(dlmt,
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "9",
                        b.Open.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else
                {
                    r = string.Join(dlmt,
                       b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                       b.Volume.ToString(CultureInfo.InvariantCulture),
                       "10",
                       b.Open.ToString(CultureInfo.InvariantCulture),
                       b.High.ToString(CultureInfo.InvariantCulture),
                       b.Low.ToString(CultureInfo.InvariantCulture)
                       );
                }
            }
            return r;
        }
    }
}
