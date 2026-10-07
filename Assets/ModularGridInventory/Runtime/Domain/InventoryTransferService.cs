namespace Pktony.GridInventory.Domain
{
    public sealed class InventoryTransferService : IInventoryTransferService
    {
        private readonly InventorySession session;
        private readonly InventoryPreviewCache preview = new();
        private readonly InventoryMutationPipeline pipeline;
        private readonly InventoryPlacementRules placement;
        internal InventoryTransferService(InventorySession session, InventoryMutationPipeline pipeline, InventoryPlacementRules placement)
        { this.session = session; this.pipeline = pipeline; this.placement = placement; }
        public MutationResult Preview(TransferRequest request) => Validate(preview.Get(session.Snapshot), request);
        public MutationResult Transfer(TransferRequest request) => pipeline.Execute(draft =>
        {
            var result = Validate(draft, request);
            if (!result.Success) return result;
            return InventoryTransferOperation.Apply(draft, request);
        });
        private MutationResult Validate(InventoryDraft draft, TransferRequest request)
        {
            if (!draft.Items.TryGetValue(request.Item, out var item) || !draft.FindOwner(request.Item, out _, out _))
                return MutationResult.Fail("Item no longer exists.");
            var error = placement.Validate(draft, item, request.Target, item.Id);
            return error == null ? MutationResult.Ok(item.Quantity) : MutationResult.Fail(error);
        }
    }
}
