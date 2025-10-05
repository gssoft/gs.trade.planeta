using System;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;

namespace GS.ProcessTasks
{
    public class UnitOfWork : Element1<string>
    {
        public string ProcessKey { get; set; }
        public override string Key => 
            string.Join(".", ProcessKey.TrimUpper(), Category.TrimUpper(), Entity.TrimUpper());

        public Action<IEventArgs1> Action { get; set; }
        public IEventArgs1 EventArgs { get; set; }

        public UnitOfWork(){}

        public UnitOfWork(string proccessKey, string category, string entity,
            Action<IEventArgs1> action, IEventArgs1 arg)
        {
            ProcessKey = proccessKey;
            Category = category;
            Entity = entity;

            Action = action;
            EventArgs = arg;
        }
        public void DoUnitOfWork()
        {
            if(EventArgs != null)
                Action(EventArgs);

            Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                            GetType().ToString(),
                            "UnitOfWork",
                            System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "EventArgs IS NULL", ToString());                            
        }

        public UnitOfWork Clone()
        {
            return new UnitOfWork
            {
                ProcessKey = ProcessKey,
                Category = Category,
                Entity = Entity,
                Action = Action,
                EventArgs = EventArgs
            };
        }
        public override string ToString()
        {
            return $"Type: {GetType().FullName}, Key: {Key}, EventArgs: {EventArgs}";
        }
    }
}
