using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem
{
    public sealed class InventoryItemPointerHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ItemData item;
        private InventoryPointerEvents events;
        public void Initialize(ItemData item, InventoryPointerEvents events) { this.item = item; this.events = events; }
        public void OnPointerDown(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.Select(item, e); }
        public void OnBeginDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.Begin(item, e); }
        public void OnDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.Drag(e); }
        public void OnEndDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.End(e); }
        public void OnPointerEnter(PointerEventData e) => events.Hover(item);
        public void OnPointerExit(PointerEventData e) => events.Hover(null);
    }
}
