using System.Collections.Generic;
namespace Pktony.GridInventory
{
    public interface IItemCatalog
    {
        IReadOnlyList<ItemDefinitionView> Definitions { get; }
        bool TryGet(DefinitionId id, out ItemDefinitionView definition);
    }
}
