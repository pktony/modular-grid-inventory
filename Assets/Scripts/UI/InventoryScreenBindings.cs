using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryScreenBindings
    {
        public RectTransform Root, Grid, ItemLayer, Overlay, Viewport;
        public ScrollRect Scroll;
        public TextMeshProUGUI Detail, Count, Status;
        public Button Add, Delete, Reset;
    }
}
