using System;
using InventorySystem.Domain;
namespace InventorySystem
{
    public sealed class ExpandedDemoSeed
    {
        public static ContainerDefinitionView StashDefinition() => new("stash", new[] { new GridSectionDefinitionView("main", 9, 48) });
        public InventorySnapshot Create(InventoryCatalog catalog)
        {
            using var runtime = new InventoryRuntime(catalog, StashDefinition());
            var root = runtime.ReadModel.Snapshot.RootContainerId;
            var berkut = Add(runtime, "berkut", root, 0, 0);
            Add(runtime, "berkut", root, 5, 0);
            var mbss = Add(runtime, "mbss", runtime.ReadModel.Snapshot.Items[berkut].ChildContainerId, 0, 0);
            Add(runtime, "ai2", runtime.ReadModel.Snapshot.Items[mbss].ChildContainerId, 0, 0);
            Add(runtime, "ammo-case", root, 0, 6); Add(runtime, "medicine-case", root, 3, 6); Add(runtime, "rig", root, 6, 6);
            Add(runtime, "aks74u", root, 0, 11); Add(runtime, "pst", root, 5, 11, 40); Add(runtime, "pst", root, 6, 11, 20);
            Add(runtime, "ps", root, 7, 11, 30); Add(runtime, "ai2", root, 5, 12); Add(runtime, "rk0", root, 6, 12);
            new RigDemoSeed().Add(runtime, catalog);
            return runtime.ReadModel.Snapshot;
        }
        private static ItemInstanceId Add(InventoryRuntime runtime, string id, ContainerId container, int x, int y, int quantity = 1)
        {
            var result = runtime.Edit.Add(new AddRequest(new DefinitionId(id), quantity, new PlacementTarget(container, new GridSectionId("main"), x, y)));
            if (!result.Success) throw new InvalidOperationException($"Seed {id}: {result.Reason}");
            return result.CreatedItemId;
        }
    }
}
