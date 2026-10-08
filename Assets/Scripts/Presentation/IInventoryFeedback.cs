using System;
using InventorySystem.Domain;
namespace InventorySystem.Presentation
{
    public interface IInventoryFeedback
    {
        ItemInstanceId Selected { get; }
        event Action<ItemInstanceId> NavigationRequested;
        void Select(ItemInstanceId id);
        void SetDrag(ItemInstanceId id);
        void Status(string text, bool valid = true);
        void Open(ItemInstanceId id);
        void Back();
        void Close();
    }
}
