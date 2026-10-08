namespace Pktony.GridInventory.Domain
{
    public readonly struct PlacementTarget
    {
        public ContainerId Container { get; }
        public GridSectionId Section { get; }
        public int X { get; }
        public int Y { get; }
        public bool Rotated { get; }
        public PlacementTarget(ContainerId container, GridSectionId section, int x, int y, bool rotated = false)
        { Container = container; Section = section; X = x; Y = y; Rotated = rotated; }
    }
    public readonly struct TransferRequest
    {
        public ItemInstanceId Item { get; }
        public PlacementTarget Target { get; }
        public TransferRequest(ItemInstanceId item, PlacementTarget target) { Item = item; Target = target; }
    }
    public readonly struct MergeRequest
    {
        public ItemInstanceId Source { get; }
        public ItemInstanceId Destination { get; }
        public MergeRequest(ItemInstanceId source, ItemInstanceId destination) { Source = source; Destination = destination; }
    }
    public readonly struct SplitRequest
    {
        public ItemInstanceId Source { get; }
        public int Quantity { get; }
        public PlacementTarget Target { get; }
        public SplitRequest(ItemInstanceId source, int quantity, PlacementTarget target)
        { Source = source; Quantity = quantity; Target = target; }
    }
    public readonly struct AddRequest
    {
        public DefinitionId Definition { get; }
        public int Quantity { get; }
        public PlacementTarget Target { get; }
        public AddRequest(DefinitionId definition, int quantity, PlacementTarget target)
        { Definition = definition; Quantity = quantity; Target = target; }
    }
}
