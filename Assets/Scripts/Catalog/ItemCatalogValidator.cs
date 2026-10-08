using System.Collections.Generic;
namespace InventorySystem
{
    public sealed class ItemCatalogValidator
    {
        private readonly ItemDefinitionValidator definitions = new();
        public IReadOnlyList<string> Validate(IReadOnlyList<ItemDefinitionView> items)
        {
            var errors = new List<string>();
            var ids = new HashSet<DefinitionId>();
            if (items.Count == 0) errors.Add("Catalog must contain at least one item.");
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                string path = $"Item[{i}]";
                if (item == null) { errors.Add($"{path}: definition reference is missing."); continue; }
                definitions.Validate(item, path, errors);
                if (!ids.Add(item.Id)) errors.Add($"{path}: duplicate item ID '{item.Identifier}'.");
            }
            return errors.AsReadOnly();
        }
    }
}
