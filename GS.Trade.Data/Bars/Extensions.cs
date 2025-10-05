using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.Data.Bars
{
    public static class BarExtensions
    {
        //public static string ToStr(this IBarSimple b)
        //{
        //    return string.Join(",",
        //        b.DT.ToLongInSec().ToString(CultureInfo.InvariantCulture),
        //        b.Open.ToString(CultureInfo.InvariantCulture),
        //        b.High.ToString(CultureInfo.InvariantCulture),
        //        b.Low.ToString(CultureInfo.InvariantCulture),
        //        b.Close.ToString(CultureInfo.InvariantCulture),
        //        b.Volume.ToString(CultureInfo.InvariantCulture)
        //        );
        //}

        public static IBar ToBar(this string sb)
        {
            var split = sb.Split(new[] { ',' });
            
            if(split.Count()<6)
                throw new Exception("Too few items in String to Parse to Bar: " + sb);

            return new Bar
            {
                DT = DateTime.ParseExact(split[(int)BarSimpleParseFieldsEnum.DateTime], "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                Open = double.Parse(split[(int)BarSimpleParseFieldsEnum.Open]),
                High = double.Parse(split[(int)BarSimpleParseFieldsEnum.High]),
                Low = double.Parse(split[(int)BarSimpleParseFieldsEnum.Low]),
                Close = double.Parse(split[(int)BarSimpleParseFieldsEnum.Close]),
                Volume = double.Parse(split[(int)BarSimpleParseFieldsEnum.Volume])
            };
        }

        //public static bool CompareTo(this IBarSimple b1, IBarSimple b2)
        //{
        //    return
        //        b1.DT == b2.DT &&
        //        b1.Open == b2.Open &&
        //        b1.High == b2.High &&
        //        b1.Low == b2.Low &&
        //        b1.Close == b2.Close &&
        //        b1.Volume == b2.Volume;
        //}
    }
}
