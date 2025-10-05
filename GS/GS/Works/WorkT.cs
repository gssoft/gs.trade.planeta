using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;
using GS.Works;

namespace GS.Works
{
    public class Work<TInput> : Element1<string>, IWork< TInput>
    {
        public Func<bool> InitFunc { get; set; }
        public Func<TInput, bool> MainFunc { get; set; }
        public Func<bool> FinishFunc { get; set; }

        public bool Init()
        {
            if (InitFunc == null)
                return false;
            try
            {
                return InitFunc();
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
            return false;
        }
        public bool Main(TInput t)
        {
            if (MainFunc == null)
                return false;
            try
            {
                return MainFunc(t);
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
            return false;
        }

        public bool Finish()
        {
            if (FinishFunc == null)
                return false;
            try
            {
                return FinishFunc();
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
            return false;
        }
        public override string Key => Code.HasValue() ? Code : "Unknown";
    }
}
