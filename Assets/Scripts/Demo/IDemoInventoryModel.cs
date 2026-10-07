namespace InventorySystem
{
    public interface IDemoInventoryModel
    {
        bool TryAdd(ItemData item);
        bool TryAdd(ItemData item, int x, int y);
        void Clear();
    }
}
