namespace Pktony.GridInventory.Domain
{
    public interface IInventoryStorageService
    {
        MutationResult Preview(ContainerStoreRequest request, out PlacementTarget target);
        MutationResult Store(ContainerStoreRequest request);
    }
}
