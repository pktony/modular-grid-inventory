using Pktony.GridInventory.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryPointerHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ItemInstanceId id;
        private InventoryInputEvents events;
        public void Initialize(ItemInstanceId id, InventoryInputEvents events) { this.id = id; this.events = events; }
        public void OnPointerEnter(PointerEventData e) => events.Hover(id);
        public void OnPointerExit(PointerEventData e) => events.Hover(default);
        public void OnPointerDown(PointerEventData e) => events.Press(id, e);
        public void OnPointerClick(PointerEventData e) => events.Click(id, e);
        public void OnBeginDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.Begin(id, e); }
        public void OnDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.Drag(e); }
        public void OnEndDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.End(e); }
    }
}
