using System.Collections.Generic;
namespace InventorySystem
{
    public interface IItemCatalog
    {
        IReadOnlyList<ItemDefinitionView> Definitions { get; }
        bool TryGet(DefinitionId id, out ItemDefinitionView definition);
    }
}
