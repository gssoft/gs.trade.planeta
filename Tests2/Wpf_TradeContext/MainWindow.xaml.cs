using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GS.EventLog;
using GS.Interfaces;
using GS.Trade.TradeContext;

namespace Wpf_TradeContext
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TradeContext _tx;
        private IEventLog _evl; 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
             //     var evls = new EventLogs();
             //     evls.DeSerializationCollection();

                  _evl = new EventLog();
                  _tx = new TradeContext { EventLog = _evl };

                  _tx.Init();
                  _tx.Open();
                  _tx.OpenChart();
                  _tx.Start();
        }

       
    }
}
