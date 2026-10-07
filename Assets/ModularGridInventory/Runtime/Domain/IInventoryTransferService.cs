namespace Pktony.GridInventory.Domain
{
    public interface IInventoryTransferService
    {
        MutationResult Transfer(TransferRequest request);
        MutationResult Preview(TransferRequest request);
    }
}
