using System;
namespace InventorySystem.Domain
{
    public sealed class InventorySession : IInventoryReadModel, IDisposable
    {
        private readonly InventoryChangePublisher publisher;
        internal bool Mutating { get; set; }
        internal bool Disposed { get; private set; }
        public InventorySnapshot Snapshot { get; private set; }
        public event Action<InventoryChangeBatch> Changed { add => publisher.Changed += value; remove => publisher.Changed -= value; }
        internal InventorySession(InventorySnapshot initial, InventoryChangePublisher publisher)
        { Snapshot = initial; this.publisher = publisher; }
        internal void Replace(InventorySnapshot next) => Snapshot = next;
        public void Dispose() { Disposed = true; publisher.Clear(); }
    }
}
