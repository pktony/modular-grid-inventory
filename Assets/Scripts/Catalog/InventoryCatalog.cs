using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem
{
    public sealed class InventoryCatalog : IItemCatalog
    {
        private readonly ItemCatalogSnapshot items;
        private readonly Dictionary<string, CategoryDefinitionView> categories;
        public IReadOnlyList<ItemDefinitionView> Definitions => items.Definitions;
        public IReadOnlyList<CategoryDefinitionView> Categories { get; }
        public InventoryCatalog(IEnumerable<CategoryDefinitionView> categories, IEnumerable<ItemDefinitionView> definitions)
        {
            Categories = Array.AsReadOnly(categories.ToArray());
            var allItems = definitions.ToArray();
            var errors = new InventoryCatalogValidator().Validate(Categories, allItems);
            if (errors.Count > 0) throw new CatalogValidationException(errors);
            items = new ItemCatalogSnapshot(allItems);
            this.categories = Categories.ToDictionary(c => c.Id);
        }
        public bool TryGet(DefinitionId id, out ItemDefinitionView definition) => items.TryGet(id, out definition);
        public bool IsCategory(string categoryId, string ancestor)
        {
            while (!string.IsNullOrEmpty(categoryId))
            {
                if (categoryId == ancestor) return true;
                categoryId = categories.TryGetValue(categoryId, out var category) ? category.ParentId : null;
            }
            return false;
        }
    }
}
