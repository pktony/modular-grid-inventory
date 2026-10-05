using System.Collections.Generic;
using InventorySystem.Domain;
namespace InventorySystem.Presentation
{
    public sealed class ContainerNavigation
    {
        public ContainerId Current { get; private set; }
        public bool Open(InventorySnapshot snapshot, ItemInstanceId item)
        {
            if (!snapshot.Items.TryGetValue(item, out var bag) || bag.ChildContainerId.IsEmpty) return false;
            Current = bag.ChildContainerId; return true;
        }
        public void Close() => Current = default;
        public void Back(InventorySnapshot snapshot)
        {
            if (!snapshot.Registry.TryGetBag(Current, out var bag) || !snapshot.Registry.TryGetOwner(bag, out var owner)
                || owner == snapshot.RootContainerId) Close();
            else Current = owner;
        }
        public IReadOnlyList<ItemInstanceId> Path(InventorySnapshot snapshot)
        {
            var path = new List<ItemInstanceId>(); var current = Current; var visited = new HashSet<ContainerId>();
            while (!current.IsEmpty && visited.Add(current) && snapshot.Registry.TryGetBag(current, out var bag))
            {
                path.Add(bag);
                if (!snapshot.Registry.TryGetOwner(bag, out current) || current == snapshot.RootContainerId) break;
            }
            path.Reverse(); return path.AsReadOnly();
        }
        public void Reconcile(InventorySnapshot snapshot) { if (!snapshot.Containers.ContainsKey(Current)) Close(); }
    }
}
