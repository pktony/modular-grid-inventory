namespace InventorySystem.Domain
{
    public interface IInventoryStackService
    {
        MutationResult Merge(MergeRequest request);
        MutationResult Split(SplitRequest request);
        MutationResult PreviewMerge(MergeRequest request);
        MutationResult PreviewSplit(SplitRequest request);
    }
}
