namespace Pktony.GridInventory.Domain
{
    public interface IInventoryEditService
    {
        MutationResult Add(AddRequest request);
        MutationResult Delete(ItemInstanceId item);
    }
}
