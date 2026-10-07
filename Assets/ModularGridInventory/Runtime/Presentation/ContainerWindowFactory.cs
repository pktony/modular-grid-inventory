using System;
using System.Linq;
using Pktony.GridInventory.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class ContainerWindowFactory
    {
        private readonly InventoryElementFactory elements;
        private readonly RectTransform desktop;
        private readonly InventoryPanelFactory panels;
        private readonly InventoryItemVisualFactory items;
        private readonly InventoryInputEvents events;
        public ContainerWindowFactory(RectTransform desktop, InventoryPanelFactory panels, InventoryItemVisualFactory items, InventoryInputEvents events, InventoryElementFactory elements)
        { this.elements = elements; this.desktop = desktop; this.panels = panels; this.items = items; this.events = events; }
        public ContainerWindowPresenter Create(InventorySnapshot snapshot, ItemInstance owner, int index, Action focus, Action close)
        {
            var definition = snapshot.Containers[owner.ChildContainerId].Definition;
            float width = definition.Layout.Sections.Max(layout => (layout.X + definition.Sections.First(s => s.Id == layout.SectionId).Width) * elements.Theme.CellPitch);
            float height = definition.Layout.Sections.Max(layout => (layout.Y + definition.Sections.First(s => s.Id == layout.SectionId).Height) * elements.Theme.CellPitch);
            var size = new Vector2(Mathf.Clamp(width + 16, 216, 736), Mathf.Clamp(height + 52, 152, 548));
            var position = new Vector2(24 + index % 5 * 34, -110 - index % 5 * 24);
            var panel = panels.Create(desktop, "Container-" + owner.ChildContainerId, position, events, size, 44);
            var frame = panel.Root.gameObject.AddComponent<Outline>(); frame.effectDistance = new Vector2(1, -1);
            var title = elements.Panel("WindowHeader", panel.Root, new Vector2(1, -1), new Vector2(size.x - 2, 25), elements.Theme.Palette.Title, true);
            panel.Title.rectTransform.SetParent(title.transform, false); panel.Title.rectTransform.anchoredPosition = new Vector2(8, -3);
            panel.Title.rectTransform.sizeDelta = new Vector2(size.x - 38, 20); panel.Title.fontSize = 13; panel.Title.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
            panel.Policy.rectTransform.anchoredPosition = new Vector2(8, -27); panel.Policy.fontSize = 11;
            var button = elements.Button("CloseWindow", title.transform, new Vector2(size.x - 25, -2), new Vector2(21, 21), "X");
            var colors = button.colors; colors.normalColor = elements.Theme.Palette.Close; colors.highlightedColor = elements.Theme.Palette.Invalid; button.colors = colors;
            button.onClick.AddListener(() => close());
            title.gameObject.AddComponent<ContainerWindowDragHandler>().Initialize(panel.Root, desktop, focus);
            panel.Root.gameObject.AddComponent<ContainerWindowFocusHandler>().Initialize(focus);
            title.transform.SetAsLastSibling();
            return new ContainerWindowPresenter(owner, panel, title.rectTransform, button,
                new InventoryPanelPresenter(panel, panels, items, events), frame, title, elements.Theme.Palette);
        }
    }
}
