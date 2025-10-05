using System;
using System.Globalization;
using System.Linq;
using GS.Extension;
using GS.Trade.Dto;

namespace GS.Trade.Extensions
{
    public static class BarSimpleExt
    {
        public static string ToStr(this IBarSimple b)
        {
            return string.Join(",",
                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                b.Open.ToString(CultureInfo.InvariantCulture),
                b.High.ToString(CultureInfo.InvariantCulture),
                b.Low.ToString(CultureInfo.InvariantCulture),
                b.Close.ToString(CultureInfo.InvariantCulture),
                b.Volume.ToString(CultureInfo.InvariantCulture)
                );
        }
        public static string ToStr(this IBarSimple b, string memberseparator, string endseparator)
        {
            return string.Join(memberseparator,
                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                b.Open.ToString(CultureInfo.InvariantCulture),
                b.High.ToString(CultureInfo.InvariantCulture),
                b.Low.ToString(CultureInfo.InvariantCulture),
                b.Close.ToString(CultureInfo.InvariantCulture),
                b.Volume.ToString(CultureInfo.InvariantCulture)
                ) + endseparator;
        }


        public static IBarSimple ToBarDto(this string sb)
        {
            var split = sb.Split(new[] { ',' });
            
            if(split.Count()<6)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            return new Dto.Bar
            {
                DT = DateTime.
                    ParseExact(split[(int)BarSimpleParseFieldsEnum.DateTime],
                                        "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                Open = double.Parse(split[(int)BarSimpleParseFieldsEnum.Open]),
                High = double.Parse(split[(int)BarSimpleParseFieldsEnum.High]),
                Low = double.Parse(split[(int)BarSimpleParseFieldsEnum.Low]),
                Close = double.Parse(split[(int)BarSimpleParseFieldsEnum.Close]),
                Volume = double.Parse(split[(int)BarSimpleParseFieldsEnum.Volume])
            };
        }
        public static bool CompareTo(this IBarSimple b1, IBarSimple b2)
        {
            return
                b1.DT == b2.DT &&
                b1.Open == b2.Open &&
                b1.High == b2.High &&
                b1.Low == b2.Low &&
                b1.Close == b2.Close &&
                b1.Volume == b2.Volume;
        }

        //public static string ToStrPacked(this IBarSimple b)
        //{
        //    string r = string.Empty; 
        //        //= string.Join(",",
        //        //b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //        //b.Volume.ToString(CultureInfo.InvariantCulture));

        //    if (b.Open.IsNoEquals(b.Close))
        //    {
        //        //if (b.Open.IsNoEquals(b.High) && b.Open.IsNoEquals(b.Low) && 
        //        //    b.High.IsNoEquals(b.Low) && b.High.IsNoEquals(b.Close) &&
        //        //    b.Low.IsNoEquals(b.Close))
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
        //        else if (
        //            //b.Open.IsNoEquals(b.High) && b.Open.IsNoEquals(b.Low) &&
        //            //b.High.IsNoEquals(b.Low) && b.High.IsNoEquals(b.Close) &&
        //            //b.Low.IsNoEquals(b.Close))
        //            b.Open != b.High && b.Open != b.Low &&
        //            b.High != b.Low && b.High != b.Close &&
        //            b.Low == b.Close
        //            )
        //        {      // OHL l=c
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "1",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                // b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( // olc o=h
        //            b.Open == b.High && b.Open != b.Low &&
        //            b.High != b.Low && b.High != b.Close &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "2",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                // b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture),
        //                b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( // ohc o=l
        //            b.Open != b.High && b.Open == b.Low &&
        //            b.High != b.Low && b.High != b.Close &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "3",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                //b.Low.ToString(CultureInfo.InvariantCulture)
        //                b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( // ohl h=c
        //            b.Open != b.High && b.Open != b.Low &&
        //            b.High != b.Low && b.High == b.Close &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "4",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                // b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( // ol h=o l=c 
        //            b.Open == b.High && b.Open != b.Low &&
        //            b.High != b.Low && b.High != b.Close &&
        //            b.Low == b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "5",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                //b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                // b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if ( // oh o=l h=c
        //            b.Open != b.High && b.Open == b.Low &&
        //            b.High != b.Low && b.High == b.Close &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "6",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture)
        //                // b.Low.ToString(CultureInfo.InvariantCulture)
        //                // b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //    }
        //    else  // 0 == C
        //    { 
        //        if (b.Open == b.High && b.Open != b.Low &&
        //            b.High != b.Low  &&
        //            b.Low != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "7",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                //b.High.ToString(CultureInfo.InvariantCulture),
        //                b.Low.ToString(CultureInfo.InvariantCulture)
        //                // b.Close.ToString(CultureInfo.InvariantCulture)
        //                );
        //        }
        //        else if (
        //            b.Open != b.High && b.Open == b.Low &&
        //            b.High != b.Low && b.High != b.Close
        //            )
        //        {
        //            r = string.Join(",",
        //                b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //                b.Volume.ToString(CultureInfo.InvariantCulture),
        //                "8",
        //                b.Open.ToString(CultureInfo.InvariantCulture),
        //                b.High.ToString(CultureInfo.InvariantCulture)
        //               // b.Low.ToString(CultureInfo.InvariantCulture),
        //               // b.Close.ToString(CultureInfo.InvariantCulture)
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
        //               "0",
        //               b.Open.ToString(CultureInfo.InvariantCulture),
        //               b.High.ToString(CultureInfo.InvariantCulture),
        //               b.Low.ToString(CultureInfo.InvariantCulture),
        //               b.Close.ToString(CultureInfo.InvariantCulture)
        //               );
        //        }
                
        //    }
        //    return r;
        //}

        //public static IBarSimple ToBarSimple(this string sb)
        //{
            
        //    var split = sb.Split(new[] { ',' });

        //    if (split.Count() < 4)
        //        throw new Exception("Too few items in String to Parse to Bar: " + sb);
            
        //    var format = Int32.Parse(split[(int)BarPackedParseEnum.Format]);
        //    Bar b = null;
        //    switch (format)
        //    {
        //        case 0:
        //            b = new Bar
        //            {
        //                DT = DateTime.ParseExact(split[(int) BarPackedParseEnum.DateTime],
        //                    "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                Open = double.Parse(split[(int) BarPackedParseEnum.P1]),
        //                High = double.Parse(split[(int) BarPackedParseEnum.P2]),
        //                Low = double.Parse(split[(int) BarPackedParseEnum.P3]),
        //                Close = double.Parse(split[(int) BarPackedParseEnum.P4]),
        //                Volume = double.Parse(split[(int) BarPackedParseEnum.Volume])
        //            };
        //            break;
        //        case 1:
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            b = new Bar
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
        //            case 9:
        //            b = new Bar
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
        //    }
        //    return b;
        //}

        public static Dto.Bar ToBarSimple(string sb)
        {
            if (string.IsNullOrWhiteSpace(sb))
                throw new Exception("String to Parse to BAr Is Empty");

            var split = sb.Split(new[] { ',' });

            if (split.Count() < 4)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            var format = Int32.Parse(split[(int)BarPackedParseEnum.Format]);
            Dto.Bar b = null;
            switch (format)
            {
                case 0:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P4]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 1:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 2:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 3:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 4:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 5:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 6:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 7:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 8:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 9:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
                case 10:
                    b = new Dto.Bar
                    {
                        DT = DateTime.ParseExact(split[(int)BarPackedParseEnum.DateTime],
                            "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        Open = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        High = double.Parse(split[(int)BarPackedParseEnum.P2]),
                        Low = double.Parse(split[(int)BarPackedParseEnum.P3]),
                        Close = double.Parse(split[(int)BarPackedParseEnum.P1]),
                        Volume = double.Parse(split[(int)BarPackedParseEnum.Volume])
                    };
                    break;
            }
            return b;
        }
        public static string ToStrPacked(this IBarSimple b)
        {
            string r = string.Empty;

            if (b.Open.IsNoEquals(b.Close))
            {
                if (b.Open != b.High && b.Open != b.Low &&
                    b.High != b.Low && b.High != b.Close &&
                    b.Low != b.Close
                    )
                {
                    r = string.Join(",",
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
                    r = string.Join(",",
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
                    r = string.Join(",",
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
                    r = string.Join(",",
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
                    r = string.Join(",",
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
                    r = string.Join(",",
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "5",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                }  // oh o=l h=c
                else if (b.Open == b.Low && b.High == b.Close)
                {
                    r = string.Join(",",
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
                    r = string.Join(",",
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "7",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.Low.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else if (b.Open != b.High && b.Open == b.Low)
                {
                    r = string.Join(",",
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "8",
                        b.Open.ToString(CultureInfo.InvariantCulture),
                        b.High.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else if (b.Open == b.High && b.Open == b.Low)
                {
                    r = string.Join(",",
                        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
                        b.Volume.ToString(CultureInfo.InvariantCulture),
                        "9",
                        b.Open.ToString(CultureInfo.InvariantCulture)
                        );
                }
                else
                {
                    r = string.Join(",",
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
