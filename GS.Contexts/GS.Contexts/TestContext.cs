using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GS.Interfaces;
using GS.Trade.Windows;

namespace GS.Contexts
{
    public class TestContext : ServContext01 // Context3
    {
        //public EventLogWindow2 EventLogWindow2;
       //  public EventLogWindow3 EventLogWindow3;
        public override void DeQueueProcess()
        {
            //throw new NotImplementedException();
            Evlm1(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name, "Runnig ...", "");
        }
        public static IServContext Instance => Lazy.Value;
        private static readonly Lazy<TestContext> Lazy = 
            new Lazy<TestContext>(CreateInstance);
        private static TestContext CreateInstance()
        {
            var instance = new TestContext();
            instance.Init();
            return instance;
        }

        //public override void Init()
        //{
        //    base.Init();
        //   //  StartEventLogWindow1();
        //    //WinThread = new Thread(StartEventLogWindow);
        //    //WinThread.SetApartmentState(ApartmentState.STA);
        //    //WinThread.Start();
        //}
        //public override void Start()
        //{
        //    base.Start();
        //}
        //public override void Stop()
        //{
        //    base.Stop();
        //    //EventLogWindow2?.Close();
        //    //EventLogWindow3?.Close();
        //    // CloseEventLogWindow1();

        //    // WinThread1.Join();
        //    // EventLogWindow2?.Close();

        //    //WinThread1.Abort();
        //    //WinThread1.Join();
        //}

        ////public override void StartLogger()
        ////{
        ////    StartEventLogWindow1();
        ////}
        //// protected Thread WinThread1;
        ////private void StartEventLogWindow1()
        ////{
        ////    WinThread1 = new Thread(() =>
        ////    {
        ////        EventLogWindow3 = new EventLogWindow3();    
        ////        EventLogWindow3.Init(EventLog);
        ////        EventLogWindow3?.Show();

        ////        Dispatcher.Run();
        ////    });
        ////    WinThread1.SetApartmentState(ApartmentState.STA);
        ////    WinThread1.Start();
        ////    Thread.Sleep(1000);
        ////}

        private void CloseEventLogWindow1()
        {
            //WinThread2 = new Thread(() =>
            //{
            //    EventLogWindow2?.Close();
            //    EventLogWindow3?.Close();

            //    Dispatcher.Run();
            //});
            //WinThread2.SetApartmentState(ApartmentState.STA);
            //WinThread2.Start();
        }
        //private void StartEventLogWindow2()
        //{
        //    // Does not work
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        //EventLogWindow2 = new EventLogWindow2();
        //        //EventLogWindow2.Init(EventLog);
        //        //EventLogWindow2?.Show();

        //        EventLogWindow3 = new EventLogWindow3();
        //        EventLogWindow3.Init(EventLog);
        //        EventLogWindow3?.Show();
        //    });
        //}
        //private void CloseEventLogWindow2()
        //{
        //    // does not work
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        //EventLogWindow2?.Close();
        //        EventLogWindow3?.Close();
        //    });
        //}
    }
}
