using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.Elements;
using GS.Interfaces;

namespace GS.EventLog.DataBase1.Dal.BatchWorks
{
    public class EvlContextWorks : Element1<string>, IBatchWorkItem
    {
        public override string Key
        {
            get { return Code; }
        }

        public void DoWork()
        {
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "ClearItemsExceptTwoDaysFromAll()", "Start", "", "");
                ClearItemsExceptTwoDaysFromAll();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode, "ClearItemsExceptTwoDaysFromAll()", "Complete", "", "");
             
            }
            catch (Exception ex)
            {
                SendException(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                // throw new Exception(ex.Message);
            }
        }
        private void ClearItemsExceptTwoDaysFromAll()
        {
            using (var evl = new EvlContext1())
            {
                var evlisCnt = evl.EventLogItems.Count();
                evl.ClearItemsExceptTwoDaysFromAll();
                var cnt = evlisCnt - evl.EventLogItems.Count();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode,
                    "ClearItemsExceptTwoDaysFromAll()", cnt + " records Removed", "", "");

            }
        }
    }
}
