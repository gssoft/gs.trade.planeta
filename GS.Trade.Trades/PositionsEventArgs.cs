using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Trades
{
    public class PositionsEventArgs : EventArgs, IPositionsEventArgs
    {
        public PositionsEventArgs(string whatsHappens, Position p)
        {
            WhatsHappens = whatsHappens;
            Position = p;
        }
        public string WhatsHappens { get; private set; }
        public IPosition Position { get; private set; }
    }
}
