using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace SerializeDto.Dto
{
    [Serializable]
    public class TcpDataClass
    {
        public string RouteKey { get; set; }
        public string Data { get; set; }

        public bool Compare(TcpDataClass dto)
        {
            return RouteKey == dto.RouteKey && Data == dto.Data;
        }
    }

    //[Serializable]
    //public class MessageList
    //{
    //    public string Key => Content[0];
    //    public List<string> Content;
    //}
    [Serializable]
    public class MessageLst
    {
        public MessageLst(List<string> content)
        {
            Content = content;
        }
        public bool IsValid => Content != null && Content.Count > 0;
        public string Key => IsValid ? Content[0].TrimUpper() : "Invalid";
        public int Count => IsValid ? Content.Count : 0;
        public List<string> Content { get; set; }
    }
    [Serializable]
    public class MessageStr
    {
        public MessageStr(string[] content)
        {
            Content = content;
        }
        public bool IsValid => Content != null && Content.Length > 0;
        public string Key => IsValid ? Content[0].TrimUpper() : "Invalid";
        public int Count => IsValid ? Content.Length : 0;
        public string[] Content { get; set; }
    }

}
