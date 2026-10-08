namespace Pktony.GridInventory.Domain
{
    public sealed class ItemInstance
    {
        public ItemInstanceId Id { get; }
        public ItemDefinitionView Definition { get; }
        public int Quantity { get; }
        public ContainerId ChildContainerId { get; }
        internal ItemInstance(ItemInstanceId id, ItemDefinitionView definition, int quantity, ContainerId child = default)
        { Id = id; Definition = definition; Quantity = quantity; ChildContainerId = child; }
        internal ItemInstance WithQuantity(int quantity) => new(Id, Definition, quantity, ChildContainerId);
    }
}
