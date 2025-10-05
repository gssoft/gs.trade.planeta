using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using GS.Extension;
using GS.Moex.Interfaces;

namespace Moex
{
    //public interface IItemHolder<T>
    //{
    //    List<T> Items { get; }
    //    void Clear();
    //}
    //public interface IHaveInit
    //{
    //    void Init();
    //}

    public partial class MoexTest
    {
        public XElement GetXmlElementFromXDoc(XNode xdoc, string xpath)
        {
            return xpath.HasNoValue() ? null : xdoc?.XPathSelectElement(xpath);
        }
        private static Array EnumToList<T>()
        {
            // Type enumType = typeof(T);
            return Enum.GetValues(typeof (T));
        }
        public void InitItems<T>(IList<T> items) where T : IHaveInit
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            //IHaveInit current = default(T);
            foreach (var i in items)
            {
                // current = i;
                try
                {
                    i.Init();
                }
                catch (Exception e)
                {
                    SendErrMessage(method, e.ToString());
                    var typStr = typeof(T).ToString();
                    SendErrMessage($"{method} {typStr}", i.ToString());
                }
            }
        }
        public IList<TItem> UnPackHolder<THolder, TItem>(THolder holder) 
            where THolder : IItemHolder<TItem> 
            where TItem : IHaveInit
        {
            if (!holder.Items.Any()) return new List<TItem>();
            var lst = holder.Items;
            holder.Clear();
            InitItems(lst);
            return lst;
        }
        public IList<TItem> GetItems<THolder, TItem>(string board)
            where THolder : IItemHolder<TItem>
            where TItem : IHaveInit
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                var xnode = GetTickersXmlDoc(board); // Get XDocument
                if (xnode == null) return null;
                var tickersxmlrows = GetXmlElementFromXDoc(xnode, TickersRowsXPath); // XPath to Ticker Rows XElement
                if (tickersxmlrows == null) return null;
                if (!tickersxmlrows.HasElements) return new List<TItem>();
                var holder = Deserialize<THolder>(tickersxmlrows);
                return holder == null ? null : UnPackHolder<THolder, TItem>(holder);
            }
            catch (Exception e)
            {
                SendErrMessage(method, typeof(TItem).ToString(), e.ToString());
            }
            return null;
        }
    }
}
