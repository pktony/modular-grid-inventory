namespace Pktony.GridInventory.Domain
{
    public sealed class InventoryResetService
    {
        private readonly InventoryMutationPipeline pipeline;
        internal InventoryResetService(InventoryMutationPipeline pipeline) { this.pipeline = pipeline; }
        public MutationResult Replace(InventorySnapshot source) => pipeline.Execute(draft =>
        {
            foreach (var id in draft.Items.Keys) draft.AffectedItems.Add(id);
            foreach (var id in draft.Containers.Keys) draft.AffectedContainers.Add(id);
            draft.Items.Clear(); draft.Containers.Clear(); draft.Root = source.RootContainerId;
            foreach (var pair in source.Items) { draft.Items.Add(pair.Key, pair.Value); draft.AffectedItems.Add(pair.Key); }
            foreach (var pair in source.Containers)
            {
                var container = new ContainerDraft(pair.Value.Definition);
                foreach (var entry in pair.Value.Entries) container.Entries.Add(entry.Key, entry.Value);
                draft.Containers.Add(pair.Key, container); draft.AffectedContainers.Add(pair.Key);
            }
            draft.Reset = true; return MutationResult.Ok();
        });
        public MutationResult Clear() => pipeline.Execute(draft =>
        {
            foreach (var id in draft.Items.Keys) draft.AffectedItems.Add(id);
            foreach (var id in draft.Containers.Keys) draft.AffectedContainers.Add(id);
            var root = draft.Containers[draft.Root];
            draft.Items.Clear(); draft.Containers.Clear(); root.Entries.Clear(); draft.Containers.Add(draft.Root, root);
            draft.Reset = true; return MutationResult.Ok();
        });
    }
}
