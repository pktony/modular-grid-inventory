using TMPro;
using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryTooltipPresenter
    {
        private readonly RectTransform root, overlay;
        private readonly TextMeshProUGUI label;
        public InventoryTooltipPresenter(RectTransform overlay)
        {
            this.overlay = overlay;
            root = UIElementFactory.Panel("Tooltip", overlay, Vector2.zero, new Vector2(240,80), InventoryTheme.Panel).rectTransform;
            label = UIElementFactory.Label("Info", root, new(12,-10), new(216,64), "", 16, InventoryTheme.Text);
            root.gameObject.SetActive(false);
        }
        public void Show(ItemData item)
        {
            root.gameObject.SetActive(item != null);
            if (item == null) return;
            var size = item.GetAbsoluteSize();
            label.text = $"{item.Definition.DisplayName}\n{size.absWidth} x {size.absHeight} cells";
            root.SetAsLastSibling();
        }
        public void Move(Vector2 screenPosition)
        {
            if (!root.gameObject.activeSelf) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay, screenPosition, null, out var local);
            root.anchoredPosition = new Vector2(Mathf.Clamp(local.x + 18, 0, overlay.rect.width - 240),
                Mathf.Clamp(local.y - 18, -overlay.rect.height + 80, 0));
        }
    }
}
