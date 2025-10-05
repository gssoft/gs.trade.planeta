using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Contexts;
using GS.Interfaces;
using GS.Trade.Windows;
using System.Windows;
using System.Windows.Threading;

namespace Ca_Test_Context2_01
{
    //public class TestContext : Context2
    //{
    //    public EventLogWindow2 EventLogWindow2;
    //    public EventLogWindow3 EventLogWindow3;
    //    public override void DeQueueProcess()
    //    {
    //        //throw new NotImplementedException();
    //        Evlm1(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, ParentTypeName, TypeName,
    //            MethodBase.GetCurrentMethod().Name, "Runnig ...", "");
    //    }
    //    protected Thread WinThread1;
    //    protected Thread WinThread2;

    //    public override void StartLogger()
    //    {
    //        // throw new NotImplementedException();
    //    }

    //    public override void Init()
    //    {
    //        base.Init();
    //        // StartEventLogWindow1();
    //        //WinThread = new Thread(StartEventLogWindow);
    //        //WinThread.SetApartmentState(ApartmentState.STA);
    //        //WinThread.Start();
    //    }
    //    public override void Start()
    //    {
    //        base.Start();
    //    }
    //    public override void Stop()
    //    {
    //        base.Stop();
    //        //EventLogWindow2?.Close();
    //        //EventLogWindow3?.Close();
    //        // CloseEventLogWindow1();

    //       // WinThread1.Join();
    //       // EventLogWindow2?.Close();

    //        WinThread1.Abort();
    //        WinThread1.Join();
    //    }

    //    private void StartEventLogWindow1()
    //    {
    //        WinThread1 = new Thread(() =>
    //        {
    //            EventLogWindow2 = new EventLogWindow2();
    //            EventLogWindow2.Init(EventLog);
    //            EventLogWindow2?.Show();

    //            EventLogWindow3 = new EventLogWindow3();
    //            EventLogWindow3.Init(EventLog);
    //            EventLogWindow3?.Show();

    //            Dispatcher.Run();
    //        });
    //        WinThread1.SetApartmentState(ApartmentState.STA);
    //        WinThread1.Start();
    //    }
        
    //    private void CloseEventLogWindow1()
    //    {
    //        WinThread2 = new Thread(() =>
    //        {
    //            EventLogWindow2?.Close();
    //            EventLogWindow3?.Close();

    //            Dispatcher.Run();
    //        });
    //        WinThread2.SetApartmentState(ApartmentState.STA);
    //        WinThread2.Start();
    //    }
    //    private void StartEventLogWindow2()
    //    {
    //        // Does not work
    //        Application.Current.Dispatcher.Invoke(() =>
    //        {
    //            EventLogWindow2 = new EventLogWindow2();
    //            EventLogWindow2.Init(EventLog);
    //            EventLogWindow2?.Show();

    //            EventLogWindow3 = new EventLogWindow3();
    //            EventLogWindow3.Init(EventLog);
    //            EventLogWindow3?.Show();
    //        });
    //    }
    //    private void CloseEventLogWindow2()
    //    {
    //        // does not work
    //        Application.Current.Dispatcher.Invoke(() =>
    //        {
    //            EventLogWindow2?.Close();
    //            EventLogWindow3?.Close();
    //        });
    //    }
    //}

    internal class Program
    {
        static void Main(string[] args)
        {
            var cntx = new TestContext
            {
                IsNeedEventHub = true,
                IsNeedEventLog = true
            };

            cntx.Init();
            ConsoleSync.WriteReadLineT("Init is Completed ...");
            cntx.Start();
            ConsoleSync.WriteReadLineT("Start is Completed ...");
            cntx.Stop();
            ConsoleSync.WriteReadLineT("Stop is Completed ...");
        }
    }
}
