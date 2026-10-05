using InventorySystem.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryItemVisual
    {
        public RectTransform Rect { get; }
        private readonly Image background, icon;
        private readonly TextMeshProUGUI quantity, title;
        private readonly CanvasGroup group;
        public InventoryItemVisual(RectTransform rect, Image background, Image icon, TextMeshProUGUI quantity,
            TextMeshProUGUI title, CanvasGroup group)
        { Rect = rect; this.background = background; this.icon = icon; this.quantity = quantity; this.title = title; this.group = group; }
        public void Present(ItemInstance item, Domain.InventoryEntry entry, bool selected, bool dragging)
        {
            int w = entry.Rotated ? item.Definition.Height : item.Definition.Width;
            int h = entry.Rotated ? item.Definition.Width : item.Definition.Height;
            Rect.anchoredPosition = new Vector2(entry.X * 50, -entry.Y * 50); Rect.sizeDelta = InventorySectionGeometry.Size(w, h);
            icon.sprite = item.Definition.Icon; icon.preserveAspect = true;
            icon.rectTransform.sizeDelta = entry.Rotated ? new Vector2(Rect.sizeDelta.y - 6, Rect.sizeDelta.x - 6) : Rect.sizeDelta - new Vector2(6, 6);
            icon.rectTransform.localRotation = Quaternion.Euler(0, 0, entry.Rotated ? -90 : 0);
            background.color = selected ? InventoryPalette.Accent : InventoryPalette.Item;
            group.alpha = dragging ? 0.3f : 1;
            quantity.text = item.Definition.MaxStack > 1 ? item.Quantity.ToString() : item.Definition.Container != null ? "+" : "";
            title.text = w >= 3 ? item.Definition.DisplayName : "";
        }
        public void SetSelected(bool selected) => background.color = selected ? InventoryPalette.Accent : InventoryPalette.Item;
        public void SetDragging(bool dragging) => group.alpha = dragging ? 0.3f : 1;
    }
}
