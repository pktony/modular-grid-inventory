using System;
using InventorySystem.Domain;
using InventorySystem.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem.Editor
{
    public sealed class ShowcasePointerDriver : IDisposable
    {
        private readonly ExpandedInventory inventory;
        private GameObject dragged;
        private PointerEventData data;
        private Vector2 start, destination, pointer;
        private float started, flashUntil;
        private bool holding;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        public ShowcasePointerDriver(ExpandedInventory inventory) { this.inventory = inventory; }
        public Vector2 Cell(ContainerId owner, string section, int x = 0, int y = 0)
        {
            var panel = owner == State.RootContainerId ? inventory.Screen.Stash : inventory.Windows.Find(owner).Panel;
            return panel.Sections[new GridSectionId(section)].Geometry.ScreenPoint(x, y, new Vector2(25, 25));
        }
        public Vector2 ItemPoint(ItemInstanceId id)
        {
            State.Registry.TryGetOwner(id, out var owner); var entry = State.Containers[owner].Entries[id];
            return Cell(owner, entry.SectionId.Value, entry.X, entry.Y);
        }
        private static GameObject View(ItemInstanceId id) => GameObject.Find("Item-" + id);
        private static PointerEventData Event(Vector2 point, int clicks = 1) => new(EventSystem.current)
            { position = point, pressPosition = point, clickCount = clicks, button = PointerEventData.InputButton.Left };
        public void Open(ItemInstanceId id, float time)
        {
            pointer = ItemPoint(id); var e = Event(pointer, 2);
            ExecuteEvents.Execute(View(id), e, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(View(id), e, ExecuteEvents.pointerClickHandler);
            flashUntil = time + 0.2f; Canvas.ForceUpdateCanvases();
        }
        public void Begin(ItemInstanceId id, Vector2 target, float time)
            => Begin(View(id), ItemPoint(id), target, time);
        private void Begin(GameObject view, Vector2 source, Vector2 target, float time)
        {
            dragged = view; pointer = start = source; destination = target;
            data = Event(source); started = time; holding = true;
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.beginDragHandler);
        }
        public void MoveWindow(ItemInstanceId id, Vector2 position, float time)
        {
            var window = inventory.Windows.Find(State.Items[id].ChildContainerId);
            var source = RectTransformUtility.WorldToScreenPoint(null, window.Header.TransformPoint(new Vector3(35, -12)));
            var desktop = window.Panel.Root.parent;
            var delta = position - window.Panel.Root.anchoredPosition;
            var target = source + RectTransformUtility.WorldToScreenPoint(null, desktop.TransformPoint(delta))
                - RectTransformUtility.WorldToScreenPoint(null, desktop.position);
            Begin(window.Header.gameObject, source, target, time);
        }
        public void End()
        {
            if (!holding) throw new InvalidOperationException("No pointer gesture to release.");
            data.position = destination;
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.endDragHandler);
            pointer = destination; holding = false;
        }
        public void Tick(float time)
        {
            if (holding)
            {
                float t = Mathf.Clamp01((time - started - 0.04f) / 0.42f);
                pointer = Vector2.Lerp(start, destination, 1 - Mathf.Pow(1 - t, 3));
                data.position = pointer; ExecuteEvents.Execute(dragged, data, ExecuteEvents.dragHandler);
            }
            inventory.Indicator.Show(pointer, holding || time < flashUntil);
        }
        public void Dispose() { inventory.Interaction.Cancel(); inventory.Indicator.Show(Vector2.zero, false); }
    }
}
