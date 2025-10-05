using System.Reflection;

namespace GS
{
    public static class MyName
    {
        public static string Is()
        {
            return MethodBase.GetCurrentMethod()?.Name + "()";
        }
    }
    public static class MethodName
    {
        public static string Is()
        {
            return MethodBase.GetCurrentMethod()?.Name + "()";
        }
    }
}
