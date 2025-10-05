using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.CompilerServices;
using GS.ConsoleAS;
using GS.Elements;
using GS.Events;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;

namespace RecursiveFilesCmp_01
{
    class Program
    {
        public  static IEventLog EventLog { get; set; }

        static void Main(string[] args)
        {
            try
            {
                EventLog = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
                EventLog.Init();

                //var comparer = Builder.Build<CompareFiles>(@"Init\CompareInfoTest.xml", "CompareFiles");
                var comparer = Builder.Build<CompareFiles>(@"Init\CompareInfo.xml", "CompareFiles");
                comparer.ExceptionEvent += ExceptionEventHandler;

                comparer?.Init();
                comparer?.Compare();
                
                ConsoleSync.WriteReadLineT(
                    $"Total Files: {comparer?.FilesCount}" +
                    $" StackDepth: {comparer?.StackDepthCount}" +
                    $" Errors: { comparer?.ErrorCount}" +
                    $" Success: { comparer?.SuccessCount}" +
                    $" Exceptions: { comparer?.ExceptionError}");
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
            }
            
            ConsoleSync.WriteReadLineT("Program Finish");       
        }

        public static void ExceptionEventHandler(object sender, IEventArgs arg)
        {
            var e = (GSException)arg.Object;
            //ConsoleSync.WriteLineT(e.ToString());
            EventLog.AddItem(
                EvlResult.FATAL, EvlSubject.PROGRAMMING,
                arg.Type?.ToString(), arg?.Entity, e?.TargetSite, e?.Message, e?.ToString());  
            
            ConsoleSync.WriteLineT(
                $" {e?.ObjStr}");       
        }
    }
}
