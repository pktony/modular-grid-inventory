using System;
namespace Pktony.GridInventory.Domain
{
    public interface IInventoryReadModel
    {
        InventorySnapshot Snapshot { get; }
        event Action<InventoryChangeBatch> Changed;
    }
}
