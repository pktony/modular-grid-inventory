namespace Pktony.GridInventory.Presentation
{
    public interface IInventoryInputSource
    {
        bool IsAvailable { get; }
        InventoryInputFrame Read();
    }
}
