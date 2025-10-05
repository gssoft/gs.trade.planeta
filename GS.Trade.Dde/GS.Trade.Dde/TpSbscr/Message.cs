using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventHubs.Interfaces;

namespace GS.Trade.Dde.TpSbscr
{
    public class MessageStr : IHaveContent<string>
    {
        public MessageStr(string key, string content)
        {
            Key = key;
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string Key { get;}
        public int Count => Content.Length;
        public string Content { get; set; }
    }
    public class MessageStrArr : IHaveContent<string[]>
    {
        public MessageStrArr(string[] content)
        {
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string Key => Content[0];
        public int Count => Content.Length;
        public string[] Content { get; set; }
    }
    public class MessageListStr : IHaveContent<List<string>>
    {
        public MessageListStr(List<string> content)
        {
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string Key => Content[0];
        public int Count => Content.Count;
        public List<string> Content { get; set; }
    }
}
