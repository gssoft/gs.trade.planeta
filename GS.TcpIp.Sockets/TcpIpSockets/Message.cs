using System.Collections.Generic;
using System.Linq;
using GS.EventHubs.Interfaces;
using GS.Extension;

namespace TcpIpSockets
{
    public class Message1<TContent> : IHaveContent<TContent>
    {
        public Message1(TContent content)
        {
            Content = content;
        }
        public string Key { get; }
        public TContent Content { get; set; }
        public int Count { get; }
    }

    public class Message<TContent> : IHaveContent<TContent>, IHaveIndex<string>
    {
        public Message(TContent content)
        {
            Content = content;
        }
        public string Key { get; }
        public TContent Content { get; set; }
        public int Count { get; }

        public string this[int index]
        {
            get { throw new System.NotImplementedException(); }
        }
    }

    public class MessageLstStr : IHaveContent<List<string>>, IHaveIndex<string>
    {
        public MessageLstStr(List<string> content)
        {
            Content = content;
        }
        public MessageLstStr(string[] content)
        {
            Content = new List<string>();
            Content.AddRange(content);
        }
        // public bool IsEmpty => Content != null && Content.Count > 1;
        public string this[int i] => Content[i];
        public string Key => Content[0].TrimUpper();
        public int Count => Content.Count;
        public List<string> Content { get; set; }
    }
    public class MessageStrArr : IHaveContent<string[]>, IHaveIndex<string>
    {
        public MessageStrArr(string[] content)
        {
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string this[int i] => Content[i];
        public string Key => Content[0];
        public int Count => Content.Length;
        public string[] Content { get; set; }
    }
    public class MessageStr : IHaveContent<string>
    {
        public MessageStr(string key, string content)
        {
            Key = key;
            Content = content;
        }
        // public bool IsEmpty => Content != null && Content.Length > 1;
        public string Key { get; }
        public int Count => Content.Length;
        public string Content { get; set; }
    }

    public class MessageStrIndexed : IHaveIndex<string>
    {
        public MessageStrIndexed(string[] strings)
        {
            Content = strings;
        }
        public string this[int i] => Content[i];
        public string[] Content { get; set; }
    }
    public class MessageLstStrIndexed : IHaveIndex<string>
    {
        public MessageLstStrIndexed(string[] strings)
        {
            Content = new List<string>();
            Content.AddRange(strings);
        }
        public MessageLstStrIndexed(List<string> strings)
        {
            Content = new List<string>();
            Content.AddRange(strings);
        }
        public string this[int i] => Content[i];
        public List<string> Content { get; set; }
    }



}
