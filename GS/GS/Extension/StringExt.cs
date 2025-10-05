using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GS.Extension
{
    public static partial class StringExt
    {
        public static string ToTitleCase(this string str)
        {
            return !string.IsNullOrWhiteSpace(str)
                ? CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(str)
                : str;
        }
        
        public static string Left(this string str, int length)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            var l = length >= 0 ? length : 0;
            str = str.Trim();
            return str.Substring(0, l <= str.Length ? l : str.Length);
        }
        public static string Right(this string str, int length)
        {
            if( string.IsNullOrWhiteSpace(str)) return str;
            str = str.Trim();
            var l = length >= 0 ? length : 0;
            return str.Substring(l < str.Length ? str.Length - l : 0);
        }
        public static string WithBrackets(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return " (" + str + ") ";
        }
        public static string WithSqBrackets(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return " [" + str + "] ";
        }
        public static string WithSqBrackets0(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return "[" + str + "]";
        }

        public static string WithRight(this string str, string rightStr)
        {
            return str + rightStr;
        }
        public static string WithSpRight(this string str, string rightStr)
        {
            return str + " " + rightStr;
        }
        public static string WithSepRight(this string str, string separator, string rightStr)
        {
            return str + separator + rightStr;
        }
        public static string WithSepRight(this string str, string separator, params string[] strlst)
        {
            return str + strlst.Aggregate(String.Empty, (current, s) => current + (separator + s));
        }

       
        public static string WithLeft(this string str, string leftStr)
        {
            return leftStr + str;
        }
        public static string WithSpLeft(this string str, string leftStr)
        {
            return leftStr + " " + str;
        }

        public static string TrimUpper(this string str)
        {
            return str.HasValue() ? str.Trim().ToUpper() : str;
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        public static bool HasNoValue(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string AddRight(this string str, string delimiter, params string[] arr)
        {
            if( arr.Length == 0)
                return str;
            return  arr.Aggregate(str, (current, s) => current + (delimiter + s));
        }

        public static double ToDouble(this string str)
        {
            return double.Parse(str.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
        }
        public static float ToSingle(this string str)
        {
            return float.Parse(str.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
        }

        public static byte[] ToLinesInBytes(this string content)
        {
           var spl = content.Split('\r', '\n');
            // Memory leaking with SelectMany with large string content
            return spl.SelectMany(s =>
                        Encoding.UTF8.GetBytes(s)).ToArray();
        }     
        public static byte[] ToBytes(this string content)
        {
            return Encoding.UTF8.GetBytes(content); // .ToArray();
        }

        // REVERSE with Array method
        public static string Reverse(this string text)
        {
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return (new string(array));
        }
    }

    public class GSstring
    {
        // REVERSE *************************************
        public static string ReverseXor( string s)
        {
            char[] charArray = s.ToCharArray();
            int len = s.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }
            return new string(charArray);
        }
        public static string ReverseSB(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length);
            for (int i = text.Length - 1; i >= 0; i--)
            {
                builder.Append(text[i]);
            }
            return builder.ToString();
        }
        public static string ReverseArray(string text)
        {
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return (new string(array));
        }
        public static string Reverse(string text)
        {
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return (new string(array));
        }
        // **************************************************************

        public static byte[] ToBytesWithoutrn(string content)
        {
            var s = content.Replace("\r", "").Replace("\n", "");
            return Encoding.UTF8.GetBytes(s);
        }
        public static byte[] ToBytes(string content)
        {
            return Encoding.UTF8.GetBytes(content);
        }
        public static byte[] ToLinesInBytes(string content)
        {
            var spl = content.Split('\r', '\n');
            // Memory leaking with SelectMany with large string content
            return spl.SelectMany(s =>
                        Encoding.UTF8.GetBytes(s)).ToArray();
        }
    }
}
