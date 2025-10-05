using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;

namespace FtpRequestParse_02.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ExceptionToConsole(this Exception e)
        {
            ConsoleSync.WriteLineT(e.ExceptionMessageAgg());
        }
    }
}
