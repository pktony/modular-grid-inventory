using System;
using System.Collections.Generic;
using System.Linq;
using Pktony.GridInventory.Domain;
namespace Pktony.GridInventory
{
    public sealed class InventorySeedBuilder
    {
        public InventorySnapshot Create(InventoryCatalog catalog, ContainerDefinitionView root, InventoryInitialState state)
        {
            using var runtime = new InventoryRuntime(catalog, root);
            var pending = state == null ? new List<InventorySeedEntry>() : state.Entries.ToList();
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in pending)
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key) || !keys.Add(entry.Key) || entry.Item == null)
                    throw new InvalidOperationException("Initial state requires unique keys and valid item references.");
            var instances = new Dictionary<string, ItemInstanceId>(StringComparer.Ordinal);
            while (pending.Count > 0)
            {
                var ready = pending.FirstOrDefault(e => string.IsNullOrEmpty(e.ParentKey) || instances.ContainsKey(e.ParentKey));
                if (ready == null) throw new InvalidOperationException("Initial state has a missing parent or a container cycle.");
                var container = string.IsNullOrEmpty(ready.ParentKey) ? runtime.ReadModel.Snapshot.RootContainerId
                    : runtime.ReadModel.Snapshot.Items[instances[ready.ParentKey]].ChildContainerId;
                if (container.IsEmpty) throw new InvalidOperationException($"Seed parent for '{ready.Key}' is not a container.");
                var result = runtime.Edit.Add(new AddRequest(ready.Item.Id, ready.Quantity,
                    new PlacementTarget(container, new GridSectionId(ready.Section), ready.X, ready.Y, ready.Rotated)));
                if (!result.Success) throw new InvalidOperationException($"Initial state '{ready.Key}': {result.Reason}");
                instances.Add(ready.Key, result.CreatedItemId); pending.Remove(ready);
            }
            return runtime.ReadModel.Snapshot;
        }
    }
}
