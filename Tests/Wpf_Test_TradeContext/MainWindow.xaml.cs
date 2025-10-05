using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GS.Assemblies;
using GS.Configurations;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.Interfaces;
using GS.Trade.TradeContext;

using WebClients;
using Wpf_Test_TradeContext.DataContext;

namespace Wpf_Test_TradeContext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ITradeContext _tx;
        // private IEventLogs _evl; 
        private IEventLog _evl;

        private DataContext.PortfolioSettings _portfolioSettings;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _evl = Builder.Build2<IEventLog>(@"Init\EventLogMem.xml", "EventLogs");
                _evl.Init();

                _portfolioSettings = new PortfolioSettings
                {
                    LongEnabled = true,
                    ShortEnabled = true,
                    MaxSideSize = 10
                };

                TextBtMaxSideSize.DataContext = _portfolioSettings;
                ChBoxLongEnabled.DataContext = _portfolioSettings;
                ChBoxShortEnabled.DataContext = _portfolioSettings;

                // _evl.SetMode(EvlModeEnum.Init);
                //var margin = Builder.Build<RtsMargin.RtsMargin>(@"Init\RtsMargin.xml", "RtsMargin");

                //     _evl = new EventLog();
                //     _evl = evls.GetPrimary();
                //_tx = new TradeContext { EventLog = _evl };
                //_tx  =  Builder.Build<TradeContext>(@"Init\TradeContext.xml", "TradeContext");
                _tx = Builder.Build<TradeContext51>(@"Init\TradeContext.xml", "TradeContext51");

                _tx.EventLog = _evl;
                _tx.Init();
                _evl = _tx.EventLog;

                //Title = _tx.ConfigurationResourse.ConfigurationKey; // + " Configuration";
                Title = _tx.ConfigurationResourse1.ConfigurationKey;
                
                _tx.Open();

                _tx.OpenChart();
                _tx.Start();
                // _tx.StartRand();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NullReferenceException("MainWindow: Main TradeContext Initialization Failure");
            }
        }
    }
}
