using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces;
using GS.Works;


namespace GS.WorkTasks
{
    public interface IWorkTasks
    {
        void Start();
        void Stop();
    }
    public interface IWorkTasks4
    {
        void Start();
        void Stop();
        void DoWork();
    }

    public interface IWorkTask: Containers5.IHaveKey<string>
    {
        Task Task { get; }
        bool IsEnabled { get; set; }
        bool IsActive { get; }
        bool IsNoActive { get; }
        bool IsUpToDate { get; }
        int ErrorCount { get; }
        int ErrorCountToStop { get; }

        void Start();
        void Stop();
        void DoWork();
        void TryToDoWork();
        Func<bool> TaskFunc { get; set; } 
    }

    public interface IWorkTask2 : Containers5.IHaveKey<string>
    {
        Task Task { get; }
        bool IsEnabled { get; set; }
        bool IsActive { get; }
        bool IsNoActive { get; }
        bool IsUpToDate { get; }
        int ErrorCount { get; }
        int ErrorCountToStop { get; }

        void Start();
        void Stop();
        void DoWork();
        void TryToDoWork();
        Work Work { get; set; }
    }

    public interface IWorkTask3 : IElement1<string>, IHaveInit // Containers5.IHaveKey<string>
    {
        Task Task { get; }
        bool IsActive { get; }
        bool IsNoActive { get; }
        bool IsUpToDate { get; }
        int ErrorCount { get; }
        int ErrorCountToStop { get; }

        void Start();
        void Stop();
        void DoWork();
        void TryToDoWork();
        IWorks Works { get; set; }
    }

    public interface IPeriodicallyWorkTask : IWorkTask
    {
        void Suspend();
        void Resume();
    }

}
