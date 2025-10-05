using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Extension;
using GS.WorkTasks;

namespace GS.Works
{
    public class Works : Element2<string, IWorkBase, DictionaryContainer<string, IWorkBase>>, IWorks
    {
        public IWorkTask3 WorkTask { get; set; }
        public string Message { get; private set; }
        public Works()
        {
            Collection = new DictionaryContainer<string, IWorkBase>();
        }
        public override void Init()
        {
            try
            {
                foreach (var i in Items)
                {
                    i.InitWorks();
                }
            }
            catch(System.Exception e)
            {
                SendException(e);
            }
        }
        public bool InitWorks()
        {
            foreach (var i in Items)
            {
                i.InitWorks();
            }
            return true;
        }

        public bool Main()
        {
            var items = Items.Where(i => i.IsEnabled);
            foreach (var i in items)
            {
                var r = i.Main();
                //if (!r)
                //    i.IsEnabled = false;
            }
            return true;
        }

        public bool Finish()
        {
            foreach (var i in Items)
            {
                i.Finish();
            }
            return true;
        }

        public bool Do()
        {
            Main();
            return true;
        }

        #region Element2 Overrides
        public override IWorkBase Register(IWorkBase item)
        {
            if (item == null)
                return null;
            item.WorkTask = WorkTask;
            return Collection.AddOrGet(item);
        }
        #endregion

        public override string Key => Code.HasValue() ? Code : GetType().FullName;
    }
}
