using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Events;
using GS.Exceptions;
using GS.Interfaces;
using GS.Trade.Interfaces;
using EventArgs = System.EventArgs;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для Exceptions_Window.xaml
    /// </summary>
    public partial class ExceptionsWindow : Window
    {
        private ITradeContext _tx;
        public ObservableCollection<GSException> GSExceptions { get; set; }
        private readonly object _locker;

        public ExceptionsWindow()
        {
            InitializeComponent();
            _locker = new object();

            GSExceptions = new ObservableCollection<GSException>();
        }
        public void Init(ITradeContext tx)
        {
            if (tx == null)
                throw new NullReferenceException("ExceptionsWindow.Init(Tx == null)");

            _tx = tx;
            _tx.Evlm(EvlResult.SUCCESS, EvlSubject.TRADING, "ExceptionsWindow", "ExceptionsWindow", "Initialization", "", "");
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_tx == null)
                throw new NullReferenceException("ExceptionsWindow.Init(Tx == null)");

            LstExceptions.ItemsSource = GSExceptions;
            _tx.EventHub.Subscribe("UI.Exceptions", "Exception", CallbackTradeOperation);
            _tx.EventHub.ExceptionEvent += CallbackTradeOperation;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Clear();
            if (_tx == null)
                return;
            _tx.EventHub.UnSubscribe("UI.Exceptions", "Exception", CallbackTradeOperation);
            _tx.EventHub.ExceptionEvent -= CallbackTradeOperation;

        }
        private void Clear()
        {
            lock (_locker)
                GSExceptions.Clear();
        }

        private void CallbackTradeOperation(object sender, IEventArgs args)
        {
            if (args.Object == null)
                return;

            //var t = args.Object as GSException;
            //var many = args.Object as IEnumerable<GSException>;
            
            switch (args.OperationKey)
            {
                case "UI.EXCEPTIONS.EXCEPTION.ADD":
                    var t = args.Object as GSException;
                    if (t == null)
                        return;
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            GSExceptions.Insert(0, t);
                        }
                    }
                   ));
                   break;
                case "UI.EXCEPTIONS.EXCEPTION.ADDMANY":
                   var excs = args.Object as IEnumerable<GSException>;
                   if (excs == null)
                       return;
                   Dispatcher.BeginInvoke((ThreadStart)(() =>
                   {
                       lock (_locker)
                       {
                           foreach(var e in excs)
                                GSExceptions.Insert(0, e);
                       }
                   }
                  ));
                   break;
                case "UI.EXCEPTIONS.EXCEPTION.DELETE":
                    t = args.Object as GSException;
                    if (t == null)
                        return;
                    Dispatcher.BeginInvoke((ThreadStart)(() =>
                    {
                        lock (_locker)
                        {
                            var ot = GSExceptions.FirstOrDefault(it => it.Key == t.Key);
                            if (ot != null)
                                GSExceptions.Remove(ot);
                        }
                    }
                    ));
                    break;
            }
        }
    }
}
