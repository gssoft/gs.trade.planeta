using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies
{
    public class X1172 : X117
    {
        protected override void PreLong()
        {
            switch (Mode2)
            {
                case 1:
                    if (XTrend.IsUp2 && XTrend.IsPrevDown2)
                        MyReverseId = 0;
                    break;
                case 2:
                    if (XTrend.IsDown2)
                        MyReverseId = 0;
                    break;
            }
        }
        protected override void PreShort()
        {
            switch (Mode2)
            {
                case 1:
                    if (XTrend.IsDown2 && XTrend.IsPrevUp2)
                        MyReverseId = 0;
                    break;
                case 2:
                    if (XTrend.IsUp2)
                        MyReverseId = 0;
                    break;
            }
        }
        /*
        protected override void NoShortReverse()
        {
            KillAllOrder();
        }
        protected override void NoLongReverse()
        {
            KillAllOrder();
        }
        protected override void NoLongEntry()
        {
            KillAllOrder();
        }
        protected override void NoShortEntry()
        {
            KillAllOrder();
        }
         */ 
    }
}
