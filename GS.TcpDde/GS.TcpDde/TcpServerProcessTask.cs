using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using GS.ProcessTasks;

namespace GS.TcpDde
{
    public partial class TcpDdeServer01
    {
        public bool IsProcessTaskInUse { get; set; }
        public ProcessTasks.ProcessTask<IList<string>> ProcessTask { get; private set; }

        private void SetupProcessTask()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (!IsProcessTaskInUse)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                $"{m}", "ProcessTask Will NOT BE USED", ToString());
                return;
            }
            ProcessTask = new ProcessTask<IList<string>>();
            ProcessTask.Init();
            ProcessTask.TimeInterval = 1000;
            ProcessTask.Parent = this;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemsProcessingAction = QuoteListsOfStringsProcessing;
            //ProcessTask.ItemProcessingAction = QuoteListsOfStringsProcessing;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, Name,
                m, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        private void QuoteListsOfStringsProcessing(IEnumerable<IList<string>> listofquotes)
        {
            foreach (var i in listofquotes)
            {
                if (!i.Any()) continue; // list is empty
                var strarr = ToTcpQuotes2(i);
                TcpServer?.SendMessage(strarr);
            }
        }
        public string[] ToTcpQuotes(IList<string> ss)
        {
            // if (ss.Any()) return new string[];
                
            var strarr = new[] { "Quotes", ss[0], ""};
            //ss.RemoveAt(0); // Remove Topic
            //foreach (var i in ss)
                //strarr[2] += i + Environment.NewLine;
            var cnt = ss.Count;
            for(var i = 1; i<cnt; i++)
                strarr[2] += ss[i] + Environment.NewLine;            
            return strarr;
        }
        public string[] ToTcpQuotes1(IList<string> ss)
        {
            // if (ss.Any()) return new string[];
            try
            {
                var strarr = new[] {"Quotes", ss[0]};

                var cnt = ss.Count;
                Array.Resize(ref strarr, 2 + cnt - 1);
                for (var i = 1; i < cnt; i++)
                    strarr[i + 1] = ss[i] + Environment.NewLine;
                return strarr;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return null;
        }
        public string[] ToTcpQuotes2(IList<string> ss)
        {
            // if (ss.Any()) return new string[];
            try
            {
                var i = 0;
                var strarr = new string[ss.Count];
                foreach (var s in ss)
                    //strarr[i++] = s + Environment.NewLine;
                    strarr[i++] = s;
                return strarr;
            }
            catch (Exception e)
            {
                SendException(e);
            }
            return null;
        }
        public List<string> ListToTcpQuotes(IList<string> ss)
        {
            var strlst = new List<string> { "Data", "Quotes"};
            strlst.AddRange(ss);
            return strlst;
        }
    }
}
