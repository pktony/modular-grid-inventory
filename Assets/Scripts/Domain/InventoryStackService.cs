namespace InventorySystem.Domain
{
    public sealed class InventoryStackService : IInventoryStackService
    {
        private readonly InventorySession session;
        private readonly InventoryPreviewCache preview = new();
        private readonly InventoryMutationPipeline pipeline;
        private readonly StackRules rules;
        private readonly InventoryPlacementRules placement;
        private readonly ItemInstanceFactory factory;
        internal InventoryStackService(InventorySession session, InventoryMutationPipeline pipeline, StackRules rules,
            InventoryPlacementRules placement, ItemInstanceFactory factory)
        { this.session = session; this.pipeline = pipeline; this.rules = rules; this.placement = placement; this.factory = factory; }
        public MutationResult PreviewMerge(MergeRequest request) => ValidateMerge(preview.Get(session.Snapshot), request);
        public MutationResult Merge(MergeRequest request) => pipeline.Execute(draft =>
        {
            var result = ValidateMerge(draft, request);
            if (!result.Success) return result;
            var source = draft.Items[request.Source]; var destination = draft.Items[request.Destination];
            draft.FindOwner(source.Id, out var sourceOwner, out _);
            draft.FindOwner(destination.Id, out var targetOwner, out _);
            draft.Items[destination.Id] = destination.WithQuantity(destination.Quantity + result.MovedQuantity);
            if (result.MovedQuantity == source.Quantity)
            { draft.Items.Remove(source.Id); draft.Containers[sourceOwner].Entries.Remove(source.Id); }
            else draft.Items[source.Id] = source.WithQuantity(source.Quantity - result.MovedQuantity);
            draft.Touch(sourceOwner, source.Id); draft.Touch(targetOwner, destination.Id);
            return result;
        });
        public MutationResult PreviewSplit(SplitRequest request) => ValidateSplit(preview.Get(session.Snapshot), request);
        public MutationResult Split(SplitRequest request) => pipeline.Execute(draft =>
        {
            var result = ValidateSplit(draft, request);
            if (!result.Success) return result;
            var source = draft.Items[request.Source]; var created = factory.Prepare(source.Definition, request.Quantity);
            var t = request.Target;
            draft.Items[source.Id] = source.WithQuantity(source.Quantity - request.Quantity);
            draft.Items.Add(created.Id, created);
            draft.Containers[t.Container].Entries.Add(created.Id, new InventoryEntry(created.Id, t.Section, t.X, t.Y, t.Rotated));
            draft.FindOwner(source.Id, out var owner, out _);
            draft.Touch(owner, source.Id); draft.Touch(t.Container, created.Id);
            return MutationResult.Ok(request.Quantity, created.Id);
        });
        private MutationResult ValidateMerge(InventoryDraft draft, MergeRequest request)
        {
            if (!draft.Items.TryGetValue(request.Source, out var source) || !draft.Items.TryGetValue(request.Destination, out var destination))
                return MutationResult.Fail("Stack no longer exists.");
            int moved = rules.MergeQuantity(source, destination);
            return moved > 0 ? MutationResult.Ok(moved) : MutationResult.Fail("Use the same item type and a stack below its limit.");
        }
        private MutationResult ValidateSplit(InventoryDraft draft, SplitRequest request)
        {
            if (!draft.Items.TryGetValue(request.Source, out var source) || !rules.CanSplit(source, request.Quantity))
                return MutationResult.Fail("Split quantity must be between 1 and the stack quantity minus 1.");
            var error = placement.Validate(draft, source, request.Target);
            return error == null ? MutationResult.Ok(request.Quantity) : MutationResult.Fail(error);
        }
    }
}
