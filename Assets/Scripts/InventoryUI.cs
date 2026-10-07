using System;
using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryUI : MonoBehaviour, IInventoryView, IDemoInventoryView
    {
        private InventoryScreenBindings bindings;
        private InventoryGridGeometry geometry;
        private InventoryGridView gridView;
        private InventoryItemPresenter itemPresenter;
        private InventoryPreviewPresenter preview;
        private InventoryHudPresenter hud;
        private InventoryTooltipPresenter tooltip;
        private IInventoryModel model;
        private ItemData selected, dragging;
        public InventoryPointerEvents PointerEvents { get; } = new();
        public event Action AddRequested, RemoveRequested, ResetRequested;
        public void Initialize(IInventoryModel inventory)
        {
            DisposeView(); model = inventory; selected = null; dragging = null;
            bindings = new InventoryScreenBuilder().Build(transform);
            geometry = new InventoryGridGeometry(bindings.Grid, 80);
            gridView = new InventoryGridView(bindings.Grid, geometry, model.capacityWidth, model.capacityHeight);
            bindings.ItemLayer.SetAsLastSibling();
            itemPresenter = new InventoryItemPresenter(bindings.ItemLayer, geometry, PointerEvents);
            preview = new InventoryPreviewPresenter(bindings.Overlay, geometry, gridView);
            hud = new InventoryHudPresenter(bindings);
            tooltip = new InventoryTooltipPresenter(bindings.Overlay);
            bindings.Add.onClick.AddListener(() => AddRequested?.Invoke());
            bindings.Delete.onClick.AddListener(() => RemoveRequested?.Invoke());
            bindings.Reset.onClick.AddListener(() => ResetRequested?.Invoke());
            model.Changed += Refresh; Refresh();
        }
        public Vector2Int CellAt(Vector2 point) => geometry.CellAt(point);
        public bool IsOverGrid(Vector2 point) => RectTransformUtility.RectangleContainsScreenPoint(bindings.Viewport, point, null);
        public void SetSelection(ItemData item) { selected = item; hud.ShowItem(item); Refresh(); }
        public void ShowItem(ItemData item) { hud.ShowItem(item ?? selected); tooltip.Show(dragging == null ? item : null); }
        public void UpdateTooltip(Vector2 point) => tooltip.Move(point);
        public void SetStatus(string message) => hud.SetStatus(message);
        public void ShowPreview(ItemData item, int x, int y, ItemDirection direction, bool valid)
        { dragging = item; tooltip.Show(null); preview.Show(item, x, y, direction, valid); Refresh(); }
        public void ClearPreview() { dragging = null; preview.Clear(); Refresh(); }
        private void Refresh()
        {
            if (model == null) return;
            if (selected != null && model.GetEntry(selected) == null) selected = null;
            itemPresenter.Render(model.Entries, selected, dragging); hud.SetCount(model);
            if (selected != null) hud.ShowItem(selected);
        }
        private void DisposeView()
        {
            if (model != null) model.Changed -= Refresh;
            if (bindings != null) { bindings.Root.gameObject.SetActive(false); Destroy(bindings.Root.gameObject); }
        }
        private void OnDestroy() => DisposeView();
    }
}
