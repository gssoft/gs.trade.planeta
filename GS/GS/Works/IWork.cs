using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces;
using GS.WorkTasks;

namespace GS.Works
{
    public interface IHaveWork<in T>
    {
        IWork1<T> Work { get; }
    }

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

    public interface IWorkBase: IElement1<string>, IHaveInit
    {
        void Init();
        bool InitWorks();
        bool Main();
        bool Finish();
        bool Do();
        string Message { get; }

        IWorkTask3 WorkTask { get; set; }
    }

    public interface IWork1<in T> : IWorkBase
    {
        void EnQueue(T t);
        bool DoWork();
        //IWorkTask3 WorkTask { get; set; }

    }

    public interface IWorks : IElement2<string, IWorkBase>, IWorkBase
    {

    }

    public interface IWork2<T> : IElement1<T>, IHaveInit
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
