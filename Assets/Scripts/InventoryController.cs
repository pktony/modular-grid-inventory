using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem
{
    public sealed class InventoryController : IDisposable
    {
        private readonly IInventoryModel model;
        private readonly IInventoryView view;
        private InventoryDragSession drag;
        private ItemData selected;
        private Vector2 pointer;
        public InventoryController(IInventoryModel model, IInventoryView view)
        {
            this.model = model; this.view = view;
            view.PointerEvents.Selected += Select;
            view.PointerEvents.DragStarted += Begin;
            view.PointerEvents.Dragged += Move;
            view.PointerEvents.DragEnded += End;
            view.PointerEvents.Hovered += view.ShowItem;
            view.RemoveRequested += RemoveSelected;
            model.Changed += ValidateSelection;
        }
        public void UpdatePointer(Vector2 point)
        {
            pointer = point;
            if (drag != null) Preview();
        }
        public void Rotate() { if (drag == null) return; drag.Rotate(); Preview(); }
        private void Select(ItemData item, PointerEventData e)
        { selected = item; view.SetSelection(item); }
        private void Begin(ItemData item, PointerEventData e)
        {
            var entry = model.GetEntry(item); if (entry == null) return;
            selected = item; pointer = e.position;
            var cell = view.CellAt(e.pressPosition);
            drag = new InventoryDragSession(entry, cell.x - entry.X, cell.y - entry.Y);
            Preview();
        }
        private void Move(PointerEventData e) { pointer = e.position; Preview(); }
        private void Preview()
        {
            if (drag == null) return;
            var cell = view.CellAt(pointer); drag.SetPointerCell(cell.x, cell.y);
            bool valid = view.IsOverGrid(pointer) && model.CanPlace(drag.Item, drag.X, drag.Y, drag.Direction);
            view.ShowPreview(drag.Item, drag.X, drag.Y, drag.Direction, valid);
            view.SetStatus(valid ? "Valid placement. Release to move." : "Blocked placement. Release to cancel.");
        }
        private void End(PointerEventData e)
        {
            if (drag == null) return;
            pointer = e.position; Preview();
            bool moved = view.IsOverGrid(pointer) && model.TryMove(drag.Item, drag.X, drag.Y, drag.Direction);
            drag = null; view.ClearPreview(); view.SetStatus(moved ? "Item moved." : "Move cancelled. Original placement preserved.");
        }
        public void Cancel()
        { drag = null; selected = null; view.ClearPreview(); view.SetSelection(null); view.SetStatus("Ready."); }
        public void RemoveSelected()
        {
            if (drag != null) { view.SetStatus("Finish or cancel the drag first."); return; }
            bool removed = model.Remove(selected);
            selected = null; view.SetSelection(null); view.SetStatus(removed ? "Item removed." : "Select an item first.");
        }
        private void ValidateSelection()
        { if (selected != null && model.GetEntry(selected) == null) { selected = null; view.SetSelection(null); } }
        public void Dispose()
        {
            view.PointerEvents.Selected -= Select; view.PointerEvents.DragStarted -= Begin;
            view.PointerEvents.Dragged -= Move; view.PointerEvents.DragEnded -= End;
            view.PointerEvents.Hovered -= view.ShowItem; view.RemoveRequested -= RemoveSelected;
            model.Changed -= ValidateSelection;
        }
    }
}
