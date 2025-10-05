using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;

namespace caTest_TaskResult01
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var res = TaskAsyncOne();
                //var res = TaskAsyncOne().Result;
                var task = TaskAsyncOne();
                var res = task.Result;
                if(task.Exception != null)
                    ConsoleAsync.WriteLineT("Main catch exception:{0}", task.Exception.ExceptionMessage() );
                ConsoleAsync.WriteLineT("Main exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                ConsoleAsync.WriteLineT(e.Message);
               // throw;
            }
        }

        private static Task<bool> TaskAsyncOne()
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                //try
                //{
                    ConsoleAsync.WriteLineT("Task is Started");
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                  //  throw new NullReferenceException("NullException");
                for (int i = -5; i < 1; i++)
                {
                    var a = 100 / i;
                    ConsoleAsync.WriteLineT(i.ToString());
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                //}
                //catch (Exception e)
                //{
                //    ConsoleAsync.WriteLineT("Exception from Task: " + e.Message);
                //    return false;
                //}
                Thread.Sleep(TimeSpan.FromSeconds(5));
                return true;
            });
//            return true;
        }

        private static async Task<bool> TaskTwoAsync()
        {

            return true;
        }
    }
}
