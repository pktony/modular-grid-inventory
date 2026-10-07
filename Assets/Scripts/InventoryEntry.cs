namespace InventorySystem
{
    public sealed class InventoryEntry
    {
        public ItemData Item { get; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public InventoryEntry(ItemData item, int x, int y) { Item = item; X = x; Y = y; }
        internal void SetPlacement(int x, int y, ItemDirection direction)
        {
            X = x; Y = y; Item.itemDirection = direction;
        }
    }
}
