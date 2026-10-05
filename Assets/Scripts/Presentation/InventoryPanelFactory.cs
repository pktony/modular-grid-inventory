using System;
using InventorySystem.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryPanelFactory
    {
        public InventoryPanelBindings Create(Transform parent, string name, Vector2 position, InventoryInputEvents events)
        {
            var root = InventoryElementFactory.Panel(name, parent, position, new Vector2(608, 492), InventoryPalette.Panel).rectTransform;
            var viewport = InventoryElementFactory.Panel("Viewport", root, new Vector2(16, -66), new Vector2(576, 410), InventoryPalette.Background, true).rectTransform;
            viewport.gameObject.AddComponent<RectMask2D>(); viewport.gameObject.AddComponent<GridPointerHandler>().Initialize(events);
            var content = InventoryElementFactory.Rect("Content", viewport, Vector2.zero, new Vector2(560, 410));
            var scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport; scroll.content = content;
            scroll.horizontal = false; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 32;
            return new InventoryPanelBindings { Root = root, Viewport = viewport, Content = content, Scroll = scroll,
                Title = InventoryElementFactory.Label("Title", root, new Vector2(16, -12), new Vector2(560, 25), name, 20, InventoryPalette.Text),
                Policy = InventoryElementFactory.Label("Policy", root, new Vector2(16, -40), new Vector2(560, 21), "", 13, InventoryPalette.Muted) };
        }
        public void BuildSections(InventoryPanelBindings panel, ContainerState state, InventoryInputEvents events)
        {
            foreach (Transform child in panel.Content) { child.gameObject.SetActive(false); UnityEngine.Object.Destroy(child.gameObject); }
            panel.Sections.Clear(); panel.Container = state.Id;
            float height = 0;
            foreach (var layout in state.Definition.Layout.Sections)
            {
                var id = new GridSectionId(layout.SectionId); var definition = state.Sections[id].Definition;
                var position = new Vector2(layout.X * InventorySectionGeometry.Pitch, -layout.Y * InventorySectionGeometry.Pitch);
                var rect = InventoryElementFactory.Rect("Section-" + layout.SectionId, panel.Content, position,
                    new Vector2(definition.Width, definition.Height) * InventorySectionGeometry.Pitch);
                var cells = new Image[definition.Width * definition.Height];
                for (int y = 0; y < definition.Height; y++) for (int x = 0; x < definition.Width; x++)
                {
                    var cell = InventoryElementFactory.Panel("Cell", rect, new Vector2(x * 50, -y * 50), new Vector2(48, 48), InventoryPalette.Cell, true);
                    cell.gameObject.AddComponent<GridPointerHandler>().Initialize(events); cells[y * definition.Width + x] = cell;
                }
                var layer = InventoryElementFactory.Rect("Items", rect, Vector2.zero, rect.sizeDelta);
                panel.Sections.Add(id, new InventorySectionView(new InventorySectionGeometry(rect, definition), cells, layer));
                height = Mathf.Max(height, (layout.Y + definition.Height) * 50);
            }
            panel.Content.sizeDelta = new Vector2(560, Mathf.Max(410, height)); panel.Scroll.verticalNormalizedPosition = 1;
        }
    }
}
