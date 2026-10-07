using System;
using System.Collections.Generic;
using System.Linq;
namespace Pktony.GridInventory
{
    public sealed class InventoryCatalogSnapshotFactory
    {
        public InventoryCatalog Create(InventoryCatalogAsset source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var containers = new Dictionary<ContainerDefinition, ContainerDefinitionView>();
            var views = new List<ItemDefinitionView>();
            foreach (var item in source.Items)
            {
                if (item == null) { views.Add(null); continue; }
                ContainerDefinitionView container = null;
                if (item.Container != null && !containers.TryGetValue(item.Container, out container))
                {
                    container = item.Container.Freeze(); containers.Add(item.Container, container);
                }
                views.Add(item.Freeze(container));
            }
            return new InventoryCatalog(source.Categories.Select(c => c != null ? c.Freeze() : null), views);
        }
    }
}
