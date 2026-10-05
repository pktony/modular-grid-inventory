using InventorySystem.Domain;
namespace InventorySystem.Presentation
{
    public readonly struct InventoryDropPreview
    {
        public MutationResult Result { get; }
        public PlacementTarget Target { get; }
        public ItemInstanceId Merge { get; }
        public ItemInstanceId ContainerItem { get; }
        public ContainerId StoreContainer { get; }
        public InventoryDropPreview(MutationResult result, PlacementTarget target = default, ItemInstanceId merge = default,
            ItemInstanceId containerItem = default, ContainerId storeContainer = default)
        { Result = result; Target = target; Merge = merge; ContainerItem = containerItem; StoreContainer = storeContainer; }
    }
}
