using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GS.Trade.Trades
{
    public class PositionNpc : Position, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ulong _firstTradeNumber;
        public override ulong FirstTradeNumber
        {
            get { return _firstTradeNumber; }
            set 
            { 
                _firstTradeNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FirstTradeNumber"));
            }
        }

        private DateTime _firstTradeDT;
        public override  DateTime FirstTradeDT
        {
            get { return _firstTradeDT; }
            set 
            { _firstTradeDT = value;
            OnPropertyChanged(new PropertyChangedEventArgs("FirstTimeDateString"));
            OnPropertyChanged(new PropertyChangedEventArgs("FirstTradeDTString"));
            }
        }

        private ulong _lastTradeNumber;
        public override ulong LastTradeNumber
        {
            get { return _lastTradeNumber; }
            set 
            { _lastTradeNumber = value;
            OnPropertyChanged(new PropertyChangedEventArgs("LastTradeNumber"));
            }
        }

        private DateTime _lastTradeDT;
        public override DateTime LastTradeDT
        {
            get { return _lastTradeDT; }
            set
            { _lastTradeDT = value;
            OnPropertyChanged(new PropertyChangedEventArgs("LastTradeDTString"));
            }
        }

        private decimal _price1;
        public override decimal Price1
        {
            get { return _price1; }
            set { _price1 = value;
            OnPropertyChanged(new PropertyChangedEventArgs("AvgPriceString"));
            }
        }

        private decimal _price2;
        public override decimal Price2
        {
            get { return _price2; }
            set { _price2 = value;
            OnPropertyChanged(new PropertyChangedEventArgs("LastTradePriceString"));
            OnPropertyChanged(new PropertyChangedEventArgs("PnLString"));
            OnPropertyChanged(new PropertyChangedEventArgs("PnLTotalString"));
            }
        }

        PosOperationEnum _operation;
        public override PosOperationEnum Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OperationString"));
            }
        }

        private long _quantity;
        public override long Quantity
        {
            get { return _quantity; }
            set { 
                _quantity = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("PositionString2"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
                OnPropertyChanged(new PropertyChangedEventArgs("PnLString"));
            }
        }

        private PosStatusEnum _status;
        public override PosStatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StatusString"));
            }
        }

        // PositionTotal

        public  long QuantityClosed
        {
            get { return PositionTotal != null ? PositionTotal.Quantity : 0; }
            /*
            set
            {
                _quantity = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PositionString2"));
                OnPropertyChanged(new PropertyChangedEventArgs("AmountString"));
                OnPropertyChanged(new PropertyChangedEventArgs("PnLString"));
            }
             */ 
        }
        public string PnLClosedString
        {
            get { return PositionTotal != null ? PositionTotal.TotalPnLString : 0.ToString(TickerFormatM); }
        }
        public decimal PnLTotal { get { return PnL + (PositionTotal != null ? PositionTotal.CurrencyPnL : 0); } }
        public string PnLTotalString
        {
            get { return (PnL + (PositionTotal != null ? PositionTotal.CurrencyPnL : 0)).ToString(TickerFormatM); }
        }
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void PositionTotalEventHandler(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "PositionString2":
                    OnPropertyChanged(new PropertyChangedEventArgs("QuantityClosed"));
                    break;
                case "PnLString":
                case "Price2":
                    OnPropertyChanged(new PropertyChangedEventArgs("PnLClosedString"));
                    break;
            }
        }
        
    }
}
