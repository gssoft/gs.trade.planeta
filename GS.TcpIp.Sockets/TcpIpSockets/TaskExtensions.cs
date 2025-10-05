using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Tasks;

namespace TcpIpSockets
{
    public static class TaskExtensions
    {
        public static bool IsFinished(this Task task)
        {
            return task.IsCompleted || task.IsCanceled || task.IsFaulted;
        }
        public static void WaitingForTaskCompleting(this Task task, int timeInteravlaInSeconds)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            while (!task.IsFinished() && timeInteravlaInSeconds-- > 0)
            {  
                ConsoleSync.WriteLineT($"{m} Wait for Completing {task.StatusStr()}");
                Thread.Sleep(1000);
            }
            ConsoleSync.WriteLineT($"{m} Task is Finished {task.StatusStr()}");
            task.Dispose();   
        }
    }
}
