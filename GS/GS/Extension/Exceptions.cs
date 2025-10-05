using System;
using System.Linq;

//namespace GS.Extension
//{
//    public static class ExceptionExt
//    {
//        public static string AggrExceptionMessage(this AggregateException e)
//        {
//            return e.Message + e.InnerExceptions.Aggregate(e.Message, (current, v) => current + ("\r\n" + v.Message));
//        }
//        public static string ExceptionMessage(this Exception e)
//        {
//            var s = e.Message;
//            while (e.InnerException != null)
//            {
//                e = e.InnerException;
//                s += "\r\n" + e.Message;
//            }
//            return s;
//        }
//    }
//}
