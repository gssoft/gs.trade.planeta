using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace GS.WorkTasks.Works
{
    public interface IWork
    {
        Func<bool> InitFunc { get; }
        Func<bool> MainFunc { get; }
        Func<bool> FinishFunc { get; }

        bool Init();
        bool Main();
        bool Finish();
        bool Do();

    }

    public interface IWork<in TInput>
    {
        bool Init();
        bool Main(TInput t);
        bool Finish();

    }

    public interface IWork2
    {
        //Func<bool> InitFunc { get; }
        //Func<bool> MainFunc { get; }
        //Func<bool> FinishFunc { get; }

        bool Init();
        bool Main();
        bool Finish();
        bool Do();

    }
    public interface IWork3
    {
        //Func<bool> InitFunc { get; }
        //Func<bool> MainFunc { get; }
        //Func<bool> FinishFunc { get; }

        bool Init();
        bool Main();
        bool Finish();
        bool Do();

        IWorkTask2 WorkTask { get; }

    }
}
