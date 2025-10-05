
using GS.Extension;

namespace GS.Trade.Data.Studies.GS.xMa018
{
    public partial class Xma018 : TimeSeries
    {
        public int SwingCount;

        private int _trend2;
        private int _trend3;
        private int _trend5;

        private int _prevTrend2;
        private int _prevTrend3;

        private double _prevTrendLastHigh;
        private double _prevTrendLastMa;
        private double _prevTrendLastLow;

        private double _prevTrendLastHigh2;
        private double _prevTrendLastMa2;
        private double _prevTrendLastLow2;

        public int Trend2 => _trend2;

        public bool IsUp2 => _trend2 > 0;

        public bool IsDown2 => _trend2 < 0;

        public bool IsFlat2 => _trend2 == 0;
        public bool IsPrevUp2 => _prevTrend2 > 0;

        public bool IsPrevDown2 => _prevTrend2 < 0;

        public bool IsPrevFlat2 => _prevTrend2 == 0;

        public bool IsUp3 => _trend3 > 0;

        public bool IsDown3 => _trend3 < 0;

        public bool IsFlat3 => _trend3 == 0;

        public bool IsUp5 => _trend5 > 0;
        public bool IsDown5 => _trend5 < 0;
        public int Trend50 => _trend5;

        public double GetPrice(int operation, int code, out string comment)
        {
            comment = string.Empty;
            var currentLimitPrice = 0.0d;
            switch (code)
            {
                case 0:
                    if (IsFlat)
                    {
                        if (operation > 0)
                        {
                            comment = "Buy Flat.Ma";
                            currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                            return Ticker.ToMinMove(currentLimitPrice, +1);
                        }
                        if (operation < 0)
                        {
                            comment = "Sell Flat.Ma";
                            currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                            return Ticker.ToMinMove(currentLimitPrice, -1);
                        }
                    }
                    break;
                case 1:
                    if (operation > 0)
                    {
                        comment = "Buy Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0)
                    {
                        comment = "Sell Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 2:
                    if (IsImpulse && operation != 0)
                    {
                        comment = operation > 0 ? "Buy Impulse.Close" : "Sell Impulse.Close";
                        return _bars.Close;
                    }
                    break;
                case 3:
                    if (IsImpulse && Count > 2)
                    {
                        double price;
                        double value;
                        string explain;
                        if (operation > 0)
                        {
                            value = ((Item) this[2]).Low + 1.5*(High - Low);
                            explain = " Close=" + _bars.Close.ToString(Ticker.Format) + ", Value=" +
                                      value.ToString("N2");

                            if (_bars.Close.CompareTo(((Item) this[2]).Low + 1.5*(High - Low)) < 0)
                            {
                                comment = "Buy Impulse.Close;" + explain;
                                price = _bars.Close;
                            }
                            else
                            {
                                comment = "Buy Impulse.Ma;" + explain;
                                price = ((Xma018.Item) (LastItemCompleted)).Ma;
                                price = Ticker.ToMinMove(price, +1);
                            }
                            //  Evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, Name, "xMa018.GetPice", "Bar=" +_bars.Close.ToString("N2") + ", Value=" + value.ToString("N2"),"" );
                            return price;
                        }
                        if (operation < 0)
                        {
                            value = ((Item) this[2]).High - 1.5*(High - Low);
                            explain = " Close=" + _bars.Close.ToString(Ticker.Format) + ", Value=" +
                                      value.ToString("N2");

                            if (_bars.Close.CompareTo(((Item) this[2]).High - 1.5*(High - Low)) > 0)
                            {
                                comment = "Sell Impulse.Close;" + explain;
                                price = _bars.Close;
                            }
                            else
                            {
                                comment = "Sell Impulse.Ma;" + explain;
                                price = ((Xma018.Item) (LastItemCompleted)).Ma;
                                price = Ticker.ToMinMove(price, -1);
                            }
                            //  Evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, Name, "xMa018.GetPice", "Bar=" +_bars.Close.ToString("N2") + ", Value=" + value.ToString("N2"),"" );
                            return price;
                        }
                    }
                    break;
                case 11:
                    if (operation > 0 &&
                        IsImpulse &&
                        Ticker.Ask.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) < 0)
                    {
                        comment = "Buy Impulse.Ma.Bid";
                        return Ticker.Bid;
                    }
                    if (operation < 0 &&
                        IsImpulse &&
                        Ticker.Bid.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) > 0)
                    {
                        comment = "Sell Impulse.Ma.Ask";
                        return Ticker.Ask;
                    }
                    break;
                case 12: // Flat is more Soft
                    if (operation > 0 &&
                        IsFlat &&
                        Ticker.Bid.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) < 0)
                    {
                        comment = "Buy Flat.Ma.Ask";
                        return Ticker.Ask;
                    }
                    if (operation < 0 &&
                        IsFlat &&
                        Ticker.Ask.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) > 0)
                    {
                        comment = "Sell Flat.Ma.Bid";
                        return Ticker.Bid;
                    }
                    break;
                case 21:
                    if (operation > 0 &&
                        IsImpulse &&
                        Ticker.Ask.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) < 0)
                    {
                        comment = "Buy Impulse.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0 &&
                        IsImpulse &&
                        Ticker.Bid.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) > 0)
                    {
                        comment = "Sell Impulse.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 211:
                    if (operation > 0 && IsImpulse)
                    {
                        comment = "Buy Impulse.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0 && IsImpulse)
                    {
                        comment = "Sell Impulse.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 22:
                    if (operation > 0 &&
                        IsFlat &&
                        Ticker.Bid.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) < 0)
                    {
                        comment = "Buy Flat.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0 &&
                        IsFlat &&
                        Ticker.Ask.CompareTo(((Xma018.Item) (LastItemCompleted)).Ma) > 0)
                    {
                        comment = "Sell Flat.Ma";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Ma;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 31:
                    if (operation > 0 && IsImpulse)
                    {
                        comment = "Buy Impulse.Ma.Low";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Low;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0 && IsImpulse)
                    {
                        comment = "Sell Impulse.Ma.High";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).High;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 32:
                    if (operation > 0 && IsFlat)
                    {
                        comment = "Buy Flat.Ma.Low";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).Low;
                        return Ticker.ToMinMove(currentLimitPrice, +1);
                    }
                    if (operation < 0 && IsFlat)
                    {
                        comment = "Sell Flat.Ma.High";
                        currentLimitPrice = ((Xma018.Item) (LastItemCompleted)).High;
                        return Ticker.ToMinMove(currentLimitPrice, -1);
                    }
                    break;
                case 100:
                    if (operation > 0)
                    {
                        comment = "Buy Bid";
                        currentLimitPrice = Ticker.Bid;
                        return currentLimitPrice;
                    }
                    if (operation < 0)
                    {
                        comment = "Sell Ask";
                        currentLimitPrice = Ticker.Ask;
                        return currentLimitPrice;
                    }
                    break;
            }
            return 0.0;
        }

        public double GetPrice2(int operation, uint signalCode, int entryMode, out string comment)
        {
            var currentLimitPrice = 0d;
            comment = string.Empty;

            switch (signalCode)
            {
                case 01:
                    if (operation > 0 && IsUp && !IsDown2)
                    {
                        currentLimitPrice = GetPrice(+1, entryMode, out comment);
                        comment = "Long Entry. Buy Ma Trend " + comment;
                    }
                    else if (operation < 0 && IsDown && !IsUp2)
                    {
                        currentLimitPrice = GetPrice(-1, entryMode, out comment);
                        comment = "Short Entry. Sell Ma Trend " + comment;
                    }
                    break;
                case 02:
                    if (operation > 0 && IsDown && !IsUp2)
                    {
                        currentLimitPrice = GetPrice(+1, entryMode, out comment);
                        comment = "Long Entry. Buy Ma Trend " + comment;
                    }
                    else if (operation < 0 && IsUp && !IsDown2)
                    {
                        currentLimitPrice = GetPrice(-1, entryMode, out comment);
                        comment = "Short Entry. Sell Ma Trend " + comment;
                    }
                    break;
            }
            return currentLimitPrice;
        }

        public double GetPrice3(int operation, int code, out string comment)
        {
            comment = "";
            if (code == 0 || operation == 0) return 0;

            var currentLimitPrice = 0d;
            switch (code)
            {
                case 1:
                    if (operation > 0)
                    {
                        comment = "Buy Bid";
                        currentLimitPrice = Ticker.Bid;
                    }
                    else
                    {
                        comment = "Sell Ask";
                        currentLimitPrice = Ticker.Ask;
                    }
                    break;
                case 2:
                    if (operation > 0)
                    {
                        comment = "Buy Ask";
                        currentLimitPrice = Ticker.Ask;
                    }
                    else
                    {
                        comment = "Sell Bid";
                        currentLimitPrice = Ticker.Bid;
                    }
                    break;
                case 3:
                    comment = operation > 0 ? "Buy Close" : "Sell Close";
                    currentLimitPrice = _bars.Close;
                    break;
                case 5:
                    if (operation > 0)
                    {
                        comment = "Buy Ma";
                        currentLimitPrice = Ticker.ToMinMove(((Item) (LastItemCompleted)).Ma, +1);
                    }
                    else
                    {
                        comment = "Sell Ma";
                        currentLimitPrice = Ticker.ToMinMove(((Item) (LastItemCompleted)).Ma, -1);
                    }
                    break;
                case 8:
                    if (operation > 0 && _bars.IsBlack)
                    {
                        comment = "Buy Black Bid";
                        currentLimitPrice = Ticker.Bid;
                    }
                    else if (operation < 0 && _bars.IsWhite)
                    {
                        comment = "Sell White Ask";
                        currentLimitPrice = Ticker.Ask;
                    }
                    break;
            }
            return currentLimitPrice;
        }

        public bool GetPrice5(int operation, int code, out double price, out string comment)
        {
            comment = "";
            price = 0;
            var boo = false;

            if (code == 0 || operation == 0) return false;

            switch (code)
            {
                case 1:
                    if (operation > 0)
                    {
                        comment = "Buy Bid";
                        price = Ticker.Bid;
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Ask";
                        price = Ticker.Ask;
                        boo = true;
                    }
                    break;
                case 2:
                    if (operation > 0)
                    {
                        comment = "Buy Ask";
                        price = Ticker.Ask;
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Bid";
                        price = Ticker.Bid;
                        boo = true;
                    }
                    break;
                case 3:
                    comment = operation > 0 ? "Buy Close" : "Sell Close";
                    price = _bars.Close;
                    boo = true;
                    break;
                case 5:
                    if (operation > 0)
                    {
                        comment = "Buy Ma";
                        price = Ticker.ToMinMove(((Item) (LastItemCompleted)).Ma, +1);
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Ma";
                        price = Ticker.ToMinMove(((Item) (LastItemCompleted)).Ma, -1);
                        boo = true;
                    }
                    break;
                case 8:
                    if (operation > 0 && _bars.IsBlack)
                    {
                        comment = "Buy Black Bid";
                        price = Ticker.Bid;
                        boo = true;
                    }
                    else if (operation < 0 && _bars.IsWhite)
                    {
                        comment = "Sell White Ask";
                        price = Ticker.Ask;
                        boo = true;
                    }
                    break;
            }
            return boo;
        }

        public bool GetPrice6(int operation, int code, int signalId, out double price, out string comment)
        {
            comment = "";
            price = 0;
            var boo = false;

            if (code == 0 || operation == 0) return false;

            switch (code)
            {
                case 1:
                    if (operation > 0)
                    {
                        comment = "Buy Bid";
                        price = Ticker.Bid;
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Ask";
                        price = Ticker.Ask;
                        boo = true;
                    }
                    break;
                case 2:
                    if (operation > 0)
                    {
                        comment = "Buy Ask";
                        price = Ticker.Ask;
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Bid";
                        price = Ticker.Bid;
                        boo = true;
                    }
                    break;
                case 3:
                    comment = operation > 0 ? "Buy Close" : "Sell Close";
                    price = _bars.Close;
                    boo = true;
                    break;
                case 5:
                    if (operation > 0)
                    {
                        comment = "Buy Ma";
                        price = Ticker.ToMinMove(((Item)(LastItemCompleted)).Ma, +1);
                        boo = true;
                    }
                    else
                    {
                        comment = "Sell Ma";
                        price = Ticker.ToMinMove(((Item)(LastItemCompleted)).Ma, -1);
                        boo = true;
                    }
                    break;
                case 8:
                    switch (signalId)
                    {
                        case 19:
                            if (operation > 0)
                            {
                                if( Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Buy Ma";
                                    price = Ticker.ToMinMove(Ma, -1);
                                    boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Sell Ma";
                                    price = Ticker.ToMinMove(Ma, +1);
                                    boo = true;
                                }
                            }
                            break;
                        case 19005:
                            if (operation > 0)
                            {
                                comment = "Buy Low - Vola";
                                price = Ticker.ToMinMove(Low2 - (High - Low), -1); 
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell High + Vola";
                                price = Ticker.ToMinMove(High2 + (High - Low), + 1); 
                                boo = true;
                            }
                            break;
                        case 25:
                        case 253:
                            if (operation > 0)
                            {
                                if (Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                //else if (_bars.IsBlack && Ticker.Bid.IsLessThan(High))
                                //else if (Ticker.Bid.IsLessThan(High))
                                else
                                {
                                    comment = "Buy Black Bid";
                                    // price = Ticker.Bid;
                                    price = Ticker.ToMinMove(Ma, -1);
                                    boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                // else if (_bars.IsWhite && Ticker.Ask.IsGreaterThan(Low))
                                // else if (Ticker.Ask.IsGreaterThan(Low))
                                else
                                {
                                    comment = "Sell White Ask";
                                    // price = Ticker.Ask;
                                    price = Ticker.ToMinMove(Ma, +1);
                                    boo = true;
                                }
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                if (Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Buy Ma";
                                    price = Ticker.ToMinMove(Ma, -1);
                                    //price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                    //price = Ticker.ToMinMove(High, -1);
                                    boo = true;

                                    //comment = "Buy Ma";
                                    //price = Ticker.ToMinMove((Ma + High)/2d, -1);
                                    //boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Sell Ma";
                                    price = Ticker.ToMinMove(Ma, +1);
                                    // price = Ticker.ToMinMove((Ma + Low)/2.0d, +1);
                                    //price = Ticker.ToMinMove(Low, +1);
                                    boo = true;

                                    //comment = "Sell Ma";
                                    //price = Ticker.ToMinMove((Ma+Low)/2d, +1);
                                    //boo = true;
                                }
                            }
                            break;
                    }
                    break;
                case 10:
                    if (operation > 0)
                    {
                        if (_bars.IsBlack) //&& Ticker.Bid.IsLessThan(High))
                        {
                            comment = "Buy Ask";;
                            price = Ticker.ToMinMove(_bars.LastCompletedClose, +1);
                            boo = true;
                        }
                    }
                    else if (operation < 0)
                    {
                        if (_bars.IsWhite) // && Ticker.Ask.IsGreaterThan(Low))
                        {
                            comment = "Buy Ask";
                            price = Ticker.ToMinMove(_bars.LastItemCompleted.Close, -1);
                            boo = true;
                        }
                    }
                    return boo;
                case 9:
                    switch (signalId)
                    {
                        case 19:
                            if (operation > 0)
                            {
                                if (Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Buy MA";
                                    price = Ticker.ToMinMove(Ma, +1);
                                    boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Sell MA";
                                    price = Ticker.ToMinMove(Ma, -1);
                                    boo = true;
                                }
                            }
                            break;
                        case 19005:
                            if (operation > 0)
                            {
                                comment = "Buy Low - Vola";
                                price = Ticker.ToMinMove(Low2 - (High - Low), -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell High + Vola";
                                price = Ticker.ToMinMove(High2 + (High - Low), +1);
                                boo = true;
                            }
                            break;
                        case 25:
                        case 253:
                            if (operation > 0)
                            {
                                if (Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                //else if (_bars.IsBlack && Ticker.Bid.IsLessThan(High))
                                //else if (Ticker.Bid.IsLessThan(High))
                                else
                                {
                                    comment = "Buy MA";
                                    // price = Ticker.Bid;
                                    price = Ticker.ToMinMove(Ma, +1);
                                    boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                // else if (_bars.IsWhite && Ticker.Ask.IsGreaterThan(Low))
                                // else if (Ticker.Ask.IsGreaterThan(Low))
                                else
                                {
                                    comment = "Sell MA";
                                    // price = Ticker.Ask;
                                    price = Ticker.ToMinMove(Ma, -1);
                                    boo = true;
                                }
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                if (Ticker.Ask.IsLessThan(Ma))
                                {
                                    comment = "Buy Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else if (Ticker.Bid.IsLessThan(Ma))
                                {
                                    comment = "Buy Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Buy MA";
                                    price = Ticker.ToMinMove(Ma, +1);
                                    //price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                    //price = Ticker.ToMinMove(High, -1);
                                    boo = true;

                                    //comment = "Buy Ma";
                                    //price = Ticker.ToMinMove((Ma + High)/2d, -1);
                                    //boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (Ticker.Bid.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Bid";
                                    price = Ticker.Bid;
                                    boo = true;
                                }
                                else if (Ticker.Ask.IsGreaterThan(Ma))
                                {
                                    comment = "Sell Ask";
                                    price = Ticker.Ask;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Sell Ma";
                                    price = Ticker.ToMinMove(Ma, -1);
                                    // price = Ticker.ToMinMove((Ma + Low)/2.0d, +1);
                                    //price = Ticker.ToMinMove(Low, +1);
                                    boo = true;

                                    //comment = "Sell Ma";
                                    //price = Ticker.ToMinMove((Ma+Low)/2d, +1);
                                    //boo = true;
                                }
                            }
                            break;
                    }
                    break;
                case 91: 
                    switch (signalId)
                    {
                        case 212:
                        case 214:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, +1);
                                //price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                //price = Ticker.ToMinMove(High, -1);
                                boo = true;
                                }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, -1);
                                // price = Ticker.ToMinMove((Ma + Low)/2.0d, +1);
                                //price = Ticker.ToMinMove(Low, +1);
                                boo = true;
                            }
                            break;
                        // Flat + Impulse
                        case 2120:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = IsFlat
                                    ? Ticker.ToMinMove(Ma, +1)
                                    : Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = IsFlat
                                    ? Ticker.ToMinMove(Ma, -1)
                                    : Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                boo = true;
                            }
                            break;
                        // Flat + Impulse
                        case 2123:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = IsImpulse
                                    ? Ticker.ToMinMove(Ma, +1)
                                    : Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = IsFlat
                                    ? Ticker.ToMinMove(Ma, -1)
                                    : Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                boo = true;
                            }
                            break;
                        // ATR
                        // Flat + Impulse
                        case 2124:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = ((Atr2)Atr).IsSlowHigher
                                    ? Ticker.ToMinMove(Ma, +1)
                                    : Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = ((Atr2)Atr).IsSlowHigher
                                    ? Ticker.ToMinMove(Ma, -1)
                                    : Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                boo = true;
                            }
                            break;
                        // ATR
                        // Flat + Impulse
                        case 2125:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = ((Atr2)Atr).IsFastHigher
                                    ? Ticker.ToMinMove(Ma, +1)
                                    : Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = ((Atr2)Atr).IsFastHigher
                                    ? Ticker.ToMinMove(Ma, -1)
                                    : Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                boo = true;
                            }
                            break;

                        // Impulse
                        case 2121:
                            if (operation > 0)
                            {
                                comment = "Buy (MA+High)/2";
                                price = Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell (Ma+Low)/2";
                                price = Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                boo = true;
                            }
                            break;
                        // Flat
                        case 2122:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            break;
                        case 221:
                            if (operation > 0)
                            {
                                comment = "Buy (MA+Low)/2";
                                price = Ticker.ToMinMove( (Ma + Low)/2.0d, +1);
                                //price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                //price = Ticker.ToMinMove(High, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell (Ma+Hihg)/2";
                                price = Ticker.ToMinMove( (Ma + High)/2.0d, -1);
                                // price = Ticker.ToMinMove((Ma + Low)/2.0d, +1);
                                //price = Ticker.ToMinMove(Low, +1);
                                boo = true;
                            }
                            break;
                        case 19:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            break;
                    }
                    break;
                // 11.04.2018
                // More Strong than 91/ More Less for Buy and More High for Sell
                case 92: 
                    switch (signalId)
                    {
                        case 212:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                        // Flat + Impulse
                        case 2120:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = IsFlat 
                                    ? Ticker.ToMinMove(Ma, -1)
                                    : Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = IsFlat 
                                    ? Ticker.ToMinMove(Ma, +1)
                                    : Ticker.ToMinMove((Ma + Low) / 2.0d, +1);
                                boo = true;
                            }
                            break;

                        // Impulse
                        case 2121:
                            if (operation > 0)
                            {
                                comment = "Buy (MA+High)/2";
                                price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell (Ma+Low)/2";
                                price = Ticker.ToMinMove((Ma + Low) / 2.0d, +1);
                                boo = true;
                            }
                            break;
                        // Flat
                        case 2122:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;

                        case 221:
                            if (operation > 0)
                            {
                                comment = "Buy (MA+Low)/2";
                                price = Ticker.ToMinMove((Ma + Low) / 2.0d, -1);
                                //price = Ticker.ToMinMove((Ma + High) / 2.0d, -1);
                                //price = Ticker.ToMinMove(High, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell (Ma+High)/2";
                                price = Ticker.ToMinMove((Ma + High) / 2.0d, +1);
                                // price = Ticker.ToMinMove((Ma + Low)/2.0d, +1);
                                //price = Ticker.ToMinMove(Low, +1);
                                boo = true;
                            }
                            break;
                        case 19:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                    }
                    break;
                case 95:
                    switch (signalId)
                    {
                        case 212:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            break;
                        case 19:
                        case 519:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                    }
                    break;
                case 98:
                    switch (signalId)
                    {
                        case 212:
                            if (operation > 0)
                            {
                                if (_bars.IsBlack)
                                {
                                    comment = "Buy Close in Black Candle";
                                    price = _bars.Close;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Buy MA";
                                    price = Ticker.ToMinMove(Ma, +1);
                                    boo = true;
                                }
                            }
                            else if (operation < 0)
                            {
                                if (_bars.IsWhite)
                                {
                                    comment = "Buy Close in White Candle";
                                    price = _bars.Close;
                                    boo = true;
                                }
                                else
                                {
                                    comment = "Sell Ma";
                                    price = Ticker.ToMinMove(Ma, -1);
                                    boo = true;
                                }
                            }
                            break;
                        case 19:
                        case 519:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                        default:
                            if (operation > 0)
                            {
                                comment = "Buy MA";
                                price = Ticker.ToMinMove(Ma, -1);
                                boo = true;
                            }
                            else if (operation < 0)
                            {
                                comment = "Sell Ma";
                                price = Ticker.ToMinMove(Ma, +1);
                                boo = true;
                            }
                            break;
                    }
                    break;
            }
            return boo;
        }
        private void SetTrend2(int ilast)
        {
            var last = ((Item) Items[ilast]);

            var temp2 = _trend2;
            var temp3 = _trend3;
            // Trend2
            if (last.Trend > 0)
            {
                if (last.Ma.CompareTo(last.High2) > 0) _trend2 = +1;
                else if (_trend2 > 0) _trend2 = 0;
            }
            else if (last.Trend < 0)
            {
                if (last.Ma.CompareTo(last.Low2) < 0) _trend2 = -1;
                else if (_trend2 < 0) _trend2 = 0;
            }
            // Trend 3
            if (last.Trend > 0)
            {
                if (last.Ma.CompareTo(last.High3) > 0) _trend3 = +1;
                else if (_trend3 > 0) _trend3 = 0;
            }
            else if (last.Trend < 0)
            {
                if (last.Ma.CompareTo(last.Low3) < 0) _trend3 = -1;
                else if (_trend3 < 0) _trend3 = 0;
            }

            //if (temp2 != _trend2) _prevTrend2 = temp2;
            //if (temp3 != _trend3) _prevTrend3 = temp3;

            //_trend5 = _trend2 > 0 ? +1 : ( _trend2 < 0 ? -1 : _trend5);
            //if (_trend2 > 0)
            //    _trend5 = +1;
            //else if (_trend2 < 0)
            //    _trend5 = -1;

            if(_trend2 != 0)
                _trend5 = _trend2;          
        }

        public string Trend2ToString()
        {
            return $"Trend2={(_trend2 > 0 ? "Up" : _trend2 < 0 ? "Down" : "Neutral")}";
        }

        // ***************** Signals ***********************
        public bool IsShortEntry(int signalIndex)
        {
            switch (signalIndex)
            {
                case 0x20:
                    if (IsUp2 && IsDown && Ma.CompareTo(_prevTrendLastLow) < 0) return true;
                    break;
            }
            return false;
        }

        public bool IsLongEntry(int signalIndex)
        {
            switch (signalIndex)
            {
                case 0x20:
                    if (IsDown2 && IsUp && Ma.CompareTo(_prevTrendLastHigh) > 0) return true;
                    break;
            }
            return false;
        }

        private float _signal2505Atrs = 5f;
        private float _signal2506Atrs = 6f;
        private float _signal2575Atrs = 7.5f;
        private float _signal2508Atrs = 8f;
        private float _signal2510Atrs = 10f;
        public bool TakeLong(int index)
        {
            //price = 0d;
            if (index == 0) return false;

            switch (index)
            {
                case 1:
                    return true;
                case 1010:
                    return IsDown;
                case 1011:
                    return IsDown && IsFlat;
                case 101:
                    return IsDown2 && IsUp;
                case 2:
                    return IsDown2 && IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);

                case 212:
                case 2124:
                case 2125:
                     return             IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);
                // 11.04.2018
                case 2120:
                case 2123:
                    return              IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);
                case 2121:
                    return IsImpulse && IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);
                case 2122:
                    return IsFlat &&    IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);

                case 221:
                    return IsFlat && IsDown && Ma.IsGreaterThan(_prevTrendLastLow);

                case 223:
                    return IsDown && Ma.IsLessThan(_prevTrendLastLow);

                case 225:
                    return IsFlat && IsDown && Ma.IsLessThan(_prevTrendLastLow);

                //case 223:
                //    return IsFlat && IsDown && Ma.IsGreaterThan(_prevTrendLastLow);

                case 213:
                    return IsUp && Low.IsGreaterThan(_prevTrendLastHigh);
                case 214:
                case 2141:
                    return IsDown && Ma.IsGreaterThan(_prevTrendLastLow);
                case 2142:
                    return IsDown && IsFlat && Ma.IsGreaterThan(_prevTrendLastLow);
                case 215:
                    return IsUp && Ma.IsGreaterThan(_prevTrendLastHigh) && IsAtrValueEnough;
                case 216:
                    return IsUp && Ma.IsGreaterThan(_prevTrendLastHigh) && ! IsAtrValueEnough;
                case 217:
                    return IsUp && Ma.IsGreaterThan(_prevTrendLastHigh) && IsTrend09Up;
                //2018.02.17
                // bit Like 212 
                case 218:
                    return IsUp && Ma.IsGreaterThan(Trend2PrevHigh);
                case 3:
                    if (IsDown2 && IsUp && High.IsLessThan(_prevTrendLastLow2)) return true;
                    break;
                case 4:
                    if (IsDown2 && IsUp &&
                        Ma.IsGreaterThan(_prevTrendLastHigh) && High.IsLessThan(_prevTrendLastLow2)) return true;
                    break;
                // 5
                case 51:
                    return IsUp2 && IsDown;

                case 511:
                    return IsUp2 && IsDown && Ma.IsLessThan(_prevTrendLastLow);

                case 512:
                    return IsUp2 && IsDown && High.IsLessThan(_prevTrendLastLow);

                case 53:
                    return (IsUp && IsUp2) || (IsFlat2 && IsPrevUp2 && IsUp);

                case 105:
                    if (IsDown2 && IsUp && High.IsLessThan(Low3))
                        return true;
                    break;
                case 106:
                    return IsDown2 && IsUp && High.IsLessThan(Low3) && Ma.IsGreaterThan(_prevTrendLastHigh);                  
                case 107:
                    return IsUp2 && IsDown && Low.IsLessThan(High3);
                case 5:
                    if (IsFlat2 && IsPrevUp2 && IsUp) return true;
                    break;
                case 6:
                    if (IsFlat2 && IsPrevUp2 && IsUp && Ma.CompareTo(_prevTrendLastHigh) > 0) return true;
                    break;
                case 61:
                    if (IsFlat2 && IsPrevUp2 && IsDown) return true;
                    break;
                case 62:
                    if ((IsFlat2 && IsPrevUp2 && IsUp && Ma.CompareTo(_prevTrendLastHigh) > 0) ||
                        (IsFlat2 && IsPrevUp2 && IsDown))
                        return true;
                    break;
                case 63:
                    if (IsFlat2 && IsPrevUp2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0) return true;
                    break;
                case 7:
                    if (IsFlat2 && IsPrevUp2 && IsUp && High.CompareTo(_prevTrendLastLow2) < 0) return true;
                    break;
                case 8:
                    return IsFlat2 && _trend5 > 0 && IsUp &&
                           Ma.IsGreaterThan(_prevTrendLastHigh) && High.IsLessThan(_prevTrendLastLow2);
                case 81:
                    return // IsFlat2 && _trend5 > 0 &&
                        IsUp && Ma.IsGreaterThan(_prevTrendLastHigh) && High.IsLessThan(_prevTrendLastLow2);
                case 82:
                    return IsFlat2 && _trend5 < 0 && IsDown && Low.IsLessThan(_prevTrendLastHigh2);
                case 83:
                    return SwingCount <= 2 && IsFlat2 && _trend5 < 0 && IsDown && Low.IsLessThan(_prevTrendLastHigh2);
                case 83106:
                    return (SwingCount <= 2 && IsFlat2 && _trend5 < 0 && IsDown && Low.IsLessThan(_prevTrendLastHigh2)) || // 83
                           (IsDown2 && IsUp && High.IsLessThan(Low3) && Ma.IsGreaterThan(_prevTrendLastHigh)); // 106
                case 9:
                    if (IsFlat2 && IsPrevDown2 && IsUp) return true;
                    break;
                case 10:
                    if (IsFlat2 && IsPrevDown2 && IsUp && Ma.CompareTo(_prevTrendLastHigh) > 0) return true;
                    break;
                case 11:
                    if (IsFlat2 && IsPrevDown2 && IsUp && High.CompareTo(_prevTrendLastLow2) < 0) return true;
                    break;
                case 12:
                    if (IsFlat2 && IsPrevDown2 && IsUp &&
                        Ma.CompareTo(_prevTrendLastHigh) > 0 && High.CompareTo(_prevTrendLastLow2) < 0) return true;
                    break;
                case 13:
                    if (!IsUp2 && IsUp) return true;
                    break;
                case 14:
                    if (!IsUp2 && IsUp && Ma.CompareTo(_prevTrendLastHigh) > 0) return true;
                    break;
                case 15:
                    if (!IsUp2 && IsUp && High.CompareTo(_prevTrendLastLow2) < 0) return true;
                    break;
                case 16:
                    if (!IsUp2 && IsUp &&
                        Ma.CompareTo(_prevTrendLastHigh) > 0 && High.CompareTo(_prevTrendLastLow2) < 0) return true;
                    break;
                case 17:
                    if (IsUp2 && IsUp) return true;
                    break;
                case 18:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0) return true;
                    break;
                case 18001:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0)
                        return true;
                    break;
                case 18002:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0)
                        return true;
                    break;
                case 18003:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastLow2) < 0)
                        return true;
                    break;
                case 18004:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0 && !Glue)
                            return true;
                    }
                    break;

                case 18005:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0)
                            return true;
                    }
                    break;
                case 18006:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastLow2) < 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0)
                            return true;
                    }
                    break;
                case 18007:
                    if (IsUp2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0)
                        return true;
                    break;
                case 18008:
                    if (IsUp2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0)
                        return true;
                    break;
                case 18009:
                    if (IsUp2 && IsDown && Low.CompareTo(Low2) < 0)
                        return true;
                    break;

                case 18101:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0 && Ma.CompareTo(Low3) >= 0)
                        return true;
                    break;
                case 18102:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0 && Ma.CompareTo(Low3) >= 0)
                        return true;
                    break;
                case 18103:
                    if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastLow2) < 0 && Ma.CompareTo(Low3) >= 0)
                        return true;
                    break;
                case 18105:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0 && Ma.CompareTo(Low3) >= 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0 && Ma.CompareTo(Low3) >= 0)
                            return true;
                    }
                    break;
                case 18106:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastLow2) < 0 && Ma.CompareTo(Low3) >= 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsDown && Low.CompareTo(_prevTrendLastMa2) < 0 && Ma.CompareTo(Low3) >= 0)
                            return true;
                    }
                    break;
                case 181:
                    if (
                        (IsUp2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) < 0) || // 18007
                        (IsFlat2 && IsPrevUp2 && IsDown) // 61
                        )
                        return true;
                    break;

                case 190:
                    return IsDown && IsDown2;

                case 19:
                    return 
                        (IsDown && IsDown2 && IsFlat && Ma.IsLessThan(_prevTrendLastLow2))
                   // ||  (IsDown2 && IsUp && Ma.IsLessOrEqualsThan(_prevTrendLastHigh))
                   // ||  (IsDown2 && IsDown && Ma.IsLessOrEqualsThan(_prevTrendLastHigh)) 
                        ;
                case 519:
                    return 
                        IsDown && IsDown2 && IsFlat && Ma.IsLessThan(_prevTrendLastLow2) &&
                        Ma.IsLessThan(TrLow1);

                case 191:
                    return IsDown && IsDown2 && Ma.IsLessThan(_prevTrendLastLow2);

                case 1953:
                    return IsDown2 && IsDown
                        && High.IsLessThan(_prevTrendLastHigh - 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));

                case 19002:
                    if (IsDown && IsDown2 && IsFlat && Ma.CompareTo(_prevTrendLastLow2) < 0
                        && FlatCount >= 2)
                        return true;
                    break;
                case 19003:
                    if (IsDown && IsDown2 && IsFlat && Ma.CompareTo(_prevTrendLastLow2) < 0
                        && FlatCount >= 3)
                        return true;
                    break;

                //case 19005:
                //    return IsFlat2 && IsDown && Low.IsLessThan(_prevTrendLastMa2) && Low.IsGreaterThan(Low2);   
                case 19005:
                    return (IsFlat2 || IsUp2) && IsDown && Ma.IsLessThan(Low2 + (High - Low)) && Ma.IsGreaterThan(Low2);
                //case 19006:
                //    if (IsDown && IsDown2 && IsFlat && Ma.CompareTo(_prevTrendLastLow2) < 0)
                //        return true;
                //    break;
                case 1931:
                    return IsDown && IsDown2 && IsFlat & Ma.IsLessThan(Low2)
                            && Ma.IsLessThan(TrLow1);

                case 195:
                    return
                        IsDown && IsDown2 && IsFlat && Ma.IsLessThan(_prevTrendLastLow2) && IsTrend55LowBreakDown;
                case 21:
                    if (IsDown && IsDown2 && IsFlat && Ma.CompareTo(_prevTrendLastLow2) < 0
                        && Ma.CompareTo(Low3) >= 0)
                        return true;
                    break;
                case 22:
                    if (IsDown && IsDown2 && IsFlat && Ma.CompareTo(_prevTrendLastLow2) < 0
                        && Ma.CompareTo(Low3) < 0)
                        return true;
                    break;
                case 23:
                    if (IsDown && IsDown2 && IsFlat && !IsPrevUp2)
                        return true;
                    break;
                case 24:
                    if (IsDown && IsDown2 && IsFlat && IsPrevUp2)
                        return true;
                    break;
                case 25:
                    return IsUp2 && IsUp;
                
                case 2510:
                    return IsUp2 && IsUp &&  (High - Low2) < _signal2510Atrs * VolatilityUnit;
                case 2505:
                    return IsUp2 && (Ma - Low2) < _signal2505Atrs * VolatilityUnit;
                case 2506:
                    return IsUp2 && (Ma - Low2) < _signal2506Atrs * VolatilityUnit;
                case 2575:
                    return IsUp2 && (Ma - Low2) < _signal2575Atrs * VolatilityUnit;
                case 2508:
                    return IsUp2 && (Ma - Low2) < _signal2508Atrs * VolatilityUnit;
                case 25010:
                    return IsUp2 && (Ma - Low2) < _signal2510Atrs * VolatilityUnit;
                case 251:
                    if ((IsUp && IsUp2))
                        return true;
                    break;
                case 252:
                    if ((IsDown && IsUp2))
                        return true;
                    break;
                case 250:
                    return IsUp2;
                case 253:
                    return IsUp2 && IsUp
                        // && Low.IsLessOrEqualsThan(_prevTrendLastLow + 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));
                        && Low.IsLessThan(_prevTrendLastLow + 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));
                case 25332:
                    return IsUp2 && IsUp && High.IsLessThan(TrHigh1)
                        && Low.IsLessThan(_prevTrendLastLow + 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));
                case 254:
                    return IsUp2 && IsUp && Low.IsLessOrEqualsThan(High2);
                case 255:
                    return IsUp2 && IsDown && Low.IsLessOrEqualsThan(High3);
                case 256:
                    return (IsUp2 && IsUp && Low.IsLessOrEqualsThan(High2)) ||
                           (IsUp2 && IsDown && Low.IsLessOrEqualsThan(High3));
                case 257:
                    return IsUp2 && IsUp && Low.IsLessOrEqualsThan(_prevTrendLastHigh);
                case 258:
                    return IsUp2 && IsDown && Low.IsLessOrEqualsThan(_prevTrendLastHigh2);
                case 259:
                    return (IsUp2 && IsUp && Low.IsLessOrEqualsThan(_prevTrendLastHigh)) ||
                           (IsUp2 && IsDown && Low.IsLessOrEqualsThan(_prevTrendLastHigh2));
                //case 2510:
                //        return IsUp && IsUp2 && High.IsLessThan(TrHigh1);
                case 2512:
                        return IsUp && IsUp2 && High.IsLessThan(TrHigh12);
                case 260:
                    return (IsUp && IsUp2 && High3.IsGreaterThan(High2) && High3.IsGreaterThan(High)); 
                case 2531:
                    return IsUp2 && IsUp && Ma.IsLessThan(TrHigh1);
                case 2532:
                    return IsUp2 && IsUp && High.IsLessThan(TrHigh1);
                case 2533:
                    return IsUp2 && IsUp && FirstSwingAfterBrDown;
                case 27:
                    return IsUp2 && IsDown;
                case 28:
                    return IsUp5 && IsDown;
                
                default:
                    return false;
            }
            return false;
        }
        public bool TakeShort(int index)
        {
            //price = 0d;
            if (index == 0) return false;

            switch (index)
            {
                case 1:
                    return true;
                case 1010:
                    return IsUp;
                case 1011:
                    return IsUp && IsFlat;
                case 101:
                    return IsUp2 && IsDown;
                case 2:
                    return IsUp2 && IsDown && Ma.IsLessThan(_prevTrendLastLow);
                // 16.11.09

                case 212:
                case 2124:
                case 2125:
                    return              IsDown && Ma.IsLessThan(_prevTrendLastLow);
                // 11.04.2018
                // See GetPRice6 91,92 EntryId 
                case 2120:
                case 2123:
                    return              IsDown && Ma.IsLessThan(_prevTrendLastLow);
                case 2121:
                    return IsImpulse && IsDown && Ma.IsLessThan(_prevTrendLastLow);
                case 2122:
                    return IsFlat &&    IsDown && Ma.IsLessThan(_prevTrendLastLow);
                
                    // 11.04.2018
                case 221:
                    return IsFlat && IsUp && Ma.IsLessThan(_prevTrendLastHigh);

                // 17.08.2018
                case 223:
                    return IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);

                case 225:
                    return IsFlat && IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);

                case 213:
                    return IsDown && High.IsLessThan(_prevTrendLastLow);
                case 214:
                case 2141:
                    return IsUp && Ma.IsLessThan(_prevTrendLastHigh);
                case 2142:
                    return IsUp && IsFlat && Ma.IsLessThan(_prevTrendLastHigh);
                case 215:
                    return IsDown && Ma.IsLessThan(_prevTrendLastLow) && IsAtrValueEnough;
                case 216:
                    return IsDown && Ma.IsLessThan(_prevTrendLastLow) && ! IsAtrValueEnough;
                case 217:
                    return IsDown && Ma.IsLessThan(_prevTrendLastLow) && IsTrend09Down;
                case 3:
                    if (IsUp2 && IsDown && Low.IsGreaterThan(_prevTrendLastHigh2))
                        return true;
                    break;
                case 4:
                    if (IsUp2 && IsDown &&
                        Ma.IsLessThan(_prevTrendLastLow) && Low.IsGreaterThan(_prevTrendLastHigh2))
                        return true;
                    break;

                case 51:
                    return IsDown2 && IsUp;

                case 511:
                    return IsDown2 && IsUp && Ma.IsGreaterThan(_prevTrendLastHigh);

                case 512:
                    return IsDown2 && IsUp && Low.IsGreaterThan(_prevTrendLastHigh);  
                                    
                case 53:
                    return (IsDown && IsDown2) || (IsFlat2 && IsPrevDown2 && IsDown);
                   
                case 105:
                    if (IsUp2 && IsDown && Low.CompareTo(High3) > 0)
                        return true;
                    break;
                case 106:
                    return IsUp2 && IsDown && Low.IsGreaterThan(High3) && Ma.IsLessThan(_prevTrendLastLow);                  
                case 107:
                    return (IsDown2 && IsUp && High.IsGreaterThan(Low3));
                case 5:
                    if (IsFlat2 && IsPrevDown2 && IsDown) return true;
                    break;
                case 6:
                    if (IsFlat2 && IsPrevDown2 && IsDown && Ma.CompareTo(_prevTrendLastLow) < 0)
                        return true;
                    break;
                case 61:
                    if (IsFlat2 && IsPrevDown2 && IsUp)
                        return true;
                    break;
                case 62:
                    if ((IsFlat2 && IsPrevDown2 && IsDown && Ma.CompareTo(_prevTrendLastLow) < 0) ||
                        (IsFlat2 && IsPrevDown2 && IsUp)
                        )
                        return true;
                    break;
                case 63:
                    if (IsFlat2 && IsPrevDown2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0)
                        return true;
                    break;
                case 7:
                    if (IsFlat2 && IsPrevDown2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) > 0) return true;
                    break;
                case 8:
                    return IsFlat2 && _trend5 < 0 && IsDown &&
                        Ma.IsLessThan(_prevTrendLastLow) && Low.IsGreaterThan(_prevTrendLastHigh2);
                case 81:
                    return // IsFlat2 && _trend5 < 0 &&
                        IsDown && Ma.IsLessThan(_prevTrendLastLow) && Low.IsGreaterThan(_prevTrendLastHigh2);
                case 82:
                    return IsFlat2 && _trend5 > 0 && IsUp && High.IsGreaterThan(_prevTrendLastLow2);
                case 83:
                    return SwingCount <= 2 && IsFlat2 && _trend5 > 0 && IsUp && High.IsGreaterThan(_prevTrendLastLow2);
                case 83106:
                    return (SwingCount <= 2 && IsFlat2 && _trend5 > 0 && IsUp && High.IsGreaterThan(_prevTrendLastLow2)) ||  // 82
                           (IsUp2 && IsDown && Low.IsGreaterThan(High3) && Ma.IsLessThan(_prevTrendLastLow)); // 106
                case 9:
                    if (IsFlat2 && IsPrevUp2 && IsDown) return true;
                    break;
                case 10:
                    if (IsFlat2 && IsPrevUp2 && IsDown && Ma.CompareTo(_prevTrendLastLow) < 0) return true;
                    break;
                case 11:
                    if (IsFlat2 && IsPrevUp2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) > 0) return true;
                    break;
                case 12:
                    if (IsFlat2 && IsPrevUp2 && IsDown &&
                        Ma.CompareTo(_prevTrendLastLow) < 0 && Low.CompareTo(_prevTrendLastHigh2) > 0) return true;
                    break;
                case 13:
                    if (!IsDown2 && IsDown) return true;
                    break;
                case 14:
                    if (!IsDown2 && IsDown && Ma.CompareTo(_prevTrendLastLow) < 0) return true;
                    break;
                case 15:
                    if (!IsDown2 && IsDown && Low.CompareTo(_prevTrendLastHigh2) > 0) return true;
                    break;
                case 16:
                    if (!IsDown2 && IsDown &&
                        Ma.CompareTo(_prevTrendLastLow) < 0 && Low.CompareTo(_prevTrendLastHigh2) > 0) return true;
                    break;
                case 17:
                    if (IsDown2 && IsDown) return true;
                    break;
                case 18:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0) return true;
                    break;
                case 18001:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0)
                        return true;
                    break;
                case 18002:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0)
                        return true;
                    break;
                case 18003:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastHigh2) > 0)
                        return true;
                    break;
                case 18004:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0 && !Glue)
                            return true;
                    }
                    break;
                case 18005:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0)
                            return true;
                    }
                    break;
                case 18006:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastHigh2) > 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0)
                            return true;
                    }
                    break;
                case 18007:
                    if (IsDown2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0)
                        return true;
                    break;
                case 18008:
                    if (IsDown2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0)
                        return true;
                    break;
                case 18009:
                    if (IsDown2 && IsUp && High.CompareTo(High2) > 0)
                        return true;
                    break;
                case 18101:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0 && Ma.CompareTo(High3) <= 0)
                        return true;
                    break;
                case 18102:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0 && Ma.CompareTo(High3) <= 0)
                        return true;
                    break;
                case 18103:
                    if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastHigh2) > 0 && Ma.CompareTo(High3) > 0)
                        return true;
                    break;
                case 18105:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0 && Ma.CompareTo(High3) <= 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0 && Ma.CompareTo(High3) <= 0)
                            return true;
                    }
                    break;
                case 18106:
                    if (PrevGlue)
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastHigh2) > 0 && Ma.CompareTo(High3) > 0)
                            return true;
                    }
                    else
                    {
                        if (IsFlat2 && IsUp && High.CompareTo(_prevTrendLastMa2) > 0 && Ma.CompareTo(High3) <= 0)
                            return true;
                    }
                    break;
                case 181:
                    if (
                        (IsDown2 && IsUp && High.CompareTo(_prevTrendLastLow2) > 0) || // 18007
                        (IsFlat2 && IsPrevDown2 && IsUp) // 61
                        )
                        return true;
                    break;
// 19
                case 190:
                    return IsUp && IsUp2;

                case 19:
                    return 
                        (IsUp && IsUp2 && IsFlat && Ma.IsGreaterThan(_prevTrendLastHigh2))
                   // ||  (IsUp2 && IsDown && Ma.IsGreaterOrEqualsThan(_prevTrendLastLow))
                    ;
                case 519:
                    return
                        IsUp && IsUp2 && IsFlat && Ma.IsGreaterThan(_prevTrendLastHigh2) &&
                        Ma.IsGreaterThan(TrHigh1)
                    ;

                case 191:
                    return IsUp && IsUp2 && Ma.IsGreaterThan(_prevTrendLastHigh2);
                case 1953:
                    return IsUp2 && IsUp
                        && Low.IsGreaterThan(_prevTrendLastLow + 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));

                case 19002:
                    if (IsUp && IsUp2 && IsFlat && Ma.CompareTo(_prevTrendLastHigh2) > 0
                        && FlatCount >= 2)
                        return true;
                    break;
                case 19003:
                    if (IsUp && IsUp2 && IsFlat && Ma.CompareTo(_prevTrendLastHigh2) > 0
                        && FlatCount >= 3)
                        return true;
                    break;
                case 19005:
                    return (IsFlat2 || IsDown2) && IsUp && Ma.IsGreaterThan(High2 - (High - Low)) && Ma.IsLessThan(High2);
                case 1931:
                    return IsUp && IsUp2 && IsFlat && Ma.IsGreaterThan(High2)
                            && Ma.IsGreaterThan(TrHigh1);
                case 195:
                    return IsUp && IsUp2 && IsFlat && Ma.IsGreaterThan(_prevTrendLastHigh2) && IsTrend55HighBreakUp;
                case 21:
                    if (IsUp && IsUp2 && IsFlat && Ma.CompareTo(_prevTrendLastHigh2) > 0
                        && Ma.CompareTo(High3) <= 0)
                        return true;
                    break;
                case 22:
                    if (IsUp && IsUp2 && IsFlat && Ma.CompareTo(_prevTrendLastHigh2) > 0
                        && Ma.CompareTo(High3) > 0)
                        return true;
                    break;
                case 23:
                    if (IsUp && IsUp2 && IsFlat && !IsPrevDown2)
                        return true;
                    break;
                case 24:
                    if (IsUp && IsUp2 && IsFlat && IsPrevDown2)
                        return true;
                    break;
                case 25:
                    return IsDown2 && IsDown;

                case 2510:
                    return IsDown2 && IsDown && (High2 - Low) < _signal2510Atrs * VolatilityUnit;
                case 2505:
                    return IsDown2 && (High2 - Ma) < _signal2505Atrs * VolatilityUnit;
                case 2506:
                    return IsDown2 && (High2 - Ma) < _signal2506Atrs * VolatilityUnit;
                case 2575:
                    return IsDown2 && (High2 - Ma) < _signal2575Atrs * VolatilityUnit;
                case 2508:
                    return IsDown2 && (High2 - Ma) < _signal2508Atrs * VolatilityUnit;
                case 25010:
                    return IsDown2 && (High2 - Ma) < _signal2510Atrs * VolatilityUnit;
                case 253:
                    return IsDown2 && IsDown 
                        // && High.IsGreaterOrEqualsThan(_prevTrendLastHigh - 1.5d*(_prevTrendLastHigh - _prevTrendLastLow));
                        && High.IsGreaterThan(_prevTrendLastHigh - 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));
                case 25332:
                    return IsDown2 && IsDown && Low.IsGreaterThan(TrLow1)
                        && High.IsGreaterThan(_prevTrendLastHigh - 1.5d * (_prevTrendLastHigh - _prevTrendLastLow));
                case 251:
                    if ((IsDown && IsDown2))
                        return true;
                    break;
                case 252:
                    if ((IsUp && IsDown2))
                        return true;
                    break;
                case 250:
                    return IsDown2;
                case 254:
                    return IsDown2 && IsDown && High.IsGreaterOrEqualsThan(Low2);
                case 255:
                    return IsDown2 && IsUp && High.IsGreaterOrEqualsThan(Low3);
                case 256:
                    return (IsDown2 && IsDown && High.IsGreaterOrEqualsThan(Low2)) ||
                           (IsDown2 && IsUp && High.IsGreaterOrEqualsThan(Low3));
                case 257:
                    return IsDown2 && IsDown && High.IsGreaterOrEqualsThan(_prevTrendLastLow);
                case 258:
                    return IsDown2 && IsUp && High.IsGreaterOrEqualsThan(_prevTrendLastLow2);
                case 259:
                    return (IsDown2 && IsDown && High.IsGreaterOrEqualsThan(_prevTrendLastLow)) ||
                           (IsDown2 && IsUp && High.IsGreaterOrEqualsThan(_prevTrendLastLow2));
                //case 2510:
                //    return IsDown && IsDown2 && Low.IsGreaterThan(TrLow1);
                case 2512:
                    return IsDown && IsDown2 && Low.IsGreaterThan(TrLow12);
                case 260:
                    return (IsDown2 && IsDown && Low3.IsLessThan(Low2) && Low3.IsLessThan(Low));
                case 2531:
                    return IsDown && IsDown2 && Ma.IsGreaterThan(TrLow1);
                case 2532:
                    return IsDown2 && IsDown && Low.IsGreaterThan(TrLow1);
                case 2533:
                    return IsDown2 && IsDown && FirstSwingAfterBrUp;
                case 27:
                    return IsDown2 && IsUp;
                case 28:
                    return IsDown5 && IsUp;

                default:
                    return false;
            }
            return false;
        }
    }
}