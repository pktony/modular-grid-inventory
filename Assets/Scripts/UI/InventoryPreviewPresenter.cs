using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryPreviewPresenter
    {
        private readonly RectTransform overlay;
        private readonly InventoryGridGeometry geometry;
        private readonly InventoryGridView grid;
        private InventoryItemView ghost;
        private ItemData item;
        public InventoryPreviewPresenter(RectTransform overlay, InventoryGridGeometry geometry, InventoryGridView grid)
        { this.overlay = overlay; this.geometry = geometry; this.grid = grid; }
        public void Show(ItemData data, int x, int y, ItemDirection direction, bool valid)
        {
            if (item != data) { Clear(); item = data; ghost = new InventoryItemViewFactory().Create(data, overlay, null); }
            int w = direction == ItemDirection.Horizontal ? data.width : data.height;
            int h = direction == ItemDirection.Horizontal ? data.height : data.width;
            ghost.Rect.position = geometry.WorldPosition(x, y);
            ghost.Present(geometry.Size(w, h), direction, valid, 0.8f);
            grid.Preview(x, y, w, h, valid);
        }
        public void Clear()
        {
            if (ghost != null) { ghost.gameObject.SetActive(false); Object.Destroy(ghost.gameObject); }
            ghost = null; item = null; grid.ClearPreview();
        }
    }
}
