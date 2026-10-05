namespace InventorySystem.Domain
{
    internal sealed class InventoryPreviewCache
    {
        private InventorySnapshot snapshot;
        private InventoryDraft draft;
        internal InventoryDraft Get(InventorySnapshot current)
        {
            if (!ReferenceEquals(snapshot, current)) { snapshot = current; draft = new InventoryDraft(current); }
            return draft;
        }
    }
}
