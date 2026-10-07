using System;
using System.Collections.Generic;
using System.Linq;
namespace Pktony.GridInventory
{
    public sealed class ItemCatalogSnapshot : IItemCatalog
    {
        private readonly Dictionary<DefinitionId, ItemDefinitionView> byId;
        public IReadOnlyList<ItemDefinitionView> Definitions { get; }
        public ItemCatalogSnapshot(IEnumerable<ItemDefinitionView> definitions)
        {
            var copy = definitions?.ToArray() ?? Array.Empty<ItemDefinitionView>();
            var errors = new ItemCatalogValidator().Validate(copy);
            if (errors.Count > 0) throw new CatalogValidationException(errors);
            Definitions = Array.AsReadOnly(copy);
            byId = copy.ToDictionary(item => item.Id);
        }
        public bool TryGet(DefinitionId id, out ItemDefinitionView definition) => byId.TryGetValue(id, out definition);
    }
}
