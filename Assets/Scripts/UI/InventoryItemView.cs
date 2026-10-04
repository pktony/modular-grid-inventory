using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryItemView : MonoBehaviour
    {
        private Image background, icon;
        private CanvasGroup group;
        public RectTransform Rect => (RectTransform)transform;
        public void Initialize(Image bg, Image image)
        { background = bg; icon = image; group = gameObject.AddComponent<CanvasGroup>(); }
        public void Present(Vector2 size, ItemDirection direction, bool selected, float alpha = 1)
        {
            Rect.sizeDelta = size; group.alpha = alpha;
            background.color = selected ? InventoryTheme.Accent : InventoryTheme.Item;
            var r = icon.rectTransform;
            r.sizeDelta = direction == ItemDirection.Horizontal ? size - Vector2.one * 8 : new Vector2(size.y - 8, size.x - 8);
            r.localEulerAngles = new Vector3(0, 0, direction == ItemDirection.Horizontal ? 0 : -90);
        }
    }
}
