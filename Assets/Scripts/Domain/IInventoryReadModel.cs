using System;
namespace InventorySystem.Domain
{
    public interface IInventoryReadModel
    {
        InventorySnapshot Snapshot { get; }
        event Action<InventoryChangeBatch> Changed;
    }
}
