using System;
namespace InventorySystem.Domain
{
    public sealed class InventoryChangePublisher
    {
        private readonly Action<Exception> report;
        internal event Action<InventoryChangeBatch> Changed;
        public InventoryChangePublisher(Action<Exception> report) { this.report = report; }
        internal void Publish(InventoryChangeBatch batch)
        {
            var handlers = Changed?.GetInvocationList();
            if (handlers == null) return;
            foreach (Action<InventoryChangeBatch> handler in handlers)
            {
                try { handler(batch); }
                catch (Exception error) { try { report?.Invoke(error); } catch { } }
            }
        }
        internal void Clear() => Changed = null;
    }
}
