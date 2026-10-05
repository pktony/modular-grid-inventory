using UnityEngine;
namespace InventorySystem.Presentation
{
    public static class InventoryPocketOutline
    {
        public static void Create(RectTransform parent, int columns, int rows)
        {
            var size = InventorySectionGeometry.Size(columns, rows);
            var outline = InventoryElementFactory.Rect("PocketOutline", parent, Vector2.zero, size);
            InventoryElementFactory.Panel("Top", outline, new Vector2(-1, 1), new Vector2(size.x + 2, 1), InventoryPalette.Border);
            InventoryElementFactory.Panel("Bottom", outline, new Vector2(-1, -size.y), new Vector2(size.x + 2, 1), InventoryPalette.Border);
            InventoryElementFactory.Panel("Left", outline, new Vector2(-1, 0), new Vector2(1, size.y), InventoryPalette.Border);
            InventoryElementFactory.Panel("Right", outline, new Vector2(size.x, 0), new Vector2(1, size.y), InventoryPalette.Border);
        }
    }
}
