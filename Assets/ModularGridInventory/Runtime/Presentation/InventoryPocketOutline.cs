using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public static class InventoryPocketOutline
    {
        public static void Create(InventoryElementFactory elements, RectTransform parent, int columns, int rows)
        {
            var size = InventorySectionGeometry.Size(columns, rows, elements.Theme.CellPitch, elements.Theme.CellGap);
            var outline = elements.Rect("PocketOutline", parent, Vector2.zero, size);
            elements.Panel("Top", outline, new Vector2(-1, 1), new Vector2(size.x + 2, 1), elements.Theme.Palette.Border);
            elements.Panel("Bottom", outline, new Vector2(-1, -size.y), new Vector2(size.x + 2, 1), elements.Theme.Palette.Border);
            elements.Panel("Left", outline, new Vector2(-1, 0), new Vector2(1, size.y), elements.Theme.Palette.Border);
            elements.Panel("Right", outline, new Vector2(size.x, 0), new Vector2(1, size.y), elements.Theme.Palette.Border);
        }
    }
}
