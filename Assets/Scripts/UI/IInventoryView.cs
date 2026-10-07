using System;
using UnityEngine;
namespace InventorySystem
{
    public interface IInventoryView
    {
        InventoryPointerEvents PointerEvents { get; }
        event Action RemoveRequested;
        Vector2Int CellAt(Vector2 screenPosition);
        bool IsOverGrid(Vector2 screenPosition);
        void SetSelection(ItemData item);
        void ShowItem(ItemData item);
        void SetStatus(string message);
        void ShowPreview(ItemData item, int x, int y, ItemDirection direction, bool valid);
        void ClearPreview();
    }
}
