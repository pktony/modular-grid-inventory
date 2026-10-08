namespace InventorySystem.Domain
{
    internal sealed class InventoryContainerPlacementFinder
    {
        private readonly InventoryPlacementRules rules;
        internal InventoryContainerPlacementFinder(InventoryPlacementRules rules) { this.rules = rules; }
        internal MutationResult Find(InventoryDraft draft, ItemInstance item, ContainerStoreRequest request, out PlacementTarget target)
        {
            target = default;
            if (!draft.Containers.TryGetValue(request.Container, out var container)) return MutationResult.Fail("Container no longer exists.");
            string rejection = null;
            for (int orientation = 0; orientation < 2; orientation++)
            {
                bool rotated = orientation == 0 ? request.Rotated : !request.Rotated;
                foreach (var section in container.Definition.Sections)
                {
                    for (int y = 0; y < section.Height; y++) for (int x = 0; x < section.Width; x++)
                    {
                        var candidate = new PlacementTarget(request.Container, new GridSectionId(section.Id), x, y, rotated);
                        var error = rules.Validate(draft, item, candidate, request.SplitQuantity == 0 ? item.Id : default);
                        if (error == null) { target = candidate; return MutationResult.Ok(request.SplitQuantity > 0 ? request.SplitQuantity : item.Quantity); }
                        rejection ??= error;
                    }
                }
            }
            return MutationResult.Fail("No permitted free compartment. " + rejection);
        }
    }
}
