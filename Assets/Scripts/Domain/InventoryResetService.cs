namespace InventorySystem.Domain
{
    public sealed class InventoryResetService
    {
        private readonly InventoryMutationPipeline pipeline;
        internal InventoryResetService(InventoryMutationPipeline pipeline) { this.pipeline = pipeline; }
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
