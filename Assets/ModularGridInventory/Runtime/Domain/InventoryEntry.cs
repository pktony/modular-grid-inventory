namespace Pktony.GridInventory.Domain
{
    public sealed class InventoryEntry
    {
        public ItemInstanceId ItemId { get; }
        public GridSectionId SectionId { get; }
        public int X { get; }
        public int Y { get; }
        public bool Rotated { get; }
        public InventoryEntry(ItemInstanceId itemId, GridSectionId sectionId, int x, int y, bool rotated)
        { ItemId = itemId; SectionId = sectionId; X = x; Y = y; Rotated = rotated; }
    }
}
