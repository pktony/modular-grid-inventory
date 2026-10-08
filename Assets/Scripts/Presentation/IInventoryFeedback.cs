using System;
using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public interface IInventoryFeedback
    {
        ItemInstanceId Selected { get; }
        event Action WindowClosing;
        void Select(ItemInstanceId id);
        void SetDrag(ItemInstanceId id);
        void Status(string text, bool valid = true);
        void Open(ItemInstanceId id);
        void FocusAt(Vector2 point);
        void CloseFrontmost();
    }
}
