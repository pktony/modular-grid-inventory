using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventorySectionView
    {
        private readonly Image[] cells;
        private readonly InventoryPaletteView palette;
        public InventorySectionGeometry Geometry { get; }
        public RectTransform ItemLayer { get; }
        public InventorySectionView(InventorySectionGeometry geometry, Image[] cells, RectTransform itemLayer, InventoryPaletteView palette)
        { this.palette = palette; Geometry = geometry; this.cells = cells; ItemLayer = itemLayer; }
        public void Highlight(int x, int y, int width, int height, bool valid)
        {
            Clear();
            for (int dy = 0; dy < height; dy++) for (int dx = 0; dx < width; dx++)
            {
                int cx = x + dx, cy = y + dy;
                if (cx >= 0 && cy >= 0 && cx < Geometry.Definition.Width && cy < Geometry.Definition.Height)
                    cells[cy * Geometry.Definition.Width + cx].color = valid ? palette.Valid : palette.Invalid;
            }
        }
        public void Clear() { foreach (var cell in cells) cell.color = palette.Cell; }
    }
}
