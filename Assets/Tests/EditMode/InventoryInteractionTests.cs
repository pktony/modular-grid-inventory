using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InventorySystem.Tests
{
    public sealed class InventoryInteractionTests
    {
        private InventoryCellData model;
        private TestInventoryView view;
        private InventoryController controller;
        private ItemData item;
        [SetUp] public void SetUp()
        {
            model = new InventoryCellData(8, 8); item = new ItemData("rifle", 4, 2);
            model.TryAdd(item, 0, 0); view = new TestInventoryView(); controller = new InventoryController(model, view);
        }
        [TearDown] public void TearDown() => controller.Dispose();
        private PointerEventData Pointer(int x, int y) => new(null) { position = new Vector2(x, y), pressPosition = new Vector2(x, y) };
        [Test] public void DragFromLastCellKeepsGrabOffset()
        {
            view.PointerEvents.Begin(item, Pointer(3, 1));
            view.PointerEvents.End(Pointer(5, 3));
            Assert.That(model.GetEntry(item).X, Is.EqualTo(2));
            Assert.That(model.GetEntry(item).Y, Is.EqualTo(2));
            Assert.That(view.HasPreview, Is.False);
        }
        [Test] public void CancelDoesNotCommitPreview()
        {
            view.PointerEvents.Begin(item, Pointer(0, 0));
            view.PointerEvents.Drag(Pointer(4, 4)); controller.Rotate(); controller.Cancel();
            view.PointerEvents.End(Pointer(4, 4));
            Assert.That(model.GetEntry(item).X, Is.Zero);
            Assert.That(item.itemDirection, Is.EqualTo(ItemDirection.Horizontal));
        }
        [Test] public void DropOutsideViewportRestoresOriginal()
        {
            view.PointerEvents.Begin(item, Pointer(0, 0)); view.IsInside = false;
            view.PointerEvents.End(Pointer(3, 3));
            Assert.That(model.GetEntry(item).X, Is.Zero);
        }
        [Test] public void RotationRoundTripKeepsGrabbedCell()
        {
            var drag = new InventoryDragSession(model.GetEntry(item), 3, 1);
            drag.SetPointerCell(5, 5); int x = drag.X, y = drag.Y;
            drag.Rotate(); drag.Rotate();
            Assert.That(drag.X, Is.EqualTo(x)); Assert.That(drag.Y, Is.EqualTo(y));
            Assert.That(drag.GripX, Is.EqualTo(3)); Assert.That(drag.GripY, Is.EqualTo(1));
        }
        [Test] public void RotationPreservesOriginalUntilValidDrop()
        {
            view.PointerEvents.Begin(item, Pointer(0, 0));
            controller.UpdatePointer(new Vector2(4, 2)); controller.Rotate();
            Assert.That(item.itemDirection, Is.EqualTo(ItemDirection.Horizontal));
            view.PointerEvents.End(Pointer(4, 2));
            Assert.That(item.itemDirection, Is.EqualTo(ItemDirection.Vertical));
            Assert.That(model.GetEntry(item).X, Is.EqualTo(3));
        }
        [Test] public void DeleteWithoutSelectionIsSafe()
        { controller.RemoveSelected(); Assert.That(model.Count, Is.EqualTo(1)); }
        [Test] public void DeleteSelectionReleasesItsWholeFootprint()
        {
            view.PointerEvents.Select(item, Pointer(0, 0)); controller.RemoveSelected();
            Assert.That(model.Count, Is.Zero); Assert.That(model.GetItemAt(3, 1), Is.Null);
        }
        [Test] public void DisposedControllerDoesNotRespondToPointerEvents()
        {
            controller.Dispose(); view.PointerEvents.Begin(item, Pointer(0, 0)); view.PointerEvents.End(Pointer(2, 2));
            Assert.That(model.GetEntry(item).X, Is.Zero);
        }
    }
    internal sealed class TestInventoryView : IInventoryView
    {
        public InventoryPointerEvents PointerEvents { get; } = new();
        public event System.Action RemoveRequested;
        public bool IsInside = true;
        public bool HasPreview;
        public Vector2Int CellAt(Vector2 point) => new((int)point.x, (int)point.y);
        public bool IsOverGrid(Vector2 point) => IsInside;
        public void SetSelection(ItemData item) { }
        public void ShowItem(ItemData item) { }
        public void SetStatus(string message) { }
        public void ShowPreview(ItemData item, int x, int y, ItemDirection direction, bool valid) => HasPreview = true;
        public void ClearPreview() => HasPreview = false;
        public void RequestRemove() => RemoveRequested?.Invoke();
    }
}
