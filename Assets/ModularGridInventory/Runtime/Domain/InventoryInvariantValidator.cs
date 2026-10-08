using System.Collections.Generic;
namespace Pktony.GridInventory.Domain
{
    internal sealed class InventoryInvariantValidator
    {
        private readonly InventoryPlacementRules placement;
        private readonly IItemCatalog catalog;
        internal InventoryInvariantValidator(InventoryPlacementRules placement, IItemCatalog catalog) { this.placement = placement; this.catalog = catalog; }
        internal string Validate(InventoryDraft draft)
        {
            if (!draft.Containers.ContainsKey(draft.Root)) return "Root container is missing.";
            var owners = new HashSet<ItemInstanceId>(); var childContainers = new HashSet<ContainerId>();
            foreach (var pair in draft.Containers) foreach (var entry in pair.Value.Entries.Values)
            {
                if (!owners.Add(entry.ItemId) || !draft.Items.TryGetValue(entry.ItemId, out var item)) return "Invalid item ownership.";
                var error = placement.Validate(draft, item, new PlacementTarget(pair.Key, entry.SectionId, entry.X, entry.Y, entry.Rotated), item.Id);
                if (error != null) return error;
            }
            if (owners.Count != draft.Items.Count) return "Unplaced item detected.";
            foreach (var item in draft.Items.Values)
            {
                if (!catalog.TryGet(item.Definition.Id, out var definition) || !ReferenceEquals(definition, item.Definition)) return "Item definition belongs to another catalog session.";
                if (item.Quantity < 1 || item.Quantity > item.Definition.MaxStack) return "Invalid quantity.";
                if (item.Definition.Container == null)
                { if (!item.ChildContainerId.IsEmpty) return "Unexpected child container."; }
                else if (item.ChildContainerId.IsEmpty || !childContainers.Add(item.ChildContainerId)
                    || !draft.Containers.TryGetValue(item.ChildContainerId, out var container)
                    || !ReferenceEquals(container.Definition, item.Definition.Container)) return "Invalid bag container link.";
            }
            if (childContainers.Contains(draft.Root) || childContainers.Count + 1 != draft.Containers.Count) return "Orphan container detected.";
            return null;
        }
    }
}
