using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Test_TradeContext.DataContext
{
    public class PortfolioSettings : INotifyPropertyChanged
    {
        private bool _longEnabled;
        private bool _shortEnabled;
        public int  MaxSideSize { get; set; }
        public bool LongEnabled
        {
            get { return _longEnabled; }
            set
            {
                _longEnabled = value;
                OnPropertyChanged("LongEnabled");
            }
        }
        public bool ShortEnabled
        {
            get { return _shortEnabled; }
            set
            {
                _shortEnabled = value;
                OnPropertyChanged("ShortEnabled");
            }
        }
        public override string ToString()
        {
            return $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled} MaxSideSize:{MaxSideSize}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
