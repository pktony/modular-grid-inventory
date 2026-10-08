using System.Collections.Generic;
using System.Linq;
namespace InventorySystem
{
    public sealed class InventoryCatalogValidator
    {
        public IReadOnlyList<string> Validate(IReadOnlyList<CategoryDefinitionView> categories, IReadOnlyList<ItemDefinitionView> items)
        {
            var errors = new List<string>(new ItemCatalogValidator().Validate(items));
            var categoryMap = new CategoryTreeValidator().Validate(categories, errors);
            var categoryIds = new HashSet<string>(categoryMap.Keys);
            var itemIds = new HashSet<DefinitionId>(items.Where(i => i != null).Select(i => i.Id));
            var containers = new Dictionary<string, ContainerDefinitionView>();
            var validator = new ContainerDefinitionValidator();
            foreach (var item in items.Where(i => i != null))
            {
                if (string.IsNullOrWhiteSpace(item.CategoryId) || !categoryMap.ContainsKey(item.CategoryId)) errors.Add($"{item.Identifier}: missing category.");
                if (item.Container == null) continue;
                if (item.MaxStack != 1) errors.Add($"{item.Identifier}: containers cannot stack.");
                var container = item.Container;
                if (!string.IsNullOrWhiteSpace(container.Id))
                {
                    if (containers.TryGetValue(container.Id, out var previous))
                    { if (!ReferenceEquals(previous, container)) errors.Add($"Duplicate container '{container.Id}'."); else continue; }
                    else containers.Add(container.Id, container);
                }
                validator.Validate(container, categoryIds, itemIds, errors);
            }
            return errors.AsReadOnly();
        }
    }
}
