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
        private readonly InventoryPanelPresenter stash, bag;
        public event Action<ItemInstanceId> NavigationRequested;
        public ContainerNavigation Navigation { get; }
        public ItemInstanceId Selected { get; private set; }
        public ItemInstanceId Dragging { get; private set; }
        public InventoryPresenter(IInventoryReadModel model, InventoryScreenBindings screen, InventoryPanelFactory panels,
            InventoryItemVisualFactory items, ContainerNavigation navigation)
        {
            this.model = model; this.screen = screen; Navigation = navigation;
            stash = new InventoryPanelPresenter(screen.Stash, panels, items, screen.Events);
            bag = new InventoryPanelPresenter(screen.Bag, panels, items, screen.Events);
            model.Changed += OnChanged; RenderAll();
        }
        public void Select(ItemInstanceId id)
        {
            var previous = Selected; Selected = id; stash.Select(previous, id); bag.Select(previous, id);
            screen.Inspector.Show(model.Snapshot, Selected); UpdateActions();
        }
        public void SetDrag(ItemInstanceId id)
        { stash.Drag(Dragging, false); bag.Drag(Dragging, false); Dragging = id; stash.Drag(id, true); bag.Drag(id, true); }
        public void Open(ItemInstanceId id)
        {
            if (!Navigation.Open(model.Snapshot, id)) { Status("This item has no internal inventory.", false); return; }
            RenderBag();
        }
        public void Back() { Navigation.Back(model.Snapshot); RenderBag(); }
        public void Close() { Navigation.Close(); RenderBag(); }
        public void Status(string text, bool valid = true)
        { screen.Status.text = text; screen.Status.color = valid ? InventoryPalette.Accent : new Color(1, 0.62f, 0.52f); }
        private void OnChanged(InventoryChangeBatch batch)
        {
            var snapshot = model.Snapshot;
            if (batch.Reset) { Selected = default; Dragging = default; Navigation.Close(); screen.Drag.Clear(); screen.Quantity.Hide(); screen.Context.Hide(); }
            if (!snapshot.Items.ContainsKey(Selected)) Selected = default;
            Navigation.Reconcile(snapshot);
            if (batch.Reset || batch.Containers.Contains(snapshot.RootContainerId)) RenderStash();
            RenderBag(); screen.Inspector.Show(snapshot, Selected); UpdateActions();
        }
        private void RenderAll() { RenderStash(); RenderBag(); screen.Inspector.Show(model.Snapshot, Selected); UpdateActions(); }
        private void RenderStash()
        {
            stash.Render(model.Snapshot, model.Snapshot.RootContainerId, Selected, Dragging);
            screen.Stash.Title.text = $"STASH   /   {model.Snapshot.Containers[model.Snapshot.RootContainerId].Entries.Count} items";
            screen.Stash.Policy.text = "9 x 20   /   Scroll to explore   /   All item types";
        }
        private void RenderBag()
        {
            var snapshot = model.Snapshot;
            bag.Render(snapshot, Navigation.Current, Selected, Dragging);
            screen.EmptyBag.gameObject.SetActive(Navigation.Current.IsEmpty);
            screen.Back.interactable = screen.Close.interactable = !Navigation.Current.IsEmpty;
            foreach (Transform child in screen.Breadcrumb) { child.gameObject.SetActive(false); UnityEngine.Object.Destroy(child.gameObject); }
            if (Navigation.Current.IsEmpty) return;
            var path = Navigation.Path(snapshot); float x = 0;
            foreach (var id in path)
            {
                var title = snapshot.Items[id].Definition.DisplayName;
                var button = InventoryElementFactory.Button("Path-" + id, screen.Breadcrumb, new Vector2(x, 0), new Vector2(142, 32), title);
                button.onClick.AddListener(() => NavigationRequested?.Invoke(id)); x += 146;
            }
            screen.Bag.Title.text = path.Count > 0 ? snapshot.Items[path[path.Count - 1]].Definition.DisplayName : "CONTAINER";
            screen.Bag.Policy.text = InventoryInspectorPresenter.Policy(snapshot.Containers[Navigation.Current].Definition);
        }
        private void UpdateActions()
        {
            model.Snapshot.Items.TryGetValue(Selected, out var item);
            screen.Open.interactable = item != null && !item.ChildContainerId.IsEmpty;
            screen.Split.interactable = item != null && item.Quantity > 1 && item.Definition.MaxStack > 1;
            screen.Delete.interactable = item != null;
        }
        public void Dispose() => model.Changed -= OnChanged;
    }
}
