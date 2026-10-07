using System;
using Pktony.GridInventory.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryPanelFactory
    {
        private readonly InventoryElementFactory elements;
        public InventoryPanelFactory(InventoryElementFactory elements) { this.elements = elements; }
        public InventoryPanelBindings Create(Transform parent, string name, Vector2 position, InventoryInputEvents events, Vector2 size = default, float header = 58)
        {
            if (size == default) size = new Vector2(474, 566);
            var root = elements.Panel(name, parent, position, size, elements.Theme.Palette.Panel, true).rectTransform;
            float width = size.x - 16, height = size.y - header - 8;
            var viewport = elements.Panel("Viewport", root, new Vector2(8, -header), new Vector2(width, height), elements.Theme.Palette.Background, true).rectTransform;
            viewport.gameObject.AddComponent<RectMask2D>(); viewport.gameObject.AddComponent<GridPointerHandler>().Initialize(events);
            var content = elements.Rect("Content", viewport, Vector2.zero, new Vector2(width, height));
            var scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport; scroll.content = content;
            scroll.horizontal = true; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped; scroll.scrollSensitivity = 32;
            var bar = elements.Panel("ScrollBar", root, new Vector2(size.x - 7, -header), new Vector2(5, height), elements.Theme.Palette.Panel, true);
            var scrollbar = bar.gameObject.AddComponent<Scrollbar>(); scrollbar.direction = Scrollbar.Direction.BottomToTop;
            var handle = elements.Panel("Handle", bar.transform, Vector2.zero, new Vector2(5, 100), elements.Theme.Palette.Muted, true);
            scrollbar.handleRect = handle.rectTransform; scrollbar.targetGraphic = handle;
            scroll.verticalScrollbar = scrollbar; scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            var horizontalBar = elements.Panel("HorizontalScrollBar", root, new Vector2(8, -size.y + 6), new Vector2(width, 4), elements.Theme.Palette.Panel, true);
            var horizontal = horizontalBar.gameObject.AddComponent<Scrollbar>(); horizontal.direction = Scrollbar.Direction.LeftToRight;
            var horizontalHandle = elements.Panel("Handle", horizontalBar.transform, Vector2.zero, new Vector2(100, 4), elements.Theme.Palette.Muted, true);
            horizontal.handleRect = horizontalHandle.rectTransform; horizontal.targetGraphic = horizontalHandle;
            scroll.horizontalScrollbar = horizontal; scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            return new InventoryPanelBindings { Root = root, Viewport = viewport, Content = content, Scroll = scroll,
                Title = elements.Label("Title", root, new Vector2(8, -6), new Vector2(width, 25), name, 20, elements.Theme.Palette.Text),
                Policy = elements.Label("Policy", root, new Vector2(8, -34), new Vector2(width, 18), "", 12, elements.Theme.Palette.Muted) };
        }

        public void BuildSections(InventoryPanelBindings panel, ContainerState state, InventoryInputEvents events)
        {
            foreach (Transform child in panel.Content) { child.gameObject.SetActive(false); UnityEngine.Object.Destroy(child.gameObject); }
            panel.Sections.Clear(); panel.ItemRects.Clear(); panel.Container = state.Id;
            float height = 0, width = 0;
            foreach (var layout in state.Definition.Layout.Sections)
            {
                var id = new GridSectionId(layout.SectionId); var definition = state.Sections[id].Definition;
                var position = new Vector2(layout.X * elements.Theme.CellPitch, -layout.Y * elements.Theme.CellPitch);
                var rect = elements.Rect("Section-" + layout.SectionId, panel.Content, position,
                    new Vector2(definition.Width, definition.Height) * elements.Theme.CellPitch);
                InventoryPocketOutline.Create(elements, rect, definition.Width, definition.Height);
                var cells = new Image[definition.Width * definition.Height];
                for (int y = 0; y < definition.Height; y++) for (int x = 0; x < definition.Width; x++)
                {
                    var cell = elements.Panel("Cell", rect, new Vector2(x, -y) * elements.Theme.CellPitch, Vector2.one * (elements.Theme.CellPitch - elements.Theme.CellGap), elements.Theme.Palette.Cell, true);
                    cell.gameObject.AddComponent<GridPointerHandler>().Initialize(events); cells[y * definition.Width + x] = cell;
                }
                var layer = elements.Rect("Items", rect, Vector2.zero, rect.sizeDelta);
                panel.Sections.Add(id, new InventorySectionView(new InventorySectionGeometry(rect, definition, elements.Theme.CellPitch), cells, layer, elements.Theme.Palette));
                height = Mathf.Max(height, (layout.Y + definition.Height) * elements.Theme.CellPitch);
                width = Mathf.Max(width, (layout.X + definition.Width) * elements.Theme.CellPitch);
            }
            panel.Content.sizeDelta = new Vector2(Mathf.Max(panel.Viewport.rect.width, width), Mathf.Max(panel.Viewport.rect.height, height));
            panel.Scroll.verticalNormalizedPosition = 1; panel.Scroll.horizontalNormalizedPosition = 0;
        }
    }
}
