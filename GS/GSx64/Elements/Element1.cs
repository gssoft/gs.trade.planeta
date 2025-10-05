using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Events;

namespace GS.Elements
{
    public interface IElement1<TKey> : Containers5.IHaveKey<TKey>
    {
        IEventLog EventLog { get; set; }
        event EventHandler<Events.IEventArgs> ChangedEvent;
        void OnChangedEvent(IEventArgs e);
        void FireChangedEvent(string category, string entity, string operation, object o);
        event EventHandler<IEventArgs> ExceptionEvent;
        void OnExceptionEvent(IEventArgs e);
        void OnExceptionEvent(Exception e);

        void SendExceptionMessage3(string source, string objtype, string operation, string objstr, Exception e);
        void SendException(Exception e);

        bool IsEnabled { get; }
        bool IsEvlEnabled { get; }

        //bool IsQueueEnabled { get; }

        string Alias { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        string FullCode { get;  }
        string FullName { get; }
        string Description { get; set; }

        string Category { get; }
        string Entity { get; }

        IElement1<TKey> Parent { get; set; }
        string ParentName { get; }
        string ParentTypeFullName { get; }
        string ParentTypeName { get; }
        string TypeFullName { get; }
        string TypeName { get; }
        string FullInfo { get; }
        string ShortInfo { get; }


        void WhoAreYou();

        void Evlm(EvlResult res, EvlSubject subj,
                    string source, string entity, string operation, string description, string obj);
        void Evlm1(EvlResult res, EvlSubject subj,
            string source, string entity, string operation, string description, string obj);
        void Evlm2(EvlResult res, EvlSubject subj,
                    string source, string entity, string operation, string description, string obj);

        void Evlm51(EvlResult res, EvlSubject subj, string operation, string description, string obj);
        void Evlm52(EvlResult res, EvlSubject subj, string operation, string description, string obj);
        //void Evlm61(EvlResult res, EvlSubject subj, string operation, string description, string obj);
        //void Evlm62(EvlResult res, EvlSubject subj, string operation, string description, string obj);
    }

    // public abstract class Element1<TKey> : IHaveInit, IElement1<TKey> //, IElement1
    public abstract class Element1<TKey> : IHaveInit<IEventLog>, IElement1<TKey> //, IElement1
    {
        public bool IsEnabled { get; set; }
        public bool IsEvlEnabled { get; set; }

        //public bool IsQueueEnabled { get; set; }

        public string Alias { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }
        public string Entity { get; set; }

        public string ParentName => Parent == null ? "" : Parent.Name;
        public string FullName => string.Join("@", GetType().FullName, Name);
        public string ParentCode => Parent == null ? "" : Parent.Code;
        public string FullCode => string.Join("@", GetType().FullName, Code);
        public abstract TKey Key { get; }

        public string ParentTypeFullName => Parent?.GetType().FullName ?? ParentObj?.GetType().FullName ?? GetType().FullName;
        public string ParentTypeName => Parent?.GetType().Name ??  ParentObj?.GetType().Name ?? GetType().Name;
        public string TypeFullName => GetType().FullName;
        public string TypeName => GetType().Name;
        public string FullInfo => $"{TypeFullName}, Parent: {ParentTypeFullName}, Code: {Code}, Name: {Name}, Key: {Key}";
        public string ShortInfo => $"{TypeName}, Parent: {ParentTypeName}, Code: {Code}, Name: {Name}, Key: {Key}";

        public string ParentAndMyTypeName => ParentTypeName != TypeName ? $"{ParentTypeName}.{TypeName}" : TypeName;
        private IElement1<TKey> _parent;
        //[XmlIgnore]
        //public IElement1<TKey> Parent
        //{
        //    get { return _parent; }
        //    set
        //    {
        //        if (value == null) return;
        //        _parent = value;

        //        if (_parent.EventLog == null) return;
        //        EventLog = _parent.EventLog;
        //    }
        //}
        [XmlIgnore]
        public IElement1<TKey> Parent { get; set; }

        [XmlIgnore]
        public object ParentObj { get; set; }
        [XmlIgnore]
        public IEventLog EventLog { get;  set; }

        //[XmlIgnore]
        //public IEventLog TradeStorageEventLog { get; set; }

        //[XmlIgnore]
        //public IEventLog TradeTerminalsEventLog { get; set; }

        public event EventHandler<IEventArgs> ChangedEvent;
        public virtual void OnChangedEvent(IEventArgs e)
        {
            if (Parent == null)
            {
                EventHandler<IEventArgs> handler = ChangedEvent;
                handler?.Invoke(this, e);
            }
            else
                Parent.OnChangedEvent(e);

            // 19.08.25
            //EventHandler<IEventArgs> handler = ChangedEvent;
            //handler?.Invoke(this, e);

            //Parent?.OnChangedEvent(e);

        }
        public event EventHandler<IEventArgs> ExceptionEvent;
        public virtual void OnExceptionEvent(IEventArgs e)
        {
            if (Parent == null)
            {
                EventHandler<IEventArgs> handler = ExceptionEvent;
                handler?.Invoke(this, e);
            }
            else
                Parent.OnExceptionEvent(e);
        }

        protected bool IsAnyExceptionEventSubscriber { get {
                EventHandler<IEventArgs> handler = ExceptionEvent;
                return handler != null;
            }
        }
        public virtual void OnExceptionEvent(Exception e)
        {
            var ea = new Events.EventArgs
            {
                Sender = this,
                Category = "Exceptions",
                Entity = "Exception",
                Operation = "New",
                Object = e,
                IsHighPriority = true,
            };
            if (Parent == null)
            {
                EventHandler<IEventArgs> handler = ExceptionEvent;
                handler?.Invoke(this, ea);
            }
            else
                Parent.OnExceptionEvent(ea);
        }

        public virtual void ChangedEventHandler(object sender, IEventArgs args)
        {
        }

        public virtual void Init(){}
        public virtual void Init(IEventLog eventLog)
        {
            try
            {
                if (eventLog == null)
                {
                    throw new NullReferenceException($"NullReferenceException: {FullInfo}");
                }
                EventLog = eventLog;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(Code, "Init(Evl==null)", "", "", e);
                // throw;
            }
        }

        public void WhoAreYou()
        {
            //EvlmPr(EvlResult.INFO,EvlSubject.DIAGNOSTIC, ParentTypeFullName,
            //                TypeFullName, "WhoAreYou()", "I'am = " + FullName.AddRight(" ", Name, Code), ToString());
            EvlmPr(EvlResult.INFO, EvlSubject.DIAGNOSTIC, ParentTypeFullName, TypeFullName,
                            "WhoAreYou()", $"I'am: {FullInfo}", ToString());
            try
            {
                var e = new NullReferenceException("I Have Exceptions Handling for You");
                SendExceptionMessage3(ParentTypeFullName, TypeFullName, "WhoAreYou()", ToString(), e);
            }
            catch (Exception e)
            {
                SendExceptionMessage3(ParentTypeFullName, TypeFullName, "WhoAreYou -- Failure", ToString(), e);
            }
        }

        public void FireChangedEvent(string category, string entity, string operation, object o)
        {
            var ea = new Events.EventArgs
            {
                Category = category,
                Entity = entity,
                Operation = operation,
                Object = o
            };
            //if (Parent == null)
            //    OnStorageChangedEvent(ea);
            //else
            //    Parent.OnStorageChangedEvent(ea);
            OnChangedEvent(ea);
        }
        public void FireChangedEvent(string operation, object o)
        {
            var ea = new Events.EventArgs
            {
                Category = Category,
                Entity = Entity,
                Operation = operation,
                Object = o
            };
            //if (Parent == null)
            //    OnStorageChangedEvent(ea);
            //else
            //    Parent.OnStorageChangedEvent(ea);
            OnChangedEvent(ea);
        }
        protected void SendException(object ve, string method, Exception e)
        {
            var type = ve == null ? "Object" : ve.GetType().FullName;
            var entity = ve == null ? "Object" : ve.ToString();
            SendExceptionMessage3(FullCode, type, method, entity, e);
        }

        public void SendExceptionMessage3(string source,
                                            string objtype, string operation, string objstr,
                                            Exception e)
                                            // string message, string excSource, string exctype, string targetSite)
        {
            if (e == null)
                return;
            if (e.InnerException == null)
            {
                var ea = new Events.EventArgs
                {
                    Category = "UI.Exceptions",
                    Entity = "Exception",
                    Operation = "Add",
                    IsHighPriority = true,
                    Object = new GSException
                    {
                        Source = source,
                        ObjType = objtype,
                        Operation = operation,
                        ObjStr = objstr,
                        Message = e.Message,
                        ExcType = e.GetType().ToString(),
                        SourceExc = e.Source ?? "",
                        TargetSite = e.TargetSite?.ToString() ?? "",
                        StackTrace = e.StackTrace ?? ""
                    }
                };
                OnExceptionEvent(ea);
               // if (!(this is IEventLog))
                // Warning may be recursing
                //if (this is IEventLog)
                //    return;
                
                if (EventLog == null && Parent == null)
                    return;

                Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, objtype, operation,
                                                                e.Message, e.ToString());
                return;
            }
            var lst = new List<GSException>
            {
                new GSException
                {
                    Source = source,
                    ObjType = objtype,
                    Operation = operation,
                    ObjStr = objstr,
                    Message = e.Message,
                    ExcType = e.GetType().ToString(),
                    SourceExc = e.Source ?? "",
                    TargetSite = e.TargetSite?.ToString() ?? "",
                    StackTrace = e.StackTrace ?? ""
                }
            };
            var exc = e;
            while (exc.InnerException != null)
            {
                var ex = exc.InnerException;
                lst.Add(new GSException
                {
                    Source = source,
                    ObjType = objtype,
                    Operation = operation,
                    ObjStr = objstr,
                    Message = ex.Message,
                    ExcType = ex.GetType().ToString(),
                    SourceExc = ex.Source ?? "",
                    TargetSite = ex.TargetSite?.ToString() ?? "",
                    StackTrace = ex.StackTrace ?? ""
                });
                exc = ex;
            }
            var earg = new Events.EventArgs
            {
                Category = "UI.Exceptions",
                Entity = "Exception",
                Operation = "AddMany",
                IsHighPriority = true,
                Object = lst
            };
            OnExceptionEvent(earg);
            // Warning may be recursing
            //if (this is IEventLog)
            //    return;
            if (EventLog == null && Parent == null)
                return;
            foreach(var x in lst)
                Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, objtype, operation, x.Message, x.ToString());
                // Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, source, objtype, operation, x.ToString(), o);
        }
        public void SendException(Exception e)
        // string message, string excSource, string exctype, string targetSite)
        {
            if (e == null)
                return;
            if (e.InnerException == null)
            {
                var ea = new Events.EventArgs
                {
                    Category = "UI.Exceptions",
                    Entity = "Exception",
                    Operation = "Add",
                    IsHighPriority = true,
                    Object = new GSException
                    {
                        Source = e.Source ?? "",
                        ObjType = e.GetType().ToString(),
                        Operation = e.TargetSite?.ToString() ?? "",
                        ObjStr = e.ToString(),
                        Message = e.Message,
                        ExcType = e.GetType().ToString(),
                        SourceExc = e.Source ?? "",
                        TargetSite = e.TargetSite?.ToString() ?? "",
                        StackTrace = e.StackTrace ?? ""
                    }
                };
                OnExceptionEvent(ea);
                // if (!(this is IEventLog))
                // Warning may be recursing
                //if (this is IEventLog)
                //    return;

                if (EventLog == null && Parent == null)
                    return;

                Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                    // e.Source ?? "",
                    ParentAndMyTypeName, 
                    e.GetType().ToString(), e.TargetSite?.ToString() ?? "", e.Message,
                    // e.ToString()
                    e.StackTrace ?? ""
                    );
                return;
            }
            var lst = new List<GSException>
            {
                new GSException
                {
                    Source = e.Source ?? "",
                    ObjType = e.GetType().ToString(),
                    Operation = e.TargetSite?.ToString() ?? "",
                    ObjStr = e.ToString(),
                    Message = e.Message,
                    ExcType = e.GetType().ToString(),
                    SourceExc = e.Source ?? "",
                    TargetSite = e.TargetSite?.ToString() ?? "",
                    StackTrace = e.StackTrace ?? ""
                }
            };
            var exc = e;
            while (exc.InnerException != null)
            {
                var ex = exc.InnerException;
                lst.Add(new GSException
                {
                    Source = ex.Source ?? "",
                    ObjType = ex.GetType().ToString(),
                    Operation = ex.TargetSite?.ToString() ?? "",
                    ObjStr = ex.ToString(),
                    Message = ex.Message,
                    ExcType = ex.GetType().ToString(),
                    SourceExc = ex.Source ?? "",
                    TargetSite = ex.TargetSite?.ToString() ?? "",
                    StackTrace = ex.StackTrace ?? ""
                });
                exc = ex;
            }
            var earg = new Events.EventArgs
            {
                Category = "UI.Exceptions",
                Entity = "Exception",
                Operation = "AddMany",
                IsHighPriority = true,
                Object = lst
            };
            OnExceptionEvent(earg);
            // Warning may be recursing
            //if (this is IEventLog)
            //    return;
            if (EventLog == null && Parent == null)
                return;
            foreach (var x in lst)
                Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                     x.Source ?? "", x.GetType().ToString(), x.TargetSite?.ToString() ?? "", x.Message, x.ToString());
        }

        protected void EvlmPr(EvlResult res, EvlSubject subj,
                                string source, string entity,
                                string operation, string description, string obj)
        {
            try
            {
                //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2} ParentCode: {4}, ParentType: {5}", Code, Name, GetType().FullName, "***** ",
                    //ParentName, Parent != null ? Parent.GetType().FullName : "null");
                if (EventLog != null)
                    EventLog.AddItem(res, subj, source, entity, operation, description, obj);
                else if (Parent != null)
                    ((Element1<string>) Parent).EvlmPr(res, subj, source, entity, operation, description, obj);
                else
                {
                    var str = $"Code: {Code}, Name: {Name}, Type: {GetType().FullName}";
                    var e =
                        new NullReferenceException("EvlmPr(): EventLog isNull && Parent isNull\r" + str);
                    
                    if (IsAnyExceptionEventSubscriber)
                    {
                        SendExceptionMessage3(FullName, "EvlmPr()",
                            "EventLog or Parent is Null or Parent.EventLog is Null", "", e);
                        return;
                    }
                    // throw e;
                    var a = 1;
                }
                //throw new NullReferenceException(FullName + ". EvlmPr(); EventLog or Parent is Null or Parent.EventLog is Null");
                //else if (Parent != null && Parent.EventLog != null)
                //    Parent.EventLog.AddItem(res, subj, source, entity, operation, description, obj);
                //else
                //{
                //    
                //}
            }
            catch (Exception e)
            {
                //SendExceptionMessage2(FullName, GetType().ToString(), "Evlm()", ToString(),
                //                            e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
                throw new NullReferenceException("Element1.EvlmPr() Failure:" + e.Message);
                //throw;
            }
        }
        public void Evlm(EvlResult res, EvlSubject subj,
                                string source, string entity, string operation, string description, string obj)
        {
                EvlmPr(res, subj, source, entity, operation, description, obj);
        }
        public void Evlm(EvlResult res, EvlSubject subj,
                            string operation, string description, string obj, int level)
        {
            EvlmPr(res, subj, ParentTypeFullName, TypeName, operation, description, obj);
        }
        public void Evlm1(EvlResult res, EvlSubject subj,
                                string source, string entity, string operation, string description, string obj)
        {
            if (IsEvlEnabled)
                EvlmPr(res, subj, source, entity, operation, description, obj);
        }
        public void Evlm51(EvlResult res, EvlSubject subj,
                                 string operation, string description, string obj)
        {
            if (IsEvlEnabled)
                EvlmPr(res, subj, ParentTypeFullName, TypeFullName, operation, description, obj);
        }
        public void Evlm52(EvlResult res, EvlSubject subj,
                                 string operation, string description, string obj)
        {
                EvlmPr(res, subj, ParentTypeFullName, TypeFullName, operation, description, obj);
        }
        //public void Evlm61(EvlResult res, EvlSubject subj,
        //                         string operation, string description, string obj)
        //{
        //    if (IsEvlEnabled)
        //        EvlmPr(res, subj, ParentTypeName, TypeName, operation, description, obj);
        //}
        //public void Evlm62(EvlResult res, EvlSubject subj,
        //                         string operation, string description, string obj)
        //{
        //    EvlmPr(res, subj, ParentTypeName, TypeName, operation, description, obj);
        //}

        public void Evlm2(EvlResult res, EvlSubject subj,
                                string source, string entity, string operation, string description, string obj)
        {
            //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
            EvlmPr(res, subj, source, entity, operation, description, obj);
        }
        public void Evlm1(EvlResult res, EvlSubject subj, 
                string entity, string operation, string description, string obj)
        {
            if (IsEvlEnabled)
                //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
                EvlmPr(res, subj, TypeName, entity, operation, description, obj);
        }
        public void Evlm2(EvlResult res, EvlSubject subj, 
            string entity, string operation, string description, string obj)
        {
            //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
            EvlmPr(res, subj, TypeName, entity, operation, description, obj);
        }
        public void Evlm3(EvlResult res, EvlSubject subj, string entity, 
            string operation, string description, string obj)
        {
            //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
            EvlmPr(res, subj, TypeName, entity, operation, description, obj);
        }
        public void Evlm1(EvlResult res, EvlSubject subj, string operation, string description, string obj)
        {
            if (IsEvlEnabled)
                //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
                EvlmPr(res, subj, ParentTypeName, TypeName, operation, description, obj);
        }
        public void Evlm2(EvlResult res, EvlSubject subj, string operation, string description, string obj)
        {
            //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
            EvlmPr(res, subj, ParentTypeName, TypeName, operation, description, obj);
        }
        public void Evlm3(EvlResult res, EvlSubject subj, string operation, string description, string obj)
        {
            //Debug.Print("{3} Code: {0}, Name: {1}, Type: {2}", Code, Name, GetType().FullName, "***** ");
            EvlmPr(res, subj, ParentTypeName, TypeName, operation, description, obj);
        }
    }
}
