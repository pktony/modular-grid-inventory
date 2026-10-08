using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryDropResolver
    {
        private readonly IInventoryReadModel model;
        private readonly IInventoryTransferService transfer;
        private readonly IInventoryStackService stack;
        private readonly IInventoryStorageService storage;
        private readonly IInventoryGridHitTester hitTest;
        public InventoryDropResolver(IInventoryReadModel model, IInventoryTransferService transfer, IInventoryStackService stack,
            IInventoryStorageService storage, IInventoryGridHitTester hitTest)
        { this.model = model; this.transfer = transfer; this.stack = stack; this.storage = storage; this.hitTest = hitTest; }
        public InventoryDropPreview Resolve(InventoryDragState drag, Vector2 point)
        {
            if (!hitTest.TryHit(point, out var hit)) return new InventoryDropPreview(MutationResult.Fail("Drop inside a grid compartment."));
            var target = drag.Placement(hit);
            var under = model.Snapshot.Containers[hit.Container].Sections[hit.Section].GetAt(hit.X, hit.Y);
            if (!under.IsEmpty && model.Snapshot.Items.TryGetValue(under, out var item) && !item.ChildContainerId.IsEmpty)
            {
                var result = storage.Preview(new ContainerStoreRequest(drag.Item, item.ChildContainerId, drag.Rotated, drag.SplitQuantity), out target);
                return new InventoryDropPreview(result, target, containerItem: under, storeContainer: item.ChildContainerId);
            }
            if (drag.SplitQuantity > 0) return new InventoryDropPreview(stack.PreviewSplit(new SplitRequest(drag.Item, drag.SplitQuantity, target)), target);
            if (!under.IsEmpty && under != drag.Item) return new InventoryDropPreview(stack.PreviewMerge(new MergeRequest(drag.Item, under)), target, under);
            return new InventoryDropPreview(transfer.Preview(new TransferRequest(drag.Item, target)), target);
        }
        public MutationResult Commit(InventoryDragState drag, InventoryDropPreview preview)
        {
            if (!preview.StoreContainer.IsEmpty) return storage.Store(new ContainerStoreRequest(drag.Item, preview.StoreContainer, drag.Rotated, drag.SplitQuantity));
            if (drag.SplitQuantity > 0) return stack.Split(new SplitRequest(drag.Item, drag.SplitQuantity, preview.Target));
            return !preview.Merge.IsEmpty ? stack.Merge(new MergeRequest(drag.Item, preview.Merge)) : transfer.Transfer(new TransferRequest(drag.Item, preview.Target));
        }
    }
}
