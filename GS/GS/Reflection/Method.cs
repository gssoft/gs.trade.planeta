using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Reflection
{
    public class Method
    {
        public static string GetTypeFullName(Type t)
        {
            return t.FullName;
        }

        public static string GetMethodName()
        {
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }
        public static string GetMethodName2()
        {
            return System.Reflection.MethodInfo.GetCurrentMethod().Name;
        }
    }
}
