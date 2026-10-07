namespace Pktony.GridInventory.Domain
{
    public sealed class MutationResult
    {
        public bool Success { get; }
        public string Reason { get; }
        public int MovedQuantity { get; }
        public ItemInstanceId CreatedItemId { get; }
        private MutationResult(bool success, string reason, int moved, ItemInstanceId created)
        { Success = success; Reason = reason; MovedQuantity = moved; CreatedItemId = created; }
        public static MutationResult Fail(string reason) => new(false, reason, 0, default);
        public static MutationResult Ok(int moved = 0, ItemInstanceId created = default) => new(true, "Ready", moved, created);
    }
}
