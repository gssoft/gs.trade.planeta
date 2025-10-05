using System.IO;
using System.Xml.Serialization;

namespace GS.Trade.QuoteDownLoader.Works
{
    public interface IWorkItem
    {
        void DoWork();
        string Name { get; set; }
        bool IsEnabled { get; set; }
        [XmlIgnore]
        WorkContainer Works { get; set; }
    }
}
