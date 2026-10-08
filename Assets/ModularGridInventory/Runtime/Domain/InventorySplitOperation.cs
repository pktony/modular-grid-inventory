namespace Pktony.GridInventory.Domain
{
    internal sealed class InventorySplitOperation
    {
        private readonly ItemInstanceFactory factory;
        internal InventorySplitOperation(ItemInstanceFactory factory) { this.factory = factory; }
        internal MutationResult Apply(InventoryDraft draft, SplitRequest request)
        {
            var source = draft.Items[request.Source]; var created = factory.Prepare(source.Definition, request.Quantity);
            var target = request.Target;
            draft.Items[source.Id] = source.WithQuantity(source.Quantity - request.Quantity);
            draft.Items.Add(created.Id, created);
            draft.Containers[target.Container].Entries.Add(created.Id,
                new InventoryEntry(created.Id, target.Section, target.X, target.Y, target.Rotated));
            draft.FindOwner(source.Id, out var owner, out _);
            draft.Touch(owner, source.Id); draft.Touch(target.Container, created.Id);
            return MutationResult.Ok(request.Quantity, created.Id);
        }
    }
}
