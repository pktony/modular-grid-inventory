using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Pktony.GridInventory.Domain
{
    public sealed class InventorySnapshot
    {
        public long Version { get; }
        public ContainerId RootContainerId { get; }
        public IReadOnlyDictionary<ItemInstanceId, ItemInstance> Items { get; }
        public IReadOnlyDictionary<ContainerId, ContainerState> Containers { get; }
        public ContainerRegistry Registry { get; }
        internal InventorySnapshot(long version, InventoryDraft draft)
        {
            Version = version; RootContainerId = draft.Root;
            Items = new ReadOnlyDictionary<ItemInstanceId, ItemInstance>(new Dictionary<ItemInstanceId, ItemInstance>(draft.Items));
            var containers = new Dictionary<ContainerId, ContainerState>();
            foreach (var pair in draft.Containers) containers.Add(pair.Key,
                new ContainerState(pair.Key, pair.Value.Definition, pair.Value.Entries, Items));
            Containers = new ReadOnlyDictionary<ContainerId, ContainerState>(containers);
            Registry = new ContainerRegistry(Containers, Items);
        }
    }
}
