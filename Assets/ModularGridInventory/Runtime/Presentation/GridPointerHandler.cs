using UnityEngine;
using UnityEngine.EventSystems;
namespace Pktony.GridInventory.Presentation
{
    public sealed class GridPointerHandler : MonoBehaviour, IPointerClickHandler
    {
        private InventoryInputEvents events;
        public void Initialize(InventoryInputEvents events) { this.events = events; }
        public void OnPointerClick(PointerEventData e) { if (e.button == PointerEventData.InputButton.Left) events.GridClick(e); }
    }
}
