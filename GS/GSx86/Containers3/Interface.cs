//using GS.Interfaces;
namespace GS.Containers3
{
    public interface IHaveStringKey
    {
        string Key { get; }
    }

    public interface IHaveKey<out TKey> 
    {
        TKey Key { get; }
    }

    public interface IContainerItem<out TKey> : Containers3.IHaveKey<TKey>
    {
    }
    
    public interface IContainer
    {
    }

   

}
