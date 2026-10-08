namespace InventorySystem.Domain
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
            draft.FindOwner(request.Item, out var owner, out _);
            draft.Containers[owner].Entries.Remove(request.Item);
            var t = request.Target;
            draft.Containers[t.Container].Entries.Add(request.Item, new InventoryEntry(request.Item, t.Section, t.X, t.Y, t.Rotated));
            draft.Touch(owner, request.Item); draft.Touch(t.Container, request.Item);
            return result;
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
