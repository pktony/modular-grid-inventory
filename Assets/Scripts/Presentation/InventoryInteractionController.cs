using System;
using InventorySystem.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem.Presentation
{
    public sealed class InventoryInteractionController : IDisposable
    {
        private readonly IInventoryReadModel model;
        private readonly InventoryDropResolver drops;
        private readonly IInventoryEditService edit;
        private readonly IInventoryFeedback presenter;
        private readonly IInventoryGridHitTester hitTest;
        private readonly InventoryScreenBindings screen;
        private Vector2 pointer;
        public InventoryDragState Drag { get; private set; }
        public InventoryInteractionController(IInventoryReadModel model, IInventoryEditService edit, IInventoryFeedback presenter,
            IInventoryGridHitTester hitTest, InventoryDropResolver drops, InventoryScreenBindings screen)
        {
            this.model = model; this.drops = drops; this.edit = edit;
            this.presenter = presenter; this.hitTest = hitTest; this.screen = screen;
            screen.Events.Pressed += Press; screen.Events.Clicked += Click; screen.Events.BeginDrag += Begin;
            screen.Events.Dragged += Move; screen.Events.EndDrag += End; screen.Events.GridClicked += GridClick;
            screen.Context.OpenRequested += Open; screen.Context.SplitRequested += Split; screen.Context.DeleteRequested += Delete;
            screen.Open.onClick.AddListener(OpenSelected); screen.Split.onClick.AddListener(SplitSelected);
            screen.Delete.onClick.AddListener(DeleteSelected);
            model.Changed += OnChanged; presenter.WindowClosing += Cancel;
        }
        private void Press(ItemInstanceId id, PointerEventData e)
        {
            if (Drag?.SplitQuantity > 0) { Drop(e.position); return; }
            presenter.Select(id); screen.Context.Hide(); pointer = e.position;
        }
        private void Click(ItemInstanceId id, PointerEventData e)
        {
            if (!model.Snapshot.Items.TryGetValue(id, out var item) || Drag != null) return;
            if (e.button == PointerEventData.InputButton.Right) screen.Context.Show(item, e.position);
            else if (e.clickCount >= 2) Open(id);
        }
        private void Begin(ItemInstanceId id, PointerEventData e)
        {
            if (screen.Quantity.IsOpen || Drag != null || !model.Snapshot.Items.TryGetValue(id, out var item)) return;
            model.Snapshot.Registry.TryGetOwner(id, out var owner);
            var entry = model.Snapshot.Containers[owner].Entries[id];
            var section = hitTest.Find(new PlacementTarget(owner, entry.SectionId, entry.X, entry.Y));
            if (section == null || !section.Geometry.TryCell(e.pressPosition, out var cell)) return;
            var grip = new Vector2Int(cell.x - entry.X, cell.y - entry.Y);
            Drag = new InventoryDragState(item, entry.Rotated, grip, section.Geometry.GripFraction(e.pressPosition));
            presenter.SetDrag(id); UpdatePointer(e.position);
        }
        private void Move(PointerEventData e) => UpdatePointer(e.position);
        private void End(PointerEventData e) => Drop(e.position);
        private void GridClick(PointerEventData e) { presenter.FocusAt(e.position); screen.Context.Hide(); if (Drag?.SplitQuantity > 0) Drop(e.position); }
        public void UpdatePointer(Vector2 point)
        {
            pointer = point;
            if (Drag == null || !model.Snapshot.Items.TryGetValue(Drag.Item, out var item)) return;
            var preview = drops.Resolve(Drag, point); var result = preview.Result; var target = preview.Target;
            screen.Drag.Show(item, Drag, point, result.Success);
            screen.Drag.Highlight(hitTest.Find(target), target, Drag.Width, Drag.Height, result.Success);
            screen.Drag.HighlightContainer(hitTest.FindItem(preview.ContainerItem), result.Success);
            presenter.Status(result.Success ? Drag.SplitQuantity > 0 ? "Click an empty compartment cell to place the split. ESC cancels." : "Release to place. R rotates. ESC cancels." : result.Reason, result.Success);
        }
        public void Drop(Vector2 point)
        {
            if (Drag == null) return;
            var preview = drops.Resolve(Drag, point); var merge = preview.Merge;
            if (!preview.Result.Success) { Cancel(); presenter.Status(preview.Result.Reason, false); return; }
            var drag = Drag;
            Cancel();
            var result = drops.Commit(drag, preview);
            presenter.Status(result.Success ? !merge.IsEmpty ? $"Merged {result.MovedQuantity}. Remaining quantity stays at its source." : $"Placed {result.MovedQuantity} item(s)." : result.Reason, result.Success);
        }
        public void Rotate() { if (Drag != null) { Drag.Rotate(); UpdatePointer(pointer); } }
        public void Cancel()
        { Drag = null; presenter.SetDrag(default); screen.Drag.Clear(); screen.Context.Hide(); screen.Quantity.Hide(); presenter.Status("Ready"); }
        public void Open(ItemInstanceId id) { Cancel(); presenter.Open(id); }
        public void Split(ItemInstanceId id)
        {
            Cancel();
            if (!model.Snapshot.Items.TryGetValue(id, out var item) || item.Quantity < 2 || item.Definition.MaxStack <= 1) return;
            screen.Quantity.Show(item.Quantity, quantity =>
            {
                if (!model.Snapshot.Items.TryGetValue(id, out var current)) return;
                Drag = new InventoryDragState(current, false, Vector2Int.zero, new Vector2(0.5f, 0.5f), quantity);
                presenter.Status("Choose an empty cell for the split. ESC cancels."); UpdatePointer(pointer);
            });
        }
        public void Delete(ItemInstanceId id)
        {
            Cancel(); var result = edit.Delete(id);
            presenter.Status(result.Success ? $"Deleted {result.MovedQuantity} item(s)." : result.Reason, result.Success);
        }
        public void OpenSelected() => Open(presenter.Selected);
        public void SplitSelected() => Split(presenter.Selected);
        public void DeleteSelected() { if (!presenter.Selected.IsEmpty) Delete(presenter.Selected); }
        public void Escape() { if (Drag != null || screen.Quantity.IsOpen || screen.Context.IsOpen) Cancel(); else presenter.CloseFrontmost(); }
        private void OnChanged(InventoryChangeBatch batch) { if (batch.Reset || Drag != null && !model.Snapshot.Items.ContainsKey(Drag.Item)) Cancel(); }
        public void Dispose()
        {
            Drag = null; model.Changed -= OnChanged; presenter.WindowClosing -= Cancel;
            screen.Events.Pressed -= Press; screen.Events.Clicked -= Click; screen.Events.BeginDrag -= Begin;
            screen.Events.Dragged -= Move; screen.Events.EndDrag -= End; screen.Events.GridClicked -= GridClick;
            screen.Context.OpenRequested -= Open; screen.Context.SplitRequested -= Split; screen.Context.DeleteRequested -= Delete;
            if (screen.Open != null) screen.Open.onClick.RemoveListener(OpenSelected);
            if (screen.Split != null) screen.Split.onClick.RemoveListener(SplitSelected);
            if (screen.Delete != null) screen.Delete.onClick.RemoveListener(DeleteSelected);
        }
    }
}
