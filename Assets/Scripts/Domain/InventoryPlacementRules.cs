using System.Linq;
namespace InventorySystem.Domain
{
    public sealed class InventoryPlacementRules
    {
        private readonly AcceptanceRules acceptance;
        private readonly ContainerHierarchyRules hierarchy;
        public InventoryPlacementRules(AcceptanceRules acceptance, ContainerHierarchyRules hierarchy)
        { this.acceptance = acceptance; this.hierarchy = hierarchy; }
        internal string Validate(InventoryDraft draft, ItemInstance item, PlacementTarget target, ItemInstanceId ignore = default)
        {
            if (!draft.Containers.TryGetValue(target.Container, out var container)) return "Container no longer exists.";
            var section = container.Definition.Sections.FirstOrDefault(s => s.Id == target.Section.Value);
            if (section == null) return "Choose a grid compartment.";
            if (!acceptance.Allows(container.Definition.Policy, item.Definition) || !acceptance.Allows(section.Policy, item.Definition))
                return "This compartment does not accept this item type.";
            if (hierarchy.WouldCycle(draft, item, target.Container)) return "A bag cannot contain itself or its ancestor.";
            int width = target.Rotated ? item.Definition.Height : item.Definition.Width;
            int height = target.Rotated ? item.Definition.Width : item.Definition.Height;
            if (target.X < 0 || target.Y < 0 || target.X + width > section.Width || target.Y + height > section.Height)
                return "Item must fit entirely inside one compartment.";
            foreach (var entry in container.Entries.Values)
            {
                if (entry.ItemId == ignore || entry.SectionId != target.Section) continue;
                var other = draft.Items[entry.ItemId].Definition;
                int ow = entry.Rotated ? other.Height : other.Width, oh = entry.Rotated ? other.Width : other.Height;
                if (target.X < entry.X + ow && target.X + width > entry.X && target.Y < entry.Y + oh && target.Y + height > entry.Y)
                    return "The target cells are occupied.";
            }
            return null;
        }
    }
}
