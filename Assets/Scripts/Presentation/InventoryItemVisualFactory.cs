using InventorySystem.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryItemVisualFactory
    {
        public InventoryItemVisual Create(Transform parent, ItemInstanceId id, InventoryInputEvents events)
        {
            var background = InventoryElementFactory.Panel("Item-" + id, parent, Vector2.zero, Vector2.one, InventoryPalette.Item, true);
            var group = background.gameObject.AddComponent<CanvasGroup>();
            var outline = background.gameObject.AddComponent<Outline>(); outline.effectColor = InventoryPalette.Accent;
            outline.effectDistance = new Vector2(2, -2); outline.enabled = false;
            background.gameObject.AddComponent<InventoryPointerHandler>().Initialize(id, events);
            var icon = InventoryElementFactory.Panel("Icon", background.transform, Vector2.zero, Vector2.one, Color.white);
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            var quantity = InventoryElementFactory.Label("Quantity", background.transform, Vector2.zero, new Vector2(44, 22), "", 17, Color.white);
            quantity.rectTransform.anchorMin = quantity.rectTransform.anchorMax = quantity.rectTransform.pivot = new Vector2(1, 0);
            quantity.alignment = TextAlignmentOptions.BottomRight; quantity.margin = new Vector4(0, 0, 4, 2);
            var title = InventoryElementFactory.Label("ItemName", background.transform, new Vector2(4, -3), new Vector2(145, 18), "", 11, Color.white);
            return new InventoryItemVisual(background.rectTransform, background, icon, quantity, title, group, outline);
        }
    }
}
