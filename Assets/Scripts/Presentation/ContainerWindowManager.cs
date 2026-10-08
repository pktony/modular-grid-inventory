using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class ContainerWindowManager : IInventoryPanelSource
    {
        private readonly Dictionary<ContainerId, ContainerWindowPresenter> windows = new();
        private readonly ContainerWindowFactory factory;
        private readonly InventoryPanelBindings stash;
        public IReadOnlyDictionary<ContainerId, ContainerWindowPresenter> Windows { get; }
        public event Action Closing;
        public event Action Changed;
        public event Action<ItemInstanceId> Opened, Closed;
        public ContainerWindowPresenter Frontmost => windows.Values.Where(w => w.Panel.Root != null).OrderByDescending(w => w.Panel.Root.GetSiblingIndex()).FirstOrDefault();
        public IEnumerable<InventoryPanelBindings> FrontToBack => windows.Values.Where(w => w.Panel.Root != null).OrderByDescending(w => w.Panel.Root.GetSiblingIndex()).Select(w => w.Panel).Concat(new[] { stash });
        public ContainerWindowManager(ContainerWindowFactory factory, InventoryPanelBindings stash)
        { this.factory = factory; this.stash = stash; Windows = new ReadOnlyDictionary<ContainerId, ContainerWindowPresenter>(windows); }
        public ContainerWindowPresenter Find(ContainerId id) => windows.TryGetValue(id, out var window) ? window : null;
        public bool Open(InventorySnapshot snapshot, ItemInstanceId id, ItemInstanceId selected, ItemInstanceId dragging)
        {
            if (!snapshot.Items.TryGetValue(id, out var item) || item.ChildContainerId.IsEmpty) return false;
            bool created = !windows.TryGetValue(item.ChildContainerId, out var window);
            if (created)
            {
                var child = item.ChildContainerId;
                window = factory.Create(snapshot, item, windows.Count, () => Focus(child), () => Close(child));
                windows.Add(child, window); window.Render(snapshot, selected, dragging);
            }
            Focus(item.ChildContainerId); Changed?.Invoke(); if (created) Opened?.Invoke(id); return true;
        }
        public void Focus(ContainerId id)
        {
            if (!windows.TryGetValue(id, out var window)) return;
            window.Panel.Root.SetAsLastSibling();
            foreach (var other in windows.Values) if (other.Panel.Root != null) other.SetFocused(other == window);
        }
        public void FocusAt(Vector2 point)
        {
            foreach (var panel in FrontToBack)
                if (RectTransformUtility.RectangleContainsScreenPoint(panel.Root, point)) { Focus(panel.Container); return; }
        }
        public void Close(ContainerId id, bool notify = true)
        {
            if (!windows.TryGetValue(id, out var window)) return;
            Closing?.Invoke(); windows.Remove(id);
            if (window.Panel.Root != null) { window.Panel.Root.gameObject.SetActive(false); UnityEngine.Object.Destroy(window.Panel.Root.gameObject); }
            Changed?.Invoke();
            if (notify) Closed?.Invoke(window.Owner);
            var front = Frontmost; if (front != null) Focus(front.Id);
        }
        public void Release() { windows.Clear(); Closing = null; Changed = null; Opened = null; Closed = null; }
        public void CloseAll() { foreach (var id in windows.Keys.ToArray()) Close(id, false); }
        public void Reconcile(InventorySnapshot snapshot, InventoryChangeBatch batch, ItemInstanceId selected, ItemInstanceId dragging)
        {
            foreach (var window in windows.Values.ToArray())
            {
                if (!snapshot.Containers.ContainsKey(window.Id) || !snapshot.Items.ContainsKey(window.Owner)) Close(window.Id, false);
                else if (batch.Containers.Contains(window.Id)) window.Render(snapshot, selected, dragging);
            }
        }
        public void Select(ItemInstanceId previous, ItemInstanceId current)
        { foreach (var window in windows.Values) window.Select(previous, current); }
        public void Drag(ItemInstanceId id, bool active) { foreach (var window in windows.Values) window.Drag(id, active); }
    }
}
