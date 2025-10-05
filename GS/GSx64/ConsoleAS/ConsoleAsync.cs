using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.String;

namespace GS.ConsoleAS
{
    public static class ConsoleAsync
    {
        #region public members
        public static async void WriteLine(string s)
        {
            await WriteLineTask(s);
        }      
        public static async void WriteLine(string format, params object[] args)
        {
            await WriteLineTask(format, args);
        }
        public static async void WriteLineT(string s)
        {
            s = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineTs(string s)
        {
            s = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineTc(string s)
        {
            s = DateTime.Now.TimeOfDay.ToString("c") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineTsg(string s)
        {
            s = DateTime.Now.TimeOfDay.ToString("g") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineTlg(string s)
        {
            s = DateTime.Now.TimeOfDay.ToString("G") + " " + s;
            await WriteLineTask(s);
        }
      
        public static async void WriteLineT(string format, params object[] args)
        {
            var s = DateTime.Now.TimeOfDay.ToString("c") + " " + Format(format,args);
            await WriteLineTask(s);
        }
        public static async void WriteLineST(string format, params object[] args)
        {
            var s = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff") + " " + Format(format, args);
            await WriteLineTask(s);
        }

        public static async void WriteLineD(string s)
        {
            s = DateTime.Now.Date.ToString("d") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineD(string format, params object[] args)
        {
            var s = DateTime.Now.Date.ToString("d") + " " + Format(format, args);
            await WriteLineTask(s);
        }
        public static async void WriteLineDT(string s)
        {
            s = DateTime.Now.ToString("G") + " " + s;
            await WriteLineTask(s);
        }
        public static async void WriteLineDT(string format, params object[] args)
        {
            var s = DateTime.Now.ToString("G") + " " + Format(format, args);
            await WriteLineTask(s);
        }
        #endregion

        #region private members
        private static Task WriteLineTask(string s)
        {
            return Task.Factory.StartNew(() => Console.WriteLine(s));
        }
        private static Task WriteLineTask(string format, params object[] args)
        {
            return Task.Factory.StartNew(() => Console.WriteLine(format, args));
        }
        #endregion
    }
}