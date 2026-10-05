namespace InventorySystem.Domain
{
    internal sealed class InventoryMutationCommitter
    {
        private readonly InventorySession session;
        private readonly InventoryChangePublisher publisher;
        internal InventoryMutationCommitter(InventorySession session, InventoryChangePublisher publisher)
        { this.session = session; this.publisher = publisher; }
        internal void Commit(InventorySnapshot prepared, InventoryChangeBatch batch)
        { session.Replace(prepared); publisher.Publish(batch); }
    }
}
