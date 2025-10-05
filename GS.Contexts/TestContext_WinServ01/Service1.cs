using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Contexts;

namespace TestContext_WinServ01
{
    [RunInstaller(true)]
    public partial class TestContextWinServ03 : ServiceBase
    {
        TestContext _testContext;
        public TestContextWinServ03()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            _testContext = new TestContext
            {
                IsNeedEventHub = true,
                IsNeedEventLog = true
            };

            _testContext.Init();
            // ConsoleSync.WriteReadLineT("Init is Completed ...");
            _testContext.Start();
            // ConsoleSync.WriteReadLineT("Start is Completed ...");
        }
        protected override void OnStop()
        {
            _testContext.Stop();
           //  ConsoleSync.WriteReadLineT("Stop is Completed ...");
        }
    }
}
