using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryScreenBindings
    {
        public RectTransform Root;
        public InventoryInputEvents Events;
        public InventoryPanelBindings Stash, Bag;
        public TextMeshProUGUI Status, EmptyBag;
        public Button Open, Split, Delete, Reset, Back, Close;
        public RectTransform Breadcrumb;
        public InventoryInspectorPresenter Inspector;
        public InventoryDragPresenter Drag;
        public StackQuantityDialog Quantity;
        public InventoryContextMenu Context;
    }
}
