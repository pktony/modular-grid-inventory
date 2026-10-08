using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventoryGridHitTester : IInventoryGridHitTester
    {
        private readonly InventoryPanelBindings[] panels;
        public InventoryGridHitTester(params InventoryPanelBindings[] panels) { this.panels = panels; }
        public bool TryHit(Vector2 screen, out PlacementTarget target)
        {
            foreach (var panel in panels)
            {
                if (!panel.Root.gameObject.activeInHierarchy || !RectTransformUtility.RectangleContainsScreenPoint(panel.Viewport, screen)) continue;
                foreach (var section in panel.Sections.Values) if (section.Geometry.TryCell(screen, out var cell))
                { target = new PlacementTarget(panel.Container, section.Geometry.Id, cell.x, cell.y); return true; }
            }
            target = default; return false;
        }
        public InventorySectionView Find(PlacementTarget target)
        {
            foreach (var panel in panels) if (panel.Container == target.Container && panel.Sections.TryGetValue(target.Section, out var section)) return section;
            return null;
        }
    }
}
