using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Pktony.GridInventory.Domain
{
    public sealed class ContainerRegistry
    {
        private readonly IReadOnlyDictionary<ItemInstanceId, ContainerId> owners;
        private readonly IReadOnlyDictionary<ContainerId, ItemInstanceId> bags;
        internal ContainerRegistry(IReadOnlyDictionary<ContainerId, ContainerState> containers,
            IReadOnlyDictionary<ItemInstanceId, ItemInstance> items)
        {
            var ownerMap = new Dictionary<ItemInstanceId, ContainerId>();
            foreach (var container in containers.Values) foreach (var entry in container.Entries.Values)
                ownerMap.Add(entry.ItemId, container.Id);
            var bagMap = new Dictionary<ContainerId, ItemInstanceId>();
            foreach (var item in items.Values) if (!item.ChildContainerId.IsEmpty) bagMap.Add(item.ChildContainerId, item.Id);
            owners = new ReadOnlyDictionary<ItemInstanceId, ContainerId>(ownerMap);
            bags = new ReadOnlyDictionary<ContainerId, ItemInstanceId>(bagMap);
        }
        public bool TryGetOwner(ItemInstanceId item, out ContainerId owner) => owners.TryGetValue(item, out owner);
        public bool TryGetBag(ContainerId container, out ItemInstanceId bag) => bags.TryGetValue(container, out bag);
    }
}
