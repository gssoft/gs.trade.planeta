using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.Dto;
using NUnit.Framework.Constraints;

namespace GS.Trade.Timeseries.NuTests
{
    public class BarPacker
    {
        public string Pack(IBarSimple b)
        {
            string bar;

            var dt = b.DT.ToLongInSec();
            
            if (b.Open == b.Close) // -- _|_ `|` 
            {
                if (b.Open == b.High && b.Open == b.Low) // length = 1
                {
                    bar = string.Join(",", dt,"1", b.Open, b.Volume);
                }
                else
                {
                    // // length = 3
                    bar = string.Join(",", dt,"3",
                        dt, b.Open,
                        b.High,
                        b.Low,
                        b.Volume
                        );
                }
            }
            else
            {   
                // length = 2
                if ((b.Open == b.High && b.Close == b.Low) ||
                    (b.Open == b.Low && b.Close == b.High))
                {
                    bar = string.Join(",", dt, "2", b.High, b.Low);
                }
                else
                {
                    //length = 4
                    bar = string.Join(",", dt,"4",
                        b.Open,
                        b.High,
                        b.Low,
                        b.Close,
                        b.Volume);
                }
            }
            return bar;
        }

        

        public IBarSimple UnPack(string strBar)
        {
            var split = strBar.Split(new [] {','});
            if (split.Count() < 3)
                return null;
            var b = new Bar();
            switch (Int32.Parse(split[1]))
            {
                case 1:
                    b.DT = DateTime.Parse(split[0]);
                    break;
            }

            return new Bar();
        }
    }

    public class BarList
    {
        public List<IBarSimple> Bars { get; set; }

        public BarList()
        {
            DateTime dt = DateTime.Now.ToLongInSec().ToDateTime();
            Bars = new List<IBarSimple>
            {
                new Bar // 9
                {
                    DT = dt,
                    Open = 100, High = 100, Low = 100, Close = 100,
                    Volume = 150
                },
                new Bar // 8
                {
                    DT = dt,
                    Open = 100, High = 150, Low = 100, Close = 100,
                    Volume = 150
                },
                new Bar // 7
                {
                    DT = dt,
                    Open = 100, High = 100, Low = 50, Close = 100,
                    Volume = 150
                },
                new Bar // 6
                {
                    DT = dt,
                    Open = 100, High = 200, Low = 100, Close = 200,
                    Volume = 150
                },
                new Bar // 5
                {
                    DT = dt,
                    Open = 150, High = 150, Low = 100, Close = 100,
                    Volume = 150
                },
                new Bar // 4
                {
                    DT = dt,
                    Open = 100, High = 150, Low = 50, Close = 150,
                    Volume = 150
                },
                new Bar // 3
                {
                    DT = dt,
                    Open = 100, High = 200, Low = 100, Close = 150,
                    Volume = 150
                },
                new Bar // 2
                {
                    DT = dt,
                    Open = 100, High = 100, Low = 50, Close = 75,
                    Volume = 150
                },
                new Bar // 1
                {
                    DT = dt,
                    Open = 100, High = 150, Low = 50, Close = 50,
                    Volume = 150
                },
                 new Bar // 0
                {
                    DT = dt,
                    Open = 100, High = 200, Low = 50, Close = 150,
                    Volume = 150
                },
                 new Bar // 1
                {
                    DT = dt,
                    Open = 150, High = 200, Low = 50, Close = 100,
                    Volume = 150
                }
            };
        }
    }
}
