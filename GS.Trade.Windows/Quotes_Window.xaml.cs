using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Process;
using GS.Trade.Quotes;


namespace GS.Trade.Windows
{
    using Quotes;
    /// <summary>
    /// Interaction logic for QuotesWindow.xaml
    /// </summary>
    public partial class QuotesWindow : Window
    {
        private IEventLog _evl;
        private Quotes _quotes;
        private SimpleProcess _observeProcess;

        public QuotesWindow()
        {
            InitializeComponent();
        }
        private void CheckNullReference()
        {
            if (_evl == null || _quotes == null)
            {
                throw new NullReferenceException("QuotesWindow(EventLog == null or Quotes == null)");
            }
        }
        public void Init( IEventLog eventlog, Quotes quotes )
        {              
            _evl = eventlog;
            _quotes = quotes;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "QuotesWindow", "QuotesWindow", "Initialization", "", "");           
            _observeProcess = new SimpleProcess("Quotes Observer Process", 5, 1, CallbackGetQuotesToObserve, _evl);
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();

            lstQuotes.ItemsSource = _quotes.QuoteObserveCollection;
            _observeProcess.Start();
        }
        private void CallbackGetQuotesToObserve()
        {
            // if (_EventLog != null) 
            Dispatcher.BeginInvoke((ThreadStart)(() => _quotes.GetQuotesToObserve()));
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (_observeProcess != null) _observeProcess.Stop();
        }
    }
}
