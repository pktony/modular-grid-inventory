using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryItemViewFactory
    {
        public InventoryItemView Create(ItemData item, Transform parent, InventoryPointerEvents events)
        {
            var bg = UIElementFactory.Panel(item.itemId, parent, Vector2.zero, Vector2.one, InventoryTheme.Item, events != null);
            var image = UIElementFactory.Panel("Icon", bg.transform, Vector2.zero, Vector2.one, Color.white);
            image.sprite = item.Definition.Icon; image.preserveAspect = true;
            image.rectTransform.anchorMin = image.rectTransform.anchorMax = image.rectTransform.pivot = Vector2.one * 0.5f;
            var view = bg.gameObject.AddComponent<InventoryItemView>(); view.Initialize(bg, image);
            if (events != null) bg.gameObject.AddComponent<InventoryItemPointerHandler>().Initialize(item, events);
            return view;
        }
    }
}
