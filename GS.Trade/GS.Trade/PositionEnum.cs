using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade
{
    public enum PositionChangedEnum : short { Registered = 0, Opened = 1, Closed = 2, ReSizedUp = 3, ReSizedDown = 5, Reversed = 6 }
}
