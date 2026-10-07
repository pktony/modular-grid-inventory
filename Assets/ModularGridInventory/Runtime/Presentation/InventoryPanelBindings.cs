using System.Collections.Generic;
using Pktony.GridInventory.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryPanelBindings
    {
        public RectTransform Root, Content, Viewport;
        public ScrollRect Scroll;
        public TextMeshProUGUI Title, Policy;
        public readonly Dictionary<GridSectionId, InventorySectionView> Sections = new();
        public readonly Dictionary<ItemInstanceId, RectTransform> ItemRects = new();
        public ContainerId Container { get; internal set; }
    }
}
