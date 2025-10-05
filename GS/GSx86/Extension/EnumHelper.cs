using System;
using System.Collections.Generic;

namespace GS.Extension
{
    public static class EnumHelper
    {
        public static List<T> EnumToList<T>()
        {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            var enumValArray = Enum.GetValues(enumType);

            var enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }
        /// <summary>
        ///  working well
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
 
        public static List<KeyValuePair<T,string>> EnumToKeyValuePairList<T>()
        {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            var enumValArray = Enum.GetValues(enumType);

            var enumValList = new List<KeyValuePair<T, string>>(enumValArray.Length);

            foreach (var val in enumValArray)
            {
                var kvpair = new KeyValuePair<T, string>((T)val, val.ToString());
                enumValList.Add(kvpair);
            }
            return enumValList;
        }
        /// <T> redudant
        /// Working Weel
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <returns></returns>
        public static List<KeyValuePair<T, string>> ToKeyValuePairList<T>(this T en)
        {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            var enumValArray = Enum.GetValues(typeof(T));

            var enumValList = new List<KeyValuePair<T, string>>(enumValArray.Length);

            foreach (var val in enumValArray)
            {
                var kvpair = new KeyValuePair<T, string>((T)val, val.ToString());
                enumValList.Add(kvpair);
            }

            return enumValList;
        }
        /// <summary>
        /// Working Well
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static List<KeyValuePair<System.Enum, string>> ToKeyValuePairList(this System.Enum en)
        {
            //Type enumType = typeof(System.Enum);

            //// Can't use type constraints on value types, so have to do check like this
            //if (enumType.BaseType != typeof(Enum))
            //    throw new ArgumentException("T must be of type System.Enum");

            var enumValArray = Enum.GetValues(typeof(System.Enum));

            var enumValList = new List<KeyValuePair<System.Enum, string>>(enumValArray.Length);

            foreach (var val in enumValArray)
            {
                var kvpair = new KeyValuePair<System.Enum, string>((System.Enum)val, val.ToString());
                enumValList.Add(kvpair);
            }

            return enumValList;
        }
    }
}
