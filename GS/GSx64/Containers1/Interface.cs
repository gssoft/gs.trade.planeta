using GS.Interfaces;
namespace GS.Containers1
{
    public interface IHaveKey
    {
        string Key { get; }
    }

    public interface IContainerItem : IHaveKey
    {
      //  IContainer Container { get; }
    }

    public interface IContainer
    {
    }

   

}
