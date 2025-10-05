using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.EventLog;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.DataBase1.Model;
using GS.Events;
using GS.Interfaces;
using GS.Serialization;

using System.Speech.Synthesis;


namespace CApp_Test_EvlContext1
{
    class Program
    {
        public static IEventLog Evl { get; set; }

        static void Main(string[] args)
        {
            Database.SetInitializer(new Initializer());
            // var db = new EvlContext1();
            //var evl = new DbEventLog
            //{
            //    Name = "GS.Trade.EventLog",
            //    Alias = "GS.Trade.EventLog",
            //    Code = "GS.Trade.EventLog",
            //    Description = "GS.Trade.EventLog",
            //};
            //db.EventLogs.Add(evl);
            //db.SaveChanges();

            //var synth = new SpeechSynthesizer();
            //synth.SetOutputToDefaultAudioDevice();
            //synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
          
            //synth.Speak("Init DataBase is Completed Init() . Press any key ...");
            //synth.Speak("SUCCESS, Technology, Initialization");

            ConsoleSync.WriteReadLineT("Init Db Complete. Press any key ...");

            //var dbevl2 = Builder.Build<DbEventLog2>(@"Init\EventLog.xml", "DbEventLog2");

            //var dbevl2 = DbEventLog2.Instance;


            Evl = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            Evl.ExceptionEvent += (s, o) =>
            {
                var ia = (IEventArgs) o;
                if (ia.Operation == "ADDMANY")
                {
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine("Exceptions Many:");
                    foreach (var e in (List<GS.Exceptions.GSException>) (ia.Object))
                    {
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("---------------------------------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine("Exceptions Single:");
                    Console.WriteLine(ia.Object.ToString());
                    Console.WriteLine("---------------------------------------------------------------");
                }
                
                // ConsoleSync.WriteReadLine("Exception Press Any Key ....");
            };
            Evl.Init();

            ConsoleSync.WriteReadLine("EventLog Init Complete ---");

            Evl.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, "EventLog", "Test New EventLog1", "", "", "");

            ConsoleSync.WriteReadLineT("Press any key To Start First Simple Sync Test...");
            foreach (var i in Enumerable.Range(1, 1000))
            {
                Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "SendItem", "", "Item: " + i.ToString(), "", "");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            ConsoleSync.WriteReadLineT("Press any key To Start One Simple Task...");

            var ta = Task.Factory.StartNew(() => Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Test:", "Simple Test", "", "", ""));
            ta.Wait();
            Console.WriteLine("Task Status: {0}", ta.Status);

            //ta.Start();
            //Console.WriteLine("Task Status: {0}", ta.Status);
            //ta.Wait();
            //Console.WriteLine("Task Status: {0}", ta.Status);

            //while (ta.Status == TaskStatus.RanToCompletion)
            //{
            //    Console.WriteLine("Task Status: {0}", ta.Status);
            //    Thread.Sleep(TimeSpan.FromSeconds(5));
            //}
            ConsoleSync.WriteReadLineT("Press any key To Start Tasks...");

            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(1, 100))
            {
                var t = Task.Factory.StartNew((p) =>
                {
                    var v = new Random().Next(1, 10);

                    foreach (var j in Enumerable.Range(1, 100))
                    {
                        try
                        {
                            Evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Task:" + p.ToString(), "Item:" + j.ToString(),
                                "", "", "");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("---------------------------------------------");
                            Console.WriteLine("Exception: " + e.ToString());
                            Console.WriteLine("---------------------------------------------");
                            Thread.Sleep(TimeSpan.FromSeconds(300));
                            Console.WriteLine("Continue................");

                            // throw;
                        }
                        // ConsoleAsync.WriteLineT(j.ToString());
                        
                        // Task.Delay(TimeSpan.FromSeconds(v));
                        Thread.Sleep(v*100);
                    }
                },i);
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());

            ConsoleSync.WriteReadLineT("Tasks is Complete. Press any key ...");

        }
    }
}
