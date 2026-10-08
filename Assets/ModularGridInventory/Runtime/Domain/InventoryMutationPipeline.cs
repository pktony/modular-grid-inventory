using System;
namespace Pktony.GridInventory.Domain
{
    internal sealed class InventoryMutationPipeline
    {
        private readonly InventorySession session;
        private readonly InventoryInvariantValidator validator;
        private readonly InventoryMutationCommitter committer;
        internal InventoryMutationPipeline(InventorySession session, InventoryInvariantValidator validator, InventoryMutationCommitter committer)
        { this.session = session; this.validator = validator; this.committer = committer; }
        internal MutationResult Execute(System.Func<InventoryDraft, MutationResult> prepare)
        {
            if (session.Disposed) return MutationResult.Fail("Inventory session is closed.");
            if (session.Mutating) return MutationResult.Fail("Inventory is notifying another change.");
            session.Mutating = true;
            try
            {
                var draft = new InventoryDraft(session.Snapshot);
                var result = prepare(draft);
                if (!result.Success) return result;
                var error = validator.Validate(draft);
                if (error != null) return MutationResult.Fail(error);
                var prepared = new InventorySnapshot(session.Snapshot.Version + 1, draft);
                var batch = new InventoryChangeBatch(prepared.Version, draft);
                committer.Commit(prepared, batch);
                return result;
            }
            catch (Exception error) { return MutationResult.Fail("Change preparation failed: " + error.Message); }
            finally { session.Mutating = false; }
        }
    }
}
