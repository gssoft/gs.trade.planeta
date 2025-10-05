using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using GS.EventLog;
using GS.ConsoleAS;
using GS.EventLog;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;

namespace caWebEventLogTest
{
    class Program
    {
        private static IEventLogs _evl;
        private static Random _rand;
        static void Main(string[] args)
        {
            _evl = Builder.Build<EventLogs>(@"Init\EventLog2.xml", "EventLogs");
            _evl.Init();

            var dtnow = DateTime.Now;
            var dtOneDay = dtnow.OneDay();
            var dtWeek = dtnow.Week();
            var dtWeekToDay = dtnow.WeekToDay();
            var dtWeekDayToDay = dtnow.WeekDayToDay();
            var dtPrevWeek = dtnow.WeekPrev();

            var dtMonth = dtnow.Month();
            var dtMonthToDay = dtnow.MonthToDay();
            var dtMonthDayToDay = dtnow.MonthDayToDay();
            var dtPrevMonth = dtnow.MonthPrev();
           

            
            ConsoleSync.WriteLineDT("Day: " + dtOneDay.Key.ToString("u") + " " + dtOneDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("Week: " + dtWeek.Key.ToString("u") + " " + dtWeek.Value.ToString("u"));
            ConsoleSync.WriteLineDT("WeekToDay: " + dtWeekToDay.Key.ToString("u") + " " + dtWeekToDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("WeekDayToDay: " + dtWeekDayToDay.Key.ToString("u") + " " + dtWeekDayToDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("PrevWeek: " + dtPrevWeek.Key.ToString("u") + " " + dtPrevWeek.Value.ToString("u"));

            ConsoleSync.WriteLineDT("-------");

            ConsoleSync.WriteLineDT("Day: " + dtOneDay.Key.ToString("u") + " " + dtOneDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("Month: " + dtMonth.Key.ToString("u") + " " + dtMonth.Value.ToString("u"));
            ConsoleSync.WriteLineDT("MonthToDay: " + dtMonthToDay.Key.ToString("u") + " " + dtMonthToDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("MonthDayToDay: " + dtMonthDayToDay.Key.ToString("u") + " " + dtMonthDayToDay.Value.ToString("u"));
            ConsoleSync.WriteLineDT("PrevMonth: " + dtPrevMonth.Key.ToString("u") + " " + dtPrevMonth.Value.ToString("u"));

            ConsoleSync.WriteReadLineDT("Finish...");


            //_rand = new Random();
            //for (var i = 0; i < 10000; i++)
            //{
            //    var randseconds = _rand.Next(5, 50);
            //    Thread.Sleep(randseconds*100);
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Test from caApplication",
            //        i.ToString(), i.ToString(), i.ToString(), i.ToString());
            //}
        }
    }
}
