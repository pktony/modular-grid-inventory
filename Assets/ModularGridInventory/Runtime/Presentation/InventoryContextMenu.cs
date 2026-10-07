using System;
using Pktony.GridInventory.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryContextMenu
    {
        private readonly RectTransform root, overlay;
        private readonly Button open, split, delete;
        private ItemInstanceId item;
        public bool IsOpen => root.gameObject.activeSelf;
        public event Action<ItemInstanceId> OpenRequested, SplitRequested, DeleteRequested;
        public InventoryContextMenu(RectTransform root, RectTransform overlay, Button open, Button split, Button delete)
        {
            this.root = root; this.overlay = overlay; this.open = open; this.split = split; this.delete = delete;
            open.onClick.AddListener(() => { Hide(); OpenRequested?.Invoke(item); });
            split.onClick.AddListener(() => { Hide(); SplitRequested?.Invoke(item); });
            delete.onClick.AddListener(() => { Hide(); DeleteRequested?.Invoke(item); }); Hide();
        }
        public void Show(ItemInstance selected, Vector2 point)
        {
            item = selected.Id; open.interactable = !selected.ChildContainerId.IsEmpty;
            split.interactable = selected.Definition.MaxStack > 1 && selected.Quantity > 1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay, point, null, out var local);
            root.anchoredPosition = new Vector2(Mathf.Clamp(local.x, 0, 1090), Mathf.Clamp(local.y, -500, -80));
            root.gameObject.SetActive(true); root.SetAsLastSibling();
        }
        public void Hide() => root.gameObject.SetActive(false);
    }
}
