using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Pktony.GridInventory.Presentation
{
    public sealed class ContainerWindowDragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform window, desktop;
        private Vector2 offset;
        private Action focus;
        public void Initialize(RectTransform window, RectTransform desktop, Action focus)
        { this.window = window; this.desktop = desktop; this.focus = focus; }
        public void OnPointerDown(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) focus(); }
        public void OnBeginDrag(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left) return;
            focus(); RectTransformUtility.ScreenPointToLocalPointInRectangle(desktop, e.position, null, out var point);
            offset = window.anchoredPosition - point;
        }
        public void OnDrag(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(desktop, e.position, null, out var point);
            var next = point + offset;
            window.anchoredPosition = new Vector2(Mathf.Clamp(next.x, 8, desktop.rect.width - window.rect.width - 8),
                Mathf.Clamp(next.y, -desktop.rect.height + window.rect.height + 62, -88));
            focus();
        }
        public void OnEndDrag(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) focus(); }
    }
}
