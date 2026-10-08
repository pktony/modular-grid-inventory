using InventorySystem.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryDragPresenter
    {
        private readonly RectTransform overlay, root;
        private readonly Image background, icon;
        private readonly TextMeshProUGUI count;
        private InventorySectionView highlighted;
        private string highlightKey;
        public InventoryDragPresenter(RectTransform overlay, RectTransform root, Image background, Image icon, TextMeshProUGUI count)
        { this.overlay = overlay; this.root = root; this.background = background; this.icon = icon; this.count = count; root.gameObject.SetActive(false); }
        public void Show(ItemInstance item, InventoryDragState drag, Vector2 point, bool valid)
        {
            root.gameObject.SetActive(true); root.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay, point, null, out var local);
            root.anchoredPosition = local - new Vector2((drag.Grip.x + drag.Fraction.x) * 50, -(drag.Grip.y + drag.Fraction.y) * 50);
            root.sizeDelta = InventorySectionGeometry.Size(drag.Width, drag.Height);
            icon.sprite = item.Definition.Icon; icon.preserveAspect = true;
            icon.rectTransform.sizeDelta = drag.Rotated ? new Vector2(root.sizeDelta.y - 6, root.sizeDelta.x - 6) : root.sizeDelta - new Vector2(6, 6);
            icon.rectTransform.localRotation = Quaternion.Euler(0, 0, drag.Rotated ? -90 : 0);
            background.color = valid ? InventoryPalette.Valid : InventoryPalette.Invalid;
            count.text = item.Definition.MaxStack > 1 ? (drag.SplitQuantity > 0 ? drag.SplitQuantity : item.Quantity).ToString() : "";
        }
        public void Highlight(InventorySectionView section, PlacementTarget target, int width, int height, bool valid)
        {
            string key = section == null ? "" : $"{target.Container}:{target.Section}:{target.X}:{target.Y}:{width}:{height}:{valid}";
            if (key == highlightKey) return;
            highlighted?.Clear(); highlighted = section; highlightKey = key;
            highlighted?.Highlight(target.X, target.Y, width, height, valid);
        }
        public void Clear() { highlighted?.Clear(); highlighted = null; highlightKey = null; root.gameObject.SetActive(false); }
    }
}
