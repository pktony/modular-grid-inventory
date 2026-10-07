using System;
namespace InventorySystem
{
    public interface IDemoInventoryView
    {
        event Action AddRequested, ResetRequested;
        void SetStatus(string message);
    }
}
