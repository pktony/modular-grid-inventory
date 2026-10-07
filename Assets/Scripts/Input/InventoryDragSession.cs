namespace InventorySystem
{
    public sealed class InventoryDragSession
    {
        public ItemData Item { get; }
        public ItemDirection Direction { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int GripX { get; private set; }
        public int GripY { get; private set; }
        public InventoryDragSession(InventoryEntry entry, int gripX, int gripY)
        {
            Item = entry.Item; Direction = Item.itemDirection; X = entry.X; Y = entry.Y;
            var size = Item.GetAbsoluteSize();
            GripX = System.Math.Clamp(gripX, 0, size.absWidth - 1);
            GripY = System.Math.Clamp(gripY, 0, size.absHeight - 1);
        }
        public void SetPointerCell(int x, int y) { X = x - GripX; Y = y - GripY; }
        public void Rotate()
        {
            int pointerX = X + GripX, pointerY = Y + GripY;
            int x = GripX, y = GripY;
            if (Direction == ItemDirection.Horizontal) { GripX = Item.height - 1 - y; GripY = x; }
            else { GripX = y; GripY = Item.height - 1 - x; }
            Direction = Direction == ItemDirection.Horizontal ? ItemDirection.Vertical : ItemDirection.Horizontal;
            SetPointerCell(pointerX, pointerY);
        }
    }
}
