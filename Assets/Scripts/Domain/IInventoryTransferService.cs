namespace InventorySystem.Domain
{
    public interface IInventoryTransferService
    {
        MutationResult Transfer(TransferRequest request);
        MutationResult Preview(TransferRequest request);
    }
}
