using System;
using System.Collections.Generic;
namespace InventorySystem
{
    public sealed class InventoryCellData
    {
        private readonly Dictionary<string, InventoryEntry> entries = new();
        private readonly ItemData[] occupancy;
        private readonly InventoryPlacementRules rules;
        public int capacityWidth { get; }
        public int capacityHeight { get; }
        public IEnumerable<InventoryEntry> Entries => entries.Values;
        public int Count => entries.Count;
        public event Action Changed;
        public ItemData[] itemData
        {
            get
            {
                var anchors = new ItemData[occupancy.Length];
                foreach (var entry in entries.Values) anchors[entry.X + entry.Y * capacityWidth] = entry.Item;
                return anchors;
            }
        }
        public InventoryCellData(int width, int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            capacityWidth = width; capacityHeight = height;
            occupancy = new ItemData[width * height];
            rules = new InventoryPlacementRules(width, height);
        }
        public ItemData GetItemAt(int x, int y) => x >= 0 && y >= 0 && x < capacityWidth && y < capacityHeight
            ? occupancy[x + y * capacityWidth] : null;
        public InventoryEntry GetEntry(ItemData item) => item != null && entries.TryGetValue(item.InstanceId, out var entry) ? entry : null;
        public bool CanPlace(ItemData item, int x, int y, ItemDirection direction) => rules.CanPlace(item, x, y, direction, GetItemAt);
        public bool TryAdd(ItemData item, int x, int y)
        {
            if (item == null || entries.ContainsKey(item.InstanceId) || !CanPlace(item, x, y, item.itemDirection)) return false;
            entries.Add(item.InstanceId, new InventoryEntry(item, x, y)); RebuildOccupancy(); return true;
        }
        public bool TryAdd(ItemData item)
        {
            int index = GetNextEmptyIndex(item);
            return index >= 0 && TryAdd(item, index % capacityWidth, index / capacityWidth);
        }
        public bool TryMove(ItemData item, int x, int y, ItemDirection direction)
        {
            var entry = GetEntry(item);
            if (entry == null || !CanPlace(item, x, y, direction)) return false;
            entry.SetPlacement(x, y, direction); RebuildOccupancy(); return true;
        }
        public bool TryRotate(ItemData item)
        {
            var entry = GetEntry(item);
            return entry != null && TryMove(item, entry.X, entry.Y,
                item.itemDirection == ItemDirection.Horizontal ? ItemDirection.Vertical : ItemDirection.Horizontal);
        }
        public bool Remove(ItemData item)
        {
            if (item == null || !entries.Remove(item.InstanceId)) return false;
            RebuildOccupancy(); return true;
        }
        public bool AddItem(int coordinate, ItemData item) => coordinate >= 0 && coordinate < occupancy.Length &&
            TryAdd(item, coordinate % capacityWidth, coordinate / capacityWidth);
        public int GetNextEmptyIndex(ItemData item)
        {
            if (item == null) return -1;
            for (int i = 0; i < occupancy.Length; i++)
                if (CanPlace(item, i % capacityWidth, i / capacityWidth, item.itemDirection)) return i;
            return -1;
        }
        private void RebuildOccupancy()
        {
            Array.Clear(occupancy, 0, occupancy.Length);
            foreach (var entry in entries.Values)
            {
                var size = entry.Item.GetAbsoluteSize();
                for (int y = 0; y < size.absHeight; y++)
                    for (int x = 0; x < size.absWidth; x++) occupancy[entry.X + x + (entry.Y + y) * capacityWidth] = entry.Item;
            }
            Changed?.Invoke();
        }
    }
}
