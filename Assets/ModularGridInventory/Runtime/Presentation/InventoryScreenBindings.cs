using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryScreenBindings
    {
        public InventoryUiTheme Theme;
        public RectTransform Root;
        public InventoryInputEvents Events;
        public InventoryPanelBindings Stash;
        public RectTransform WindowLayer, Overlay;
        public TextMeshProUGUI Status, EmptyWindows;
        public Button Open, Split, Delete, Reset;
        public InventoryInspectorPresenter Inspector;
        public InventoryDragPresenter Drag;
        public StackQuantityDialog Quantity;
        public InventoryContextMenu Context;
    }
}
