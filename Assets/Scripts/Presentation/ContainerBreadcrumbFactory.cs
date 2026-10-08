using System;
using InventorySystem.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class ContainerBreadcrumbFactory
    {
        public Button Create(RectTransform parent, ItemInstanceId id, string title, int index, Action<ItemInstanceId> navigate)
        {
            var button = InventoryElementFactory.Button("Path-" + id, parent, new Vector2(index * 146, 0), new Vector2(142, 32), title);
            button.onClick.AddListener(() => navigate(id)); return button;
        }
    }
}
