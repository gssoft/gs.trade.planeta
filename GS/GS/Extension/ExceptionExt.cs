using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;


namespace GS.Extension
{
    public static class ExceptionExt
    {
        public static string AggrExceptionMessage(this AggregateException e)
        {
            return e.Message + e.InnerExceptions.Aggregate(e.Message, (current, v) => current + ("\r\n" + v.Message));
        }
        public static string ExceptionMessage(this Exception e)
        {
            var s = e.Message;
            while (e.InnerException != null)
            {
                e = e.InnerException;
                s += "\r\n" + e.Message;
            }
            return s;
        }
        public static string ExceptionMessageAgg(this Exception e)
        {
            var origExc = e;
            var s = "Exception:" + Environment.NewLine + e.Message;
            while (e.InnerException != null)
            {
                e = e.InnerException;
                s += "\r\n" + e.Message;
            }
            var ae = origExc as AggregateException;
            if (ae == null)
                return s;
            s += ae.AggExceptionMessage();
            return s;
        }
        public static string AggExceptionMessage(this AggregateException ae)
        {
            string[] s = { "AggregateException:" + Environment.NewLine + ae.Message };
            ae = ae.Flatten();
            ae.Handle(ie =>
            {
                s[0] += Environment.NewLine + ie.Message;
                return true;
            });
            return s[0];
        }
        public static string[] AggExceptionMessageArr(this AggregateException ae)
        {
            var ss = new string[] {};
            ss[0] = $"{ae.Message}";
            ae = ae.Flatten();
            var i = 1;
            ae.Handle((ie) =>
            {
                ss[i++] = ie.Message;
                return true;
            });
            return ss;
        }

        public static List<string> ToList(this Exception e)
        {
            var lst = new List<string> {e.ToString()};
            var origExc = e;
            //var s = "Exception:" + Environment.NewLine + e.Message;
            while (e.InnerException != null)
            {
                e = e.InnerException;
                lst.Add(e.ToString());
            }
            return lst;
            /*
            var ae = origExc as AggregateException;
            if (ae == null)
                return s;
            s += ae.AggExceptionMessage();
            return s;
            */
        }
        public static List<string> ToList(this AggregateException ae)
        {
            var lst = new List<string> { ae.ToString() };
            //string[] s = { "AggregateException:" + Environment.NewLine + ae.Message };
            ae = ae.Flatten();
            ae.Handle((ie) =>
            {
                // s[0] += Environment.NewLine + ie.Message;
                lst.Add(ie.ToString());
                return true;
            });
            return lst;
        }
        public static void ToConsoleSync(this Exception exs)
        {
            foreach (var e in exs.ToList())
                ConsoleSync.WriteLineT(e);
        }
        public static void ToConsoleSync(this AggregateException exs)
        {
            foreach (var e in exs.ToList())
                ConsoleSync.WriteLineT(e);
        }


        public static string ExceptionMessageAsync(Exception e)
        {
            return ExceptionMessageA(e).Result;
        }

        private static Task<string> ExceptionMessageA(this Exception exc)
        {
            return Task<string>.Factory.StartNew(exception =>
            {
                var e = (Exception) exception;
                var s = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    s += "\r\n" + e.Message;
                }
                return s;
            }, exc);
        }

        public static void PrintExceptions(this Exception e)
        {
            e.PrintException();
            while (e.InnerException != null)
            {
                e = e.InnerException;
                e.PrintException();
            }
        }
        public static void PrintException(this Exception e)
        {
            Console.WriteLine($"{e.GetType()}: {e.Message}");
            Console.WriteLine($"{e.TargetSite}");
        }
        public static void PrintExceptions(this Exception e, string method)
        {
            e.PrintException(method);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                e.PrintException(method);
            }
        }
        public static void PrintException(this Exception e, string method)
        {
            // Console.WriteLine($"{GetType().FullName}");
            Console.WriteLine($"{method}");
            Console.WriteLine($"{e.GetType()}: {e.Message}");
            Console.WriteLine($"{e.TargetSite}");
        }
        public static void PrintExceptions(this Exception e, object obj, string method)
        {
            e.PrintException(obj, method);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                e.PrintException(obj, method);
            }
        }
        public static void PrintException(this Exception e, object obj, string method)
        {
            Console.WriteLine($"{obj?.GetType().FullName}");
            Console.WriteLine($"{method}");
            Console.WriteLine($"{e.GetType()}: {e.Message}");
            Console.WriteLine($"{e.TargetSite}");
        }
    }
}
