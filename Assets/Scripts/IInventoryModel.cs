using System;
using System.Collections.Generic;
namespace InventorySystem
{
    public interface IInventoryModel
    {
        int capacityWidth { get; }
        int capacityHeight { get; }
        int Count { get; }
        IEnumerable<InventoryEntry> Entries { get; }
        event Action Changed;
        InventoryEntry GetEntry(ItemData item);
        ItemData GetItemAt(int x, int y);
        bool CanPlace(ItemData item, int x, int y, ItemDirection direction);
        bool TryMove(ItemData item, int x, int y, ItemDirection direction);
        bool Remove(ItemData item);
    }
}
