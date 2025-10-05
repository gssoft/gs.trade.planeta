using GS.Elements;

namespace GS.BatchWorks
{
    public interface IBatchWorkItem : IElement1<string>
    {
        void DoWork();
        //string Code { get; set; }
        //string Name { get; set; }
        //bool IsEnabled { get; set; }

        // [XmlIgnore]
        // BatchWork Works { get; set; }
    }
}
