using System.Collections.Generic;
namespace Pktony.GridInventory.Domain
{
    internal sealed class ContainerDraft
    {
        internal readonly ContainerDefinitionView Definition;
        internal readonly Dictionary<ItemInstanceId, InventoryEntry> Entries = new();
        internal ContainerDraft(ContainerDefinitionView definition) { Definition = definition; }
    }
    internal sealed class InventoryDraft
    {
        internal ContainerId Root;
        internal readonly Dictionary<ItemInstanceId, ItemInstance> Items = new();
        internal readonly Dictionary<ContainerId, ContainerDraft> Containers = new();
        internal readonly HashSet<ContainerId> AffectedContainers = new();
        internal readonly HashSet<ItemInstanceId> AffectedItems = new();
        internal bool Reset;
        internal InventoryDraft(InventorySnapshot snapshot)
        {
            Root = snapshot.RootContainerId;
            foreach (var pair in snapshot.Items) Items.Add(pair.Key, pair.Value);
            foreach (var pair in snapshot.Containers)
            {
                var container = new ContainerDraft(pair.Value.Definition);
                foreach (var entry in pair.Value.Entries) container.Entries.Add(entry.Key, entry.Value);
                Containers.Add(pair.Key, container);
            }
        }
        internal InventoryDraft(ContainerId root, ContainerDefinitionView definition)
        { Root = root; Containers.Add(root, new ContainerDraft(definition)); }
        internal bool FindOwner(ItemInstanceId id, out ContainerId owner, out InventoryEntry entry)
        {
            foreach (var pair in Containers) if (pair.Value.Entries.TryGetValue(id, out entry)) { owner = pair.Key; return true; }
            owner = default; entry = null; return false;
        }
        internal void Touch(ContainerId container, ItemInstanceId item)
        { AffectedContainers.Add(container); AffectedItems.Add(item); }
    }
}
