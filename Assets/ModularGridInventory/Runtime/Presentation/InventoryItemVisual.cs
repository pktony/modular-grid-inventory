using Pktony.GridInventory.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryItemVisual
    {
        public RectTransform Rect { get; }
        private readonly Image background, icon;
        private readonly InventoryUiSettings theme;
        private readonly Outline outline;
        private readonly TextMeshProUGUI quantity, title;
        private readonly CanvasGroup group;
        public InventoryItemVisual(RectTransform rect, Image background, Image icon, TextMeshProUGUI quantity,
            TextMeshProUGUI title, CanvasGroup group, Outline outline, InventoryUiSettings theme)
        { this.theme = theme; Rect = rect; this.background = background; this.icon = icon; this.quantity = quantity; this.title = title; this.group = group; this.outline = outline; }
        public void Present(ItemInstance item, Domain.InventoryEntry entry, bool selected, bool dragging)
        {
            int w = entry.Rotated ? item.Definition.Height : item.Definition.Width;
            int h = entry.Rotated ? item.Definition.Width : item.Definition.Height;
            Rect.anchoredPosition = new Vector2(entry.X * theme.CellPitch, -entry.Y * theme.CellPitch); Rect.sizeDelta = InventorySectionGeometry.Size(w, h, theme.CellPitch, theme.CellGap);
            icon.sprite = item.Definition.Icon; icon.preserveAspect = true;
            icon.rectTransform.sizeDelta = entry.Rotated ? new Vector2(Rect.sizeDelta.y - 6, Rect.sizeDelta.x - 6) : Rect.sizeDelta - new Vector2(6, 6);
            icon.rectTransform.localRotation = Quaternion.Euler(0, 0, entry.Rotated ? -90 : 0);
            SetSelected(selected);
            group.alpha = dragging ? 0.3f : 1;
            quantity.text = item.Definition.MaxStack > 1 ? item.Quantity.ToString() : "";
            title.rectTransform.sizeDelta = new Vector2(Rect.sizeDelta.x - 8, 16);
            title.textWrappingMode = TextWrappingModes.NoWrap; title.overflowMode = TextOverflowModes.Ellipsis;
            title.alignment = TextAlignmentOptions.TopRight;
            title.text = item.Definition.DisplayName;
        }
        public void SetSelected(bool selected) { background.color = theme.Palette.Item; outline.enabled = selected; }
        public void SetDragging(bool dragging) => group.alpha = dragging ? 0.3f : 1;
    }
}
