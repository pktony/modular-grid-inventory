using System;
using System.Linq;
using Pktony.GridInventory.Domain;
namespace Pktony.GridInventory
{
    public sealed class RigDemoSeed
    {
        public void Add(InventoryRuntime runtime, InventoryCatalog catalog)
        {
            var rigs = catalog.Definitions.Where(d => d.CategoryId == "Container/Rig" && d.Identifier != "rig").ToArray();
            var root = runtime.ReadModel.Snapshot.RootContainerId;
            for (int i = 0; i < rigs.Length; i++)
            {
                var target = new PlacementTarget(root, new GridSectionId("main"), i % 2 * 5, 20 + i / 2 * 5);
                var result = runtime.Edit.Add(new AddRequest(rigs[i].Id, 1, target));
                if (!result.Success) throw new InvalidOperationException($"Seed {rigs[i].Identifier}: {result.Reason}");
            }
        }
    }
}
