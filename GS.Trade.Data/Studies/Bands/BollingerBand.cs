using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Data.Studies.Bands
{
    public class BollingerBand : StdDevBand
    {
        public BollingerBand(string name, ITicker ticker, int timeIntSeconds, BarValue be,  int length, float kStdDevUp, float kStdDevDown)
            : base(name, ticker, timeIntSeconds, be, MaType.Simple, length, 0, be, length, 0, kStdDevUp, kStdDevDown)
        {            
        }
    }
}
