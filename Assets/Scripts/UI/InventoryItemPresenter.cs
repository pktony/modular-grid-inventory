using System.Collections.Generic;
using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryItemPresenter
    {
        private readonly Dictionary<string, InventoryItemView> views = new();
        private readonly RectTransform parent;
        private readonly InventoryGridGeometry geometry;
        private readonly InventoryPointerEvents events;
        private readonly InventoryItemViewFactory factory = new();
        public InventoryItemPresenter(RectTransform parent, InventoryGridGeometry geometry, InventoryPointerEvents events)
        { this.parent = parent; this.geometry = geometry; this.events = events; }
        public void Render(IEnumerable<InventoryEntry> entries, ItemData selected, ItemData dragging)
        {
            var alive = new HashSet<string>();
            foreach (var entry in entries)
            {
                var item = entry.Item; alive.Add(item.InstanceId);
                if (!views.TryGetValue(item.InstanceId, out var view))
                { view = factory.Create(item, parent, events); views.Add(item.InstanceId, view); }
                var size = item.GetAbsoluteSize();
                view.Rect.anchoredPosition = geometry.Position(entry.X, entry.Y);
                view.Present(geometry.Size(size.absWidth, size.absHeight), item.itemDirection, item == selected, item == dragging ? 0.25f : 1);
            }
            var stale = new List<string>();
            foreach (var pair in views) if (!alive.Contains(pair.Key)) { Object.Destroy(pair.Value.gameObject); stale.Add(pair.Key); }
            foreach (var id in stale) views.Remove(id);
        }
    }
}
