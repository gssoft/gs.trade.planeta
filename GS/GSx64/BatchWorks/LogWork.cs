using System.Xml.Serialization;
using GS.Elements;
using GS.Interfaces;

namespace GS.BatchWorks
{
    public class LogWork : Element1<string>, IBatchWorkItem
    {
        public string Subject { get; set; }   

        public override string Key => 
            string.Join("@", Parent != null ? Parent.FullCode : "", FullCode);

        [XmlIgnore]
        public BatchWorks.BatchWork Works { get; set; }

        public void DoWork()
        {
            Works.EvlMessage(EvlResult.INFO, Subject, Description);
        }
    }
}
