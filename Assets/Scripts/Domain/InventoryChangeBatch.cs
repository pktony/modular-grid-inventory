using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem.Domain
{
    public sealed class InventoryChangeBatch
    {
        public long Version { get; }
        public bool Reset { get; }
        public IReadOnlyList<ContainerId> Containers { get; }
        public IReadOnlyList<ItemInstanceId> Items { get; }
        internal InventoryChangeBatch(long version, InventoryDraft draft)
        {
            Version = version; Reset = draft.Reset;
            Containers = Array.AsReadOnly(draft.AffectedContainers.ToArray());
            Items = Array.AsReadOnly(draft.AffectedItems.ToArray());
        }
    }
}
