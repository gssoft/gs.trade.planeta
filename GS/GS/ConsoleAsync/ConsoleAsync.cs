using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//namespace GS.ConsoleAsync
//{
//    //public static class ConsoleAsync
//    //{
//    //    #region public members
//    //    public async static void WriteLine(string s)
//    //    {
//    //        await WriteLineTask(s);
//    //    }      
//    //    public async static void WriteLine(string format, params object[] args)
//    //    {
//    //        await WriteLineTask(format, args);
//    //    }
//    //    public async static void WriteLineT(string s)
//    //    {
//    //        s = DateTime.Now.TimeOfDay.ToString("c") + " " + s;
//    //        await WriteLineTask(s);
//    //    }
//    //    public async static void WriteLineT(string format, params object[] args)
//    //    {
//    //        var s = DateTime.Now.TimeOfDay.ToString("c") + " " + String.Format(format,args);
//    //        await WriteLineTask(s);
//    //    }

//    //    public async static void WriteLineD(string s)
//    //    {
//    //        s = DateTime.Now.Date.ToString("d") + " " + s;
//    //        await WriteLineTask(s);
//    //    }
//    //    public async static void WriteLineD(string format, params object[] args)
//    //    {
//    //        var s = DateTime.Now.Date.ToString("d") + " " + String.Format(format, args);
//    //        await WriteLineTask(s);
//    //    }
//    //    public async static void WriteLineDT(string s)
//    //    {
//    //        s = DateTime.Now.ToString("G") + " " + s;
//    //        await WriteLineTask(s);
//    //    }
//    //    public async static void WriteLineDT(string format, params object[] args)
//    //    {
//    //        var s = DateTime.Now.ToString("G") + " " + String.Format(format, args);
//    //        await WriteLineTask(s);
//    //    }
//    //    #endregion

//    //    #region private members
//    //    private static Task WriteLineTask(string s)
//    //    {
//    //        return Task.Factory.StartNew(() => Console.WriteLine(s));
//    //    }
//    //    private static Task WriteLineTask(string format, params object[] args)
//    //    {
//    //        return Task.Factory.StartNew(() => Console.WriteLine(format, args));
//    //    }
//    //    #endregion
//    //}
//}