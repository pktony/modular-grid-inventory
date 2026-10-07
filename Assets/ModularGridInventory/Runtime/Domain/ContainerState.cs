using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace Pktony.GridInventory.Domain
{
    public sealed class ContainerState
    {
        public ContainerId Id { get; }
        public ContainerDefinitionView Definition { get; }
        public IReadOnlyDictionary<ItemInstanceId, InventoryEntry> Entries { get; }
        public IReadOnlyDictionary<GridSectionId, GridSectionState> Sections { get; }
        internal ContainerState(ContainerId id, ContainerDefinitionView definition,
            IDictionary<ItemInstanceId, InventoryEntry> entries, IReadOnlyDictionary<ItemInstanceId, ItemInstance> items)
        {
            Id = id; Definition = definition;
            Entries = new ReadOnlyDictionary<ItemInstanceId, InventoryEntry>(new Dictionary<ItemInstanceId, InventoryEntry>(entries));
            Sections = new ReadOnlyDictionary<GridSectionId, GridSectionState>(definition.Sections.ToDictionary(
                s => new GridSectionId(s.Id), s => new GridSectionState(s, Entries.Values, items)));
        }
    }
}
