using System.Collections.Generic;
using System.Linq;
using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryPanelPresenter
    {
        private readonly InventoryPanelFactory panels;
        private readonly InventoryItemVisualFactory items;
        private readonly InventoryInputEvents events;
        private readonly Dictionary<ItemInstanceId, InventoryItemVisual> views = new();
        public InventoryPanelBindings Bindings { get; }
        public int ViewCount => views.Count;
        public InventoryPanelPresenter(InventoryPanelBindings bindings, InventoryPanelFactory panels, InventoryItemVisualFactory items,
            InventoryInputEvents events) { Bindings = bindings; this.panels = panels; this.items = items; this.events = events; }
        public void Render(InventorySnapshot snapshot, ContainerId id, ItemInstanceId selected, ItemInstanceId dragging)
        {
            if (!snapshot.Containers.TryGetValue(id, out var state)) { Hide(); return; }
            Bindings.Root.gameObject.SetActive(true);
            if (Bindings.Container != id) { views.Clear(); panels.BuildSections(Bindings, state, events); }
            foreach (var removed in views.Keys.Where(k => !state.Entries.ContainsKey(k)).ToArray())
            { views[removed].Rect.gameObject.SetActive(false); Object.Destroy(views[removed].Rect.gameObject); views.Remove(removed); Bindings.ItemRects.Remove(removed); }
            foreach (var entry in state.Entries.Values)
            {
                var layer = Bindings.Sections[entry.SectionId].ItemLayer;
                if (!views.TryGetValue(entry.ItemId, out var view))
                { view = items.Create(layer, entry.ItemId, events); views.Add(entry.ItemId, view); Bindings.ItemRects.Add(entry.ItemId, view.Rect); }
                if (view.Rect.parent != layer) view.Rect.SetParent(layer, false);
                view.Present(snapshot.Items[entry.ItemId], entry, entry.ItemId == selected, entry.ItemId == dragging);
            }
        }
        public void Select(ItemInstanceId previous, ItemInstanceId current)
        { if (views.TryGetValue(previous, out var a)) a.SetSelected(false); if (views.TryGetValue(current, out var b)) b.SetSelected(true); }
        public void Drag(ItemInstanceId id, bool active) { if (views.TryGetValue(id, out var item)) item.SetDragging(active); }
        public void Hide() { Bindings.Root.gameObject.SetActive(false); }
    }
}
