using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryGridView
    {
        private readonly Image[] cells;
        private readonly int width;
        public InventoryGridView(RectTransform root, InventoryGridGeometry geometry, int width, int height)
        {
            this.width = width; cells = new Image[width * height];
            root.sizeDelta = geometry.Size(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    cells[x + y * width] = UIElementFactory.Panel($"Cell {x},{y}", root,
                        geometry.Position(x, y), Vector2.one * geometry.CellSize, InventoryTheme.Cell);
        }
        public void ClearPreview() { foreach (var cell in cells) cell.color = InventoryTheme.Cell; }
        public void Preview(int x, int y, int w, int h, bool valid)
        {
            ClearPreview();
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    if (x + dx >= 0 && x + dx < width && y + dy >= 0 && y + dy < cells.Length / width)
                        cells[x + dx + (y + dy) * width].color = valid ? InventoryTheme.Valid : InventoryTheme.Invalid;
        }
    }
}
