using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.EventHubs.EventHubT1;
using GS.EventHubs.Interfaces;
using GS.Extension;
using GS.Interfaces;

namespace GS.EventHubs
{
    public interface IHaveIndex<out T>
    {
        T this[int index] { get; }
    }

    public class Message : IHaveContent<List<string>>, IHaveIndex<string>
    {
        public Message(List<string> content)
        {
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Count > 1;
        public string this[int i] => Content[i];
        public string Key => Content[0].TrimUpper();
        public int Count => Content.Count;
        public List<string> Content { get; set; }
    }
    public class MessageStr : IHaveContent<string[]>
    {
        public MessageStr(string[] content)
        {
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string Key => Content[0];
        public int Count => Content.Length;
        public string[] Content { get; set; }
    }

    //public class Message<T1,T2> : IHaveContent<T1>
    //    where T1 : IList<T1>
    //    where T2 : IHaveKey<string>
    //{
    //    public string Key => Content[0];
    //    public int Count => Content.Count();
    //    public T1 Content { get; set; }
    //}
    public class Message<T1> : IHaveContent<T1>
       where T1 : IList<string>
    {
        public string Key => Content[0];
        public int Count => Content.Count;
        public T1 Content { get; set; }
    }
}
