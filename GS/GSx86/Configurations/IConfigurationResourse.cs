using System.Xml.Linq;
using GS.Containers5;
using GS.Elements;

namespace GS.Configurations
{
    public interface IConfigurationResourse : IElement1<string>
    {
        void Init();
        XDocument Get(string configurationKey, string cnfItemKey);
        XDocument Get(string configurationKey, string cnfItemKey, string cnfObjKey);

        void LoadAssemblies();
    }
    public interface IConfigurationResourse2 : IElement1<string>
    {
        void Init();

        string ConfigurationKey { get; }
        XDocument Get(string cnfItemKey);
        XDocument Get(string cnfItemKey, string cnfObjKey);

        byte[] GetByteArr(string cnfItemKey, string cnfObjKey);

        void LoadAssemblies();
    }

    public interface IConfigurationResourse21 : IConfigurationResourse2
    {
        T Build<T>(string cnfItem, string root) where T : class ;

        T Build<T, TKey, TItem>(string configurationItem, string root)
            where T : class
            where TItem : class, IHaveKey<TKey>;
    }

    public interface IConfigurationRequester
    {
        void Init();
        XDocument Get(string configurationKey, string cnfItemKey);
        XDocument Get(string configurationKey, string cnfItemKey, string cnfObjKey);
        byte[] GetByteArray(string configurationKey, string cnfItemKey, string cnfObjKey);

        XDocument Get(long token, string configurationKey, string cnfItemKey);
        XDocument Get(long token, string configurationKey, string cnfItemKey, string cnfObjKey);
        byte[] GetByteArray(long token, string configurationKey, string cnfItemKey, string cnfObjKey);
    }
}
