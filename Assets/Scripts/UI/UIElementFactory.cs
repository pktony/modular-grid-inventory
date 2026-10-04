using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    public static class UIElementFactory
    {
        public static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }
        public static Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color, bool raycast = false)
        {
            var image = Rect(name, parent, position, size).gameObject.AddComponent<Image>();
            image.color = color; image.raycastTarget = raycast; return image;
        }
        public static TextMeshProUGUI Label(string name, Transform parent, Vector2 position, Vector2 size, string text, int fontSize, Color color)
        {
            var label = Rect(name, parent, position, size).gameObject.AddComponent<TextMeshProUGUI>();
            label.text = text; label.fontSize = fontSize; label.color = color;
            label.raycastTarget = false; label.textWrappingMode = TextWrappingModes.Normal;
            return label;
        }
        public static Button Button(string name, Transform parent, Vector2 position, Vector2 size, string text)
        {
            var panel = Panel(name, parent, position, size, InventoryTheme.Cell, true);
            var button = panel.gameObject.AddComponent<Button>();
            button.targetGraphic = panel;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = button.colors; colors.highlightedColor = InventoryTheme.Accent; button.colors = colors;
            var label = Label("Label", panel.transform, Vector2.zero, size, text, 15, InventoryTheme.Text);
            label.alignment = TextAlignmentOptions.Center;
            return button;
        }
    }
}
