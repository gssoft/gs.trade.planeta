using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.Trades.UI.Model
{
    public class TradeNpc : TradeEntity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, args);
        }

        public TradeNpc()
        {
        }

        public TradeNpc(ITrade3 t)
        {
            Strategy = t.Strategy;

            DT = t.DT;
            Number = t.Number;

            Operation = t.Operation;
            Status = t.Status;

            Quantity = t.Quantity;

            Price1 = t.Price;

        }

        public void Update(ITrade3 t)
        {
            Operation = t.Operation;
            Status = t.Status;

            Quantity = t.Quantity;
            Price1 = t.Price;
        }

        public string Key { get { return (Number + "." + AccountCode).TrimUpper(); } }
        

        private decimal _price1;
        public decimal Price1
        {
            get { return _price1; }
            set
            {
                _price1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Price1String"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
            }
        }

        public float Price2 {
            get { return (float)Price1 * Rate; }
        }

        private float _rate;
        public float Rate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RateString"));
                OnPropertyChanged(new PropertyChangedEventArgs("Price2String"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
            }
        }
        TradeOperationEnum _operation;
        public TradeOperationEnum Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OperationString"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
            }
        }

        private long _quantity;
        public long Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Quantity"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
            }
        }

        public decimal Amount { get { return Quantity * Price1; } }


        private TradeStatusEnum _status;
        public TradeStatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StatusString"));
                OnPropertyChanged(new PropertyChangedEventArgs("StrategyCode"));
            }
        }

        public long Position { get { return Quantity * (short)Operation; } }

        public string OperationString { get { return Operation.ToString().ToUpper(); } }
        public string StrategyTickerString { get { return StrategyCode + "." + TickerCode; } }

        public string StatusString { get { return Status.ToString().ToUpper(); } }
        public string Comment { get; set; }
        public long Index { get; set; }

        public string TradeKey { get { return StrategyKey + "@" + AccountKey + "@" + TickerKey; } }

        public string TickerFormat { get { return Ticker != null ? Ticker.Format : "N2"; } }
        public string TickerFormatAvg { get { return Ticker != null ? Ticker.FormatAvg : "N2"; } }
        public string TickerFormatM { get { return Ticker != null ? Ticker.FormatM : "N2"; } }     

        public string AmountString { get { return Amount.ToString(TickerFormatAvg); } }


        public string Price1String { get { return Price1.ToString(TickerFormatAvg); } }
        public string Price2String { get { return Price2.ToString(TickerFormatAvg); } }

        public string LastTradePriceString { get { return Price2.ToString(TickerFormat); } }

    }
}
