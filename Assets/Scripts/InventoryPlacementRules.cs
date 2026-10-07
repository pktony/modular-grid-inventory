using System;
namespace InventorySystem
{
    public sealed class InventoryPlacementRules
    {
        private readonly int width;
        private readonly int height;
        public InventoryPlacementRules(int width, int height) { this.width = width; this.height = height; }
        public bool CanPlace(ItemData item, int x, int y, ItemDirection direction, Func<int, int, ItemData> occupiedAt)
        {
            if (item == null || item.width <= 0 || item.height <= 0 ||
                (direction != ItemDirection.Horizontal && direction != ItemDirection.Vertical)) return false;
            int w = direction == ItemDirection.Horizontal ? item.width : item.height;
            int h = direction == ItemDirection.Horizontal ? item.height : item.width;
            if (x < 0 || y < 0 || x > width - w || y > height - h) return false;
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                {
                    var occupant = occupiedAt(x + dx, y + dy);
                    if (occupant != null && occupant != item) return false;
                }
            return true;
        }
    }
}
