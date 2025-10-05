using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GS.WorkTasks
{
    public class WorkTaskManager : WorkTask
    {
        private readonly ConcurrentDictionary<string, IWorkTask> _workTasks;

        public IEnumerable<IWorkTask> WorkTasks => _workTasks.Values.ToList();

        public WorkTaskManager()
        {
            _workTasks = new ConcurrentDictionary<string, IWorkTask>();
            TaskFunc = () =>
            {
                ProcessTask();
                return true;
            };
        }
        public IWorkTask Register(IWorkTask tw)
        {
            IWorkTask t;
            if (_workTasks.TryGetValue(tw.Key, out t))
                return t;

            _workTasks.TryAdd(tw.Key,tw);
            return tw;
        }

        private void ProcessTask()
        {
            foreach (var t in WorkTasks.Where(t=>t.IsEnabled))
            {
                if (t.IsNoActive)
                {
                    t.Start();
                    continue;
                }
                if (t.IsUpToDate)
                {
                    t.Stop();
                    continue;
                }
            }
            
            return;
        }

        private void ReStart(Task t)
        {
            t.Start();
        }
    }
}
