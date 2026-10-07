using System.Collections.Generic;
namespace Pktony.GridInventory.Domain
{
    public sealed class ContainerHierarchyRules
    {
        internal bool WouldCycle(InventoryDraft draft, ItemInstance item, ContainerId destination)
        {
            if (item.ChildContainerId.IsEmpty) return false;
            var visited = new HashSet<ContainerId>();
            var queue = new Stack<ContainerId>(); queue.Push(item.ChildContainerId);
            while (queue.Count > 0)
            {
                var id = queue.Pop();
                if (id == destination || !visited.Add(id)) return true;
                if (!draft.Containers.TryGetValue(id, out var container)) continue;
                foreach (var entry in container.Entries.Values)
                {
                    var child = draft.Items[entry.ItemId].ChildContainerId;
                    if (!child.IsEmpty) queue.Push(child);
                }
            }
            return false;
        }
    }
}
