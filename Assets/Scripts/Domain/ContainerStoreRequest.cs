namespace InventorySystem.Domain
{
    public readonly struct ContainerStoreRequest
    {
        public ItemInstanceId Item { get; }
        public ContainerId Container { get; }
        public bool Rotated { get; }
        public int SplitQuantity { get; }
        public ContainerStoreRequest(ItemInstanceId item, ContainerId container, bool rotated = false, int splitQuantity = 0)
        { Item = item; Container = container; Rotated = rotated; SplitQuantity = splitQuantity; }
    }
}
