namespace Pktony.GridInventory.Domain
{
    public sealed class InventoryStorageService : IInventoryStorageService
    {
        private readonly InventorySession session;
        private readonly InventoryPreviewCache preview = new();
        private readonly InventoryMutationPipeline pipeline;
        private readonly InventoryContainerPlacementFinder finder;
        private readonly InventorySplitOperation split;
        private readonly StackRules stacks;
        internal InventoryStorageService(InventorySession session, InventoryMutationPipeline pipeline,
            InventoryContainerPlacementFinder finder, InventorySplitOperation split, StackRules stacks)
        { this.session = session; this.pipeline = pipeline; this.finder = finder; this.split = split; this.stacks = stacks; }
        public MutationResult Preview(ContainerStoreRequest request, out PlacementTarget target)
            => Validate(preview.Get(session.Snapshot), request, out target);
        public MutationResult Store(ContainerStoreRequest request) => pipeline.Execute(draft =>
        {
            var result = Validate(draft, request, out var target);
            if (!result.Success) return result;
            return request.SplitQuantity > 0 ? split.Apply(draft, new SplitRequest(request.Item, request.SplitQuantity, target))
                : InventoryTransferOperation.Apply(draft, new TransferRequest(request.Item, target));
        });
        private MutationResult Validate(InventoryDraft draft, ContainerStoreRequest request, out PlacementTarget target)
        {
            target = default;
            if (!draft.Items.TryGetValue(request.Item, out var item) || !draft.FindOwner(request.Item, out _, out _))
                return MutationResult.Fail("Item no longer exists.");
            if (request.SplitQuantity < 0 || request.SplitQuantity > 0 && !stacks.CanSplit(item, request.SplitQuantity))
                return MutationResult.Fail("Split quantity must be between 1 and the stack quantity minus 1.");
            return finder.Find(draft, item, request, out target);
        }
    }
}
