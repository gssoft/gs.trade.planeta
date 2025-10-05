using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces.Collections;

namespace GS.Interfaces.Dde
{
    //public interface IDde : IElement1<string>
    //{
    //    void Init(IEventLog evl);
    //    void Start();
    //    void Stop();
    //    void Close();

    //    void RegisterTopic(string t, string i, Action<string> a);
    //    void RegisterDefaultCallBack(Action<string> a);

    //}
    public interface IDde21 : IElement1<string> // : IHaveCollection<string, ITopic>
    {
        void Init(IEventLog evl);

        void Start();
        void Stop();
        void Close();

        void RegisterTopic(string t, string i, Action<string> a);
        void RegisterDefaultCallBack(Action<string> a);

    }

    public interface ITopicItem : IElement1<string>, IHaveInit
    {
        string TopicItemKey { get; }
        Action<string> Action { get; set; }

    }
}
