using System;
using System.Collections.Generic;
using System.Linq;

namespace TcpIpSockets.Extensions
{
    public static class MessageExtensions
    {
        public static string MessageToString(this string[] strings)
        {
            var strEnd = Environment.NewLine;
            var str = strings.Aggregate(string.Empty, (current, i) => current + $"{i}{strEnd}");
            if(str.Length>strEnd.Length)
                str = str.Substring(0, str.Length - strEnd.Length);
            return str;
        }
        public static string MessageToString(this List<string> strings)
        {
            var strEnd = Environment.NewLine;
            var str = strings.Aggregate(string.Empty, (current, i) => current + $"{i}{strEnd}");
            if (str.Length > strEnd.Length)
                str = str.Substring(0, str.Length - strEnd.Length);
            return str;
        }

        public static string MessageToString<T>(this T strings)
        {
            var inputType = typeof (T);
            if (inputType == typeof(List<string>))
            {
                var list = strings as List<string>;
                if (list == null) return null;
                var strEnd = Environment.NewLine;
                var str = list.Aggregate(string.Empty, (current, i) => current + $"{i}{strEnd}");
                return str.Length > strEnd.Length 
                    ? str.Substring(0, str.Length - strEnd.Length) 
                    : str;
            }
            if (inputType == typeof(string[]))
            {
                var strarr = strings as string[];
                if (strarr == null) return null;
                var strEnd = Environment.NewLine;
                var str = strarr.Aggregate(string.Empty, (current, i) => current + $"{i}{strEnd}");
                return str.Length > strEnd.Length 
                    ? str.Substring(0, str.Length - strEnd.Length) 
                    : str;
            }
            return null;
        }
    }
}
