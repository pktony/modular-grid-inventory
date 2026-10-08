using System;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class ClickIndicator : IDisposable
    {
        private readonly RectTransform root, overlay;
        private readonly Texture2D texture;
        private readonly Sprite sprite;
        public ClickIndicator(RectTransform overlay, InventoryElementFactory elements)
        {
            this.overlay = overlay;
            var image = elements.Panel("ClickIndicator", overlay, Vector2.zero, new Vector2(30, 30), Color.white);
            root = image.rectTransform; root.pivot = new Vector2(0.5f, 0.5f);
            texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
            {
                float radius = Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f));
                texture.SetPixel(x, y, new Color(0.95f, 0.97f, 0.97f, Mathf.Clamp01(2.5f - Mathf.Abs(radius - 26)) * 0.9f));
            }
            texture.Apply(); sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f)); image.sprite = sprite;
            Show(Vector2.zero, false);
        }
        public void Show(Vector2 point, bool pressed)
        {
            root.gameObject.SetActive(pressed); if (!pressed) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay, point, null, out var local);
            root.anchoredPosition = local; root.SetAsLastSibling();
        }
        public void Dispose() { if (root != null) UnityEngine.Object.Destroy(root.gameObject); UnityEngine.Object.Destroy(sprite); UnityEngine.Object.Destroy(texture); }
    }
}
