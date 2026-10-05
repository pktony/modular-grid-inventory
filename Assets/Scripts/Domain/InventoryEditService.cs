namespace InventorySystem.Domain
{
    public sealed class InventoryEditService : IInventoryEditService
    {
        private readonly InventoryCatalog catalog;
        private readonly InventoryMutationPipeline pipeline;
        private readonly InventoryPlacementRules placement;
        private readonly ItemInstanceFactory factory;
        internal InventoryEditService(InventoryCatalog catalog, InventoryMutationPipeline pipeline,
            InventoryPlacementRules placement, ItemInstanceFactory factory)
        { this.catalog = catalog; this.pipeline = pipeline; this.placement = placement; this.factory = factory; }
        public MutationResult Add(AddRequest request) => pipeline.Execute(draft =>
        {
            if (!catalog.TryGet(request.Definition, out var definition)) return MutationResult.Fail("Unknown item definition.");
            if (request.Quantity < 1 || request.Quantity > definition.MaxStack) return MutationResult.Fail("Quantity exceeds stack limits.");
            var item = factory.Prepare(definition, request.Quantity);
            var error = placement.Validate(draft, item, request.Target);
            if (error != null) return MutationResult.Fail(error);
            draft.Items.Add(item.Id, item);
            if (!item.ChildContainerId.IsEmpty) draft.Containers.Add(item.ChildContainerId, new ContainerDraft(definition.Container));
            var t = request.Target;
            draft.Containers[t.Container].Entries.Add(item.Id, new InventoryEntry(item.Id, t.Section, t.X, t.Y, t.Rotated));
            draft.Touch(t.Container, item.Id);
            return MutationResult.Ok(request.Quantity, item.Id);
        });
        public MutationResult Delete(ItemInstanceId id) => pipeline.Execute(draft =>
        {
            if (!draft.Items.TryGetValue(id, out var item) || !draft.FindOwner(id, out var owner, out _))
                return MutationResult.Fail("Item no longer exists.");
            if (!item.ChildContainerId.IsEmpty)
            {
                if (draft.Containers[item.ChildContainerId].Entries.Count > 0) return MutationResult.Fail("Empty the bag before deleting it.");
                draft.Containers.Remove(item.ChildContainerId); draft.AffectedContainers.Add(item.ChildContainerId);
            }
            draft.Containers[owner].Entries.Remove(id); draft.Items.Remove(id); draft.Touch(owner, id);
            return MutationResult.Ok(item.Quantity);
        });
    }
}
