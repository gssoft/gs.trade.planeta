using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Interfaces.Dde;
using GS.Tasks;

namespace GS.TcpDde
{
    public class DdeEmulator01 : Element1<string>, IDde
    {
        public override string Key => GetType().Name;
        private Random random;
        private TaskBase02 MainActionTask;

        public DdeEmulator01()
        {
            random = new Random(DateTime.Now.TimeOfDay.Milliseconds);
        }
        public void Start()
        {
            _working = true;
            MainActionTask = new TaskBase02
            {
                Parent = this,
                TimeIntervalForMainTaskCompletingSeconds = 60
            };
            MainActionTask.Start(MainCycle);
        }
        public void Stop()
        {
            _working = false;
        }
        public override void Init()
        {
        }
        public bool IsProcessTaskInUse { get; }
        public bool IsNeedTopicName { get; }
        public ChangesSendMode Mode { get; }
        public Action<string> LineChangesSendAction { get; set; }
        public Action<List<string>> TableChangesSendAction { get; set; }

        private readonly string[] _topics =
                { "QuikDdeServer.OptionDesk", "QuikDdeServer.Quotes", "QuikDdeServer.TickerInfo" };

        private bool _working;

        private void MainCycle(CancellationToken token)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var timedelay = random.Next(1000, 5000);
                while (_working)
                {
                    var lst = CreateTableToSend();
                    lst.Insert(0, GetTopic());
                    TableChangesSendAction(lst);
                    foreach (var i in lst)
                    {
                        ConsoleSync.WriteLineT(i);
                    }                   
                    Thread.Sleep(timedelay);
                }
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m );
                SendException(e);
            }
        }
        private List<string> CreateTableToSend()
        {
            var nquotes = random.Next(1, 10);
            return Enumerable.Range(1, nquotes).Select(i =>
                    $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")} {Guid.NewGuid()}").ToList();
        }
        private string GetTopic()
        {
            var rand = random.Next(0, 3);
            return _topics[rand];
        }
    }
}
