using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;

namespace GS.WorkTasks
{
    public class WorkTasks : Element2<string, IWorkTask3, Containers5.DictionaryContainer<string, IWorkTask3>>, IWorkTasks
    {
        public WorkTasks()
        {
            Collection = new DictionaryContainer<string, IWorkTask3>();
        }

        //public override void Init(IEventLog evl)
        //{
        //    base.Init(evl);
        //}

        public override string Key
        {
            get { return Code.HasValue() ? FullCode : GetType().FullName; }
        }

        public void Start()
        {
            foreach (var t in Items)
                t.Start();
        }

        public void Stop()
        {
            // 15.12.13
            //var l = Items.Select(i => i.Task).ToArray();
            foreach (var t in Items)
                t.Stop();
            // 15.12.13
            // Task.WaitAll(l);   only for No Cancelation Token Tasks

            Evlm2(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, FullCode, Code,                                        
                "Stop() Tasks", "Complete","");

        }
    }
}
