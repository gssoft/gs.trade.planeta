using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.WorkTasks.Works
{
    public class Works : IWork
    {
        public Func<bool> InitFunc { get; set; }
        public Func<bool> MainFunc { get; set; }
        public Func<bool> FinishFunc { get; set; }

        public bool Init()
        {
            throw new NotImplementedException();
        }

        public bool Main()
        {
            throw new NotImplementedException();
        }

        public bool Finish()
        {
            throw new NotImplementedException();
        }

        public bool Do()
        {
            throw new NotImplementedException();
        }

        public bool IsEnaled { get; set; }
    }
}
