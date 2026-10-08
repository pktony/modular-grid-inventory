using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventoryGridHitTester : IInventoryGridHitTester
    {
        private readonly IInventoryPanelSource source;
        public InventoryGridHitTester(IInventoryPanelSource source) { this.source = source; }
        public bool TryHit(Vector2 screen, out PlacementTarget target)
        {
            target = default;
            foreach (var panel in source.FrontToBack)
            {
                if (!panel.Root.gameObject.activeInHierarchy || !RectTransformUtility.RectangleContainsScreenPoint(panel.Root, screen)) continue;
                if (!RectTransformUtility.RectangleContainsScreenPoint(panel.Viewport, screen)) return false;
                foreach (var section in panel.Sections.Values) if (section.Geometry.TryCell(screen, out var cell))
                { target = new PlacementTarget(panel.Container, section.Geometry.Id, cell.x, cell.y); return true; }
                return false;
            }
            return false;
        }
        public InventorySectionView Find(PlacementTarget target)
        {
            foreach (var panel in source.FrontToBack) if (panel.Container == target.Container && panel.Sections.TryGetValue(target.Section, out var section)) return section;
            return null;
        }
        public RectTransform FindItem(ItemInstanceId id)
        {
            foreach (var panel in source.FrontToBack) if (panel.ItemRects.TryGetValue(id, out var rect)) return rect;
            return null;
        }
    }
}
