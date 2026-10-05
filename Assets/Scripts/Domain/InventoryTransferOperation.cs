namespace InventorySystem.Domain
{
    internal static class InventoryTransferOperation
    {
        internal static MutationResult Apply(InventoryDraft draft, TransferRequest request)
        {
            draft.FindOwner(request.Item, out var owner, out _);
            var item = draft.Items[request.Item]; var target = request.Target;
            draft.Containers[owner].Entries.Remove(request.Item);
            draft.Containers[target.Container].Entries.Add(request.Item,
                new InventoryEntry(request.Item, target.Section, target.X, target.Y, target.Rotated));
            draft.Touch(owner, request.Item); draft.Touch(target.Container, request.Item);
            return MutationResult.Ok(item.Quantity);
        }
    }
}
