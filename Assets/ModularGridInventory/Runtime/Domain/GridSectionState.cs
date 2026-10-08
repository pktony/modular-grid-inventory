using System.Collections.Generic;
namespace Pktony.GridInventory.Domain
{
    public sealed class GridSectionState
    {
        private readonly ItemInstanceId[] occupancy;
        public GridSectionDefinitionView Definition { get; }
        internal GridSectionState(GridSectionDefinitionView definition, IEnumerable<InventoryEntry> entries,
            IReadOnlyDictionary<ItemInstanceId, ItemInstance> items)
        {
            Definition = definition; occupancy = new ItemInstanceId[definition.Width * definition.Height];
            foreach (var entry in entries)
            {
                if (entry.SectionId.Value != definition.Id) continue;
                var item = items[entry.ItemId];
                int width = entry.Rotated ? item.Definition.Height : item.Definition.Width;
                int height = entry.Rotated ? item.Definition.Width : item.Definition.Height;
                for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
                    occupancy[(entry.Y + y) * definition.Width + entry.X + x] = entry.ItemId;
            }
        }
        public ItemInstanceId GetAt(int x, int y) => x < 0 || y < 0 || x >= Definition.Width || y >= Definition.Height
            ? default : occupancy[y * Definition.Width + x];
    }
}
