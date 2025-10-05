using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.WorkTasks;

namespace Test_CaWorkTask_01.Task
{
    public class WorkTasks4 : Element2<string, IWorkTask3, DictionaryContainer<string, IWorkTask3>>, IWorkTasks4
    {
        public WorkTasks4()
        {
            Collection = new DictionaryContainer<string, IWorkTask3>();
        }

        //public override void Init(IEventLog evl)
        //{
        //    base.Init(evl);
        //}

        public override string Key => 
            Code.HasValue() ? FullCode : TypeFullName;

        public void Start()
        {
            foreach (var t in Items)
                t.Start();
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, 
                "Start Tasks", "Complete", ToString());
        }

        public void Stop()
        {
            // 15.12.13
            //var l = Items.Select(i => i.Task).ToArray();
            foreach (var t in Items)
                t.Stop();
            // 15.12.13
            // Task.WaitAll(l);   only for No Cancelation Token Tasks

            Evlm2(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY,
                "Stop Tasks", "Complete", ToString());
        }

        public void DoWork()
        {
            foreach (var t in Items)
                t.DoWork();

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                "Tasks.DoWork", "Complete", ToString());
        }

        public override IWorkTask3 Register(IWorkTask3 item)
        {
            item.Parent = this;
            return base.Register(item);
        }
    }
}
