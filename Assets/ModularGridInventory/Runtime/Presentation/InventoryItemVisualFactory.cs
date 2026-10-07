using Pktony.GridInventory.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryItemVisualFactory
    {
        private readonly InventoryElementFactory elements;
        public InventoryItemVisualFactory(InventoryElementFactory elements) { this.elements = elements; }
        public InventoryItemVisual Create(Transform parent, ItemInstanceId id, InventoryInputEvents events)
        {
            var background = elements.Panel("Item-" + id, parent, Vector2.zero, Vector2.one, elements.Theme.Palette.Item, true);
            var group = background.gameObject.AddComponent<CanvasGroup>();
            var border = background.gameObject.AddComponent<Shadow>(); border.effectColor = elements.Theme.Palette.Border; border.effectDistance = new Vector2(1, -1);
            var outline = background.gameObject.AddComponent<Outline>(); outline.effectColor = elements.Theme.Palette.Accent;
            outline.effectDistance = new Vector2(2, -2); outline.enabled = false;
            background.gameObject.AddComponent<InventoryPointerHandler>().Initialize(id, events);
            var icon = elements.Panel("Icon", background.transform, Vector2.zero, Vector2.one, Color.white);
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            var quantity = elements.Label("Quantity", background.transform, Vector2.zero, new Vector2(44, 22), "", 17, Color.white);
            quantity.rectTransform.anchorMin = quantity.rectTransform.anchorMax = quantity.rectTransform.pivot = new Vector2(1, 0);
            quantity.alignment = TextAlignmentOptions.BottomRight; quantity.margin = new Vector4(0, 0, 4, 2);
            var title = elements.Label("ItemName", background.transform, new Vector2(4, -3), new Vector2(145, 16), "", 10, Color.white);
            return new InventoryItemVisual(background.rectTransform, background, icon, quantity, title, group, outline, elements.Theme);
        }
    }
}
