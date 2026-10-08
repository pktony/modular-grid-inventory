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
            scroll.horizontal = true; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 32;
            var bar = InventoryElementFactory.Panel("ScrollBar", root, new Vector2(588, -66), new Vector2(12, 410), InventoryPalette.Cell, true);
            var scrollbar = bar.gameObject.AddComponent<Scrollbar>(); scrollbar.direction = Scrollbar.Direction.BottomToTop;
            var handle = InventoryElementFactory.Panel("Handle", bar.transform, Vector2.zero, new Vector2(12, 100), InventoryPalette.Muted, true);
            scrollbar.handleRect = handle.rectTransform; scrollbar.targetGraphic = handle;
            scroll.verticalScrollbar = scrollbar; scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            var horizontalBar = InventoryElementFactory.Panel("HorizontalScrollBar", root, new Vector2(16, -478), new Vector2(576, 10), InventoryPalette.Cell, true);
            var horizontal = horizontalBar.gameObject.AddComponent<Scrollbar>(); horizontal.direction = Scrollbar.Direction.LeftToRight;
            var horizontalHandle = InventoryElementFactory.Panel("Handle", horizontalBar.transform, Vector2.zero, new Vector2(100, 10), InventoryPalette.Muted, true);
            horizontal.handleRect = horizontalHandle.rectTransform; horizontal.targetGraphic = horizontalHandle;
            scroll.horizontalScrollbar = horizontal; scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            return new InventoryPanelBindings { Root = root, Viewport = viewport, Content = content, Scroll = scroll,
                Title = InventoryElementFactory.Label("Title", root, new Vector2(16, -12), new Vector2(560, 25), name, 20, InventoryPalette.Text),
                Policy = InventoryElementFactory.Label("Policy", root, new Vector2(16, -40), new Vector2(560, 21), "", 13, InventoryPalette.Muted) };
        }
        public void BuildSections(InventoryPanelBindings panel, ContainerState state, InventoryInputEvents events)
        {
            foreach (Transform child in panel.Content) { child.gameObject.SetActive(false); UnityEngine.Object.Destroy(child.gameObject); }
            panel.Sections.Clear(); panel.Container = state.Id;
            float height = 0, width = 0;
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
                width = Mathf.Max(width, (layout.X + definition.Width) * 50);
            }
            panel.Content.sizeDelta = new Vector2(Mathf.Max(560, width), Mathf.Max(410, height));
            panel.Scroll.verticalNormalizedPosition = 1; panel.Scroll.horizontalNormalizedPosition = 0;
        }
    }
}
