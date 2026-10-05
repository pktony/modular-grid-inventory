namespace InventorySystem.Domain
{
    public interface IInventoryEditService
    {
        MutationResult Add(AddRequest request);
        MutationResult Delete(ItemInstanceId item);
    }
}
