using GS.Containers;
using GS.Interfaces;
namespace GS.Containers
{
    public interface IHaveKey<out TKey>
    {
        TKey Key { get; }
    }

    public interface IContainerItem<out TKey> : IHaveKey<TKey>
    {
      //  IContainer Container { get; }
    }

    public interface IContainer
    {
    }

   

}
