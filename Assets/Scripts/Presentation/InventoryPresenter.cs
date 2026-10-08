using System;
using System.Linq;
using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventoryPresenter : IInventoryFeedback, IDisposable
    {
        private readonly IInventoryReadModel model;
        private readonly InventoryScreenBindings screen;
        private readonly InventoryPanelPresenter stash;
        private readonly ContainerWindowManager windows;
        public event Action WindowClosing;
        public ItemInstanceId Selected { get; private set; }
        public ItemInstanceId Dragging { get; private set; }
        public InventoryPresenter(IInventoryReadModel model, InventoryScreenBindings screen, InventoryPanelFactory panels,
            InventoryItemVisualFactory items, ContainerWindowManager windows)
        {
            this.model = model; this.screen = screen; this.windows = windows;
            stash = new InventoryPanelPresenter(screen.Stash, panels, items, screen.Events);
            model.Changed += OnChanged; screen.Events.Hovered += Hover; windows.Closing += Closing;
            windows.Changed += UpdateWindowHint; RenderStash(); UpdateActions(); UpdateWindowHint();
            screen.Inspector.Show(model.Snapshot, Selected);
        }
        public void Select(ItemInstanceId id)
        {
            var previous = Selected; Selected = id; stash.Select(previous, id); windows.Select(previous, id);
            if (model.Snapshot.Registry.TryGetOwner(id, out var owner)) windows.Focus(owner);
            screen.Inspector.Show(model.Snapshot, Selected); UpdateActions();
        }
        public void SetDrag(ItemInstanceId id)
        { stash.Drag(Dragging, false); windows.Drag(Dragging, false); Dragging = id; stash.Drag(id, true); windows.Drag(id, true); }
        public void Open(ItemInstanceId id)
        { if (!windows.Open(model.Snapshot, id, Selected, Dragging)) Status("This item has no internal inventory.", false); }
        public void FocusAt(Vector2 point) => windows.FocusAt(point);
        public void CloseFrontmost() { if (windows.Frontmost != null) windows.Close(windows.Frontmost.Id); }
        public void Status(string text, bool valid = true)
        { screen.Status.text = text; screen.Status.color = valid ? InventoryPalette.Accent : InventoryPalette.ErrorText; }
        private void Closing() => WindowClosing?.Invoke();
        private void OnChanged(InventoryChangeBatch batch)
        {
            var snapshot = model.Snapshot;
            if (batch.Reset) { Selected = default; Dragging = default; windows.CloseAll(); screen.Drag.Clear(); screen.Quantity.Hide(); screen.Context.Hide(); }
            if (!snapshot.Items.ContainsKey(Selected)) Selected = default;
            if (batch.Reset || batch.Containers.Contains(snapshot.RootContainerId)) RenderStash();
            windows.Reconcile(snapshot, batch, Selected, Dragging);
            screen.Inspector.Show(snapshot, Selected); UpdateActions();
        }
        private void RenderStash()
        {
            stash.Render(model.Snapshot, model.Snapshot.RootContainerId, Selected, Dragging);
            screen.Stash.Title.text = "STASH";
            var state = model.Snapshot.Containers[model.Snapshot.RootContainerId];
            var size = string.Join(" / ", state.Definition.Sections.Select(s => $"{s.Width} x {s.Height}"));
            screen.Stash.Policy.text = $"{size}   |   {state.Entries.Count} items";
        }
        private void UpdateWindowHint() => screen.EmptyWindows.gameObject.SetActive(windows.Windows.Count == 0);
        private void UpdateActions()
        {
            model.Snapshot.Items.TryGetValue(Selected, out var item);
            screen.Open.interactable = item != null && !item.ChildContainerId.IsEmpty;
            screen.Split.interactable = item != null && item.Quantity > 1 && item.Definition.MaxStack > 1;
            screen.Delete.interactable = item != null;
        }
        private void Hover(ItemInstanceId id) => screen.Inspector.Show(model.Snapshot, id.IsEmpty ? Selected : id);
        public void Dispose()
        { model.Changed -= OnChanged; screen.Events.Hovered -= Hover; windows.Closing -= Closing; windows.Changed -= UpdateWindowHint; windows.Release(); }
    }
}
