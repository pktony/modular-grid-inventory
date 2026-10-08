using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class ContainerBreadcrumbPresenter
    {
        private readonly RectTransform root;
        private readonly ScrollRect scroll;
        private readonly ContainerBreadcrumbFactory factory;
        private string key;
        public ContainerBreadcrumbPresenter(RectTransform root, ScrollRect scroll, ContainerBreadcrumbFactory factory)
        { this.root = root; this.scroll = scroll; this.factory = factory; }
        public void Render(InventorySnapshot snapshot, IReadOnlyList<ItemInstanceId> path, Action<ItemInstanceId> navigate)
        {
            string next = string.Join("/", path.Select(id => id.Value)); if (next == key) return; key = next;
            foreach (Transform child in root) { child.gameObject.SetActive(false); UnityEngine.Object.Destroy(child.gameObject); }
            for (int i = 0; i < path.Count; i++) factory.Create(root, path[i], snapshot.Items[path[i]].Definition.DisplayName, i, navigate);
            root.sizeDelta = new Vector2(Mathf.Max(472, path.Count * 146), 32); scroll.horizontalNormalizedPosition = 1;
        }
    }
}
