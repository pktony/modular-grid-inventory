using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryElementFactory
    {
        public InventoryUiSettings Theme { get; }
        public InventoryElementFactory(InventoryUiSettings theme) { Theme = theme; }
        public RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }
        public Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color, bool raycast = false)
        {
            var image = Rect(name, parent, position, size).gameObject.AddComponent<Image>();
            image.color = color; image.raycastTarget = raycast; return image;
        }
        public TextMeshProUGUI Label(string name, Transform parent, Vector2 position, Vector2 size, string text, int fontSize, Color color)
        {
            var label = Rect(name, parent, position, size).gameObject.AddComponent<TextMeshProUGUI>();
            label.font = Theme.Font;
            label.fontSharedMaterial = label.font.material;
            label.fontStyle = FontStyles.Normal;
            label.fontWeight = FontWeight.Regular;
            label.text = text; label.fontSize = fontSize; label.color = color;
            label.raycastTarget = false; label.textWrappingMode = TextWrappingModes.Normal;
            return label;
        }
        public Button Button(string name, Transform parent, Vector2 position, Vector2 size, string text)
        {
            var panel = Panel(name, parent, position, size, Theme.Palette.Cell, true);
            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.navigation = new Navigation { mode = Navigation.Mode.Automatic };
            panel.color = Color.white;
            var colors = button.colors; colors.normalColor = Theme.Palette.Cell;
            colors.highlightedColor = new Color(0.2f, 0.26f, 0.22f); colors.selectedColor = new Color(0.28f, 0.34f, 0.23f);
            colors.pressedColor = new Color(0.34f, 0.41f, 0.26f); colors.disabledColor = new Color(0.08f, 0.1f, 0.09f);
            button.colors = colors;
            var label = Label("Label", panel.transform, Vector2.zero, size, text, 15, Theme.Palette.Text);
            label.alignment = TextAlignmentOptions.Center;
            return button;
        }
    }
}
