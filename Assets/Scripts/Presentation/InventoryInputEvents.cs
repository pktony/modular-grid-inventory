using System;
using InventorySystem.Domain;
using UnityEngine.EventSystems;
namespace InventorySystem.Presentation
{
    public sealed class InventoryInputEvents
    {
        public event Action<ItemInstanceId, PointerEventData> Pressed, BeginDrag, Clicked;
        public event Action<PointerEventData> Dragged, EndDrag, GridClicked;
        internal void Press(ItemInstanceId id, PointerEventData e) => Pressed?.Invoke(id, e);
        internal void Begin(ItemInstanceId id, PointerEventData e) => BeginDrag?.Invoke(id, e);
        internal void Click(ItemInstanceId id, PointerEventData e) => Clicked?.Invoke(id, e);
        internal void Drag(PointerEventData e) => Dragged?.Invoke(e);
        internal void End(PointerEventData e) => EndDrag?.Invoke(e);
        internal void GridClick(PointerEventData e) => GridClicked?.Invoke(e);
    }
}
