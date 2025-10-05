using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.Interfaces.Dde;
using GS.Serialization;
using GS.Trade.Dde.TpSbscr;
using GS.ConsoleAS;
namespace CA_Test_Dde_02
{
    class Program
    {
        public static Tickers Tickers;
        public static XDocument XDoc;
        public static GS.Trade.Dde.TpSbscr.Dde DdeServer;

        private static double _iterationCnt = 0;
        private static double _itemCnt = 0;
        private static double _average;
        private static int _lastIterationItemCnt;
        private static bool _working;

        static void Main(string[] args)
        {
            Tickers = new Tickers();
            Test_Dde_TpSbscr_01();
        }
        private static void Test_Dde_TpSbscr_01()
        {
            _working = true;
            //var t = Task.Factory.StartNew(PrintAverage);
            Task.Factory.StartNew(PrintAverage).ContinueWith(TaskCompleted);

            XDoc = XDocument.Load(@"Init\Dde.xml");
            DdeServer = Builder.Build2<Dde, string, ITopicItem>(XDoc, "Dde");
            //DdeServer.Parent = this;
            DdeServer.Init();
            //DdeServer.TableChangesSendAction = TableString;
            DdeServer.Subscribe("QuikDdeServer.Quotes", TableString);
            DdeServer.Subscribe("QuikDdeServer.TickerInfo", TableString);
            DdeServer.Subscribe("QuikDdeServer.OptionDesk", TableString);

            DdeServer.Start();
            ConsoleSync.WriteReadLine("Press any key to Stop Dde");
            DdeServer.TableChangesSendAction = null;
            DdeServer.UnSubscribe("QuikDdeServer.Quotes", TableString);
            DdeServer.UnSubscribe("QuikDdeServer.TickerInfo", TableString);
            DdeServer.UnSubscribe("QuikDdeServer.OptionDesk", TableString);
            DdeServer.Stop();

            ConsoleSync.WriteReadLine("Press any key to Finish");
            _working = false;
            Thread.Sleep(5000);
            ConsoleSync.WriteLineT($"Working:{_working}");
            Thread.Sleep(5000);
        }
        private static void TableString(object sender,  List<string> lst)
        {
            _iterationCnt++;
            _lastIterationItemCnt = lst.Count - 1;
            _itemCnt += _lastIterationItemCnt;
            _average = _iterationCnt > 0 ? _itemCnt / _iterationCnt : 0;
            
            if (!lst.Any()) return;

            //ConsoleSync.WriteLineST($"********* TopicName:{lst[0]} Count:{lst.Count} **********{Environment.NewLine}");
            foreach (var s in lst)
            {
                // ConsoleSync.WriteLineST(s);
                var optinfo = Tickers.ParseOptionDeskItemStr(s);
                if (optinfo?.CallInfo == null) continue;
                if (optinfo.PutInfo == null) continue;
                Console.WriteLine($"{optinfo.CallInfo}{Environment.NewLine}");
                Console.WriteLine($"{optinfo.PutInfo}{Environment.NewLine}");
            }
        }

        private static void PrintAverage()
        {
            while (_working)
            {
                ConsoleSync.WriteLineT(
                    $"***** Average:{_average:N3} Iterations:{_iterationCnt} ItemsCnt:{_itemCnt} LastIterationItemsCnt:{_lastIterationItemCnt}{Environment.NewLine}");
                Thread.Sleep(5000);
            }
            ConsoleSync.WriteLineT("Task goes to Completed...");
        }
        private static void TaskCompleted(Task t)
        {
            ConsoleSync.WriteReadLine($"Task completed. Status:{t.Status} Press any key to Finish ...");
        }
    }
}
