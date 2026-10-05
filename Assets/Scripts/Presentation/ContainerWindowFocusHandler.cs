using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem.Presentation
{
    public sealed class ContainerWindowFocusHandler : MonoBehaviour, IPointerDownHandler
    {
        private Action focus;
        public void Initialize(Action focus) { this.focus = focus; }
        public void OnPointerDown(PointerEventData e) => focus();
    }
}
