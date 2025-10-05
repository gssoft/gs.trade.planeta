using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Interfaces
{
    public interface ITimePlans
    {
        void Init(IEventLog evl);
    }

    public interface ITimePlan
    {
        bool Enabled { get; }
    }

    public interface ITimePlanEventArgs
    {
        TimePlanEventType EventType { get; }
        string TimePlanCode { get; }
        string TimePlanItemCode { get; }
        string TimePlanItemEventCode{ get; }
        //  public readonly string Code;
        string Msg{ get; }
        //TimeSpan StartTime { get; }
        //TimeSpan EndTime { get; }
    }

    public interface ITimePlanItemEventArgs : ITimePlanEventArgs
    {
       TimeSpan StartTime { get;}
       TimeSpan EndTime { get;}
    }

    public enum TimePlanEventType
    {
        TimePlanItemEvent = 1,
        TimePlanItem = 2,
        TimePlan = 3
    } ;
}
