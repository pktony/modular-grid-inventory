using System;
using UnityEngine.EventSystems;
namespace InventorySystem
{
    public sealed class InventoryPointerEvents
    {
        public event Action<ItemData, PointerEventData> Selected, DragStarted;
        public event Action<PointerEventData> Dragged, DragEnded;
        public event Action<ItemData> Hovered;
        public void Select(ItemData item, PointerEventData e) => Selected?.Invoke(item, e);
        public void Begin(ItemData item, PointerEventData e) => DragStarted?.Invoke(item, e);
        public void Drag(PointerEventData e) => Dragged?.Invoke(e);
        public void End(PointerEventData e) => DragEnded?.Invoke(e);
        public void Hover(ItemData item) => Hovered?.Invoke(item);
    }
}
