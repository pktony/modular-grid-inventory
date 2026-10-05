using System.Collections.Generic;
using InventorySystem.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryPanelBindings
    {
        public RectTransform Root, Content, Viewport;
        public ScrollRect Scroll;
        public TextMeshProUGUI Title, Policy;
        public readonly Dictionary<GridSectionId, InventorySectionView> Sections = new();
        public ContainerId Container { get; internal set; }
    }
}
