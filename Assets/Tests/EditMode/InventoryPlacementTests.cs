using NUnit.Framework;
namespace InventorySystem.Tests
{
    public class InventoryPlacementTests
    {
        [TestCase(-1, 0)] [TestCase(0, -1)] [TestCase(3, 0)] [TestCase(0, 3)]
        public void RejectsOutsidePlacement(int x, int y)
        {
            var inventory = new InventoryCellData(4, 4);
            Assert.That(inventory.TryAdd(new ItemData("rifle", 2, 2), x, y), Is.False);
            Assert.That(inventory.Count, Is.Zero);
        }
        [Test] public void FullInventoryRejectsAdditionalItem()
        {
            var inventory = new InventoryCellData(2, 2);
            Assert.That(inventory.TryAdd(new ItemData("full", 2, 2)), Is.True);
            Assert.That(inventory.TryAdd(new ItemData("extra", 1, 1)), Is.False);
            Assert.That(inventory.AddItem(-1, new ItemData("extra", 1, 1)), Is.False);
        }
        [Test] public void FailedMovePreservesOriginalPlacement()
        {
            var inventory = new InventoryCellData(4, 4);
            var item = new ItemData("rifle", 2, 2);
            inventory.TryAdd(item, 0, 0);
            inventory.TryAdd(new ItemData("blocker", 1, 1), 2, 2);
            Assert.That(inventory.TryMove(item, 1, 1, ItemDirection.Horizontal), Is.False);
            Assert.That(inventory.GetEntry(item).X, Is.Zero);
            Assert.That(inventory.GetItemAt(0, 0), Is.SameAs(item));
            Assert.That(inventory.GetItemAt(1, 1), Is.SameAs(item));
        }
        [Test] public void MoveCanReuseItsOwnOccupiedCells()
        {
            var inventory = new InventoryCellData(4, 4);
            var item = new ItemData("rifle", 2, 2);
            inventory.TryAdd(item, 0, 0);
            Assert.That(inventory.TryMove(item, 1, 0, ItemDirection.Horizontal), Is.True);
            Assert.That(inventory.GetItemAt(0, 0), Is.Null);
            Assert.That(inventory.GetItemAt(2, 1), Is.SameAs(item));
        }
        [Test] public void RotationRejectsCollisionWithoutChangingDirection()
        {
            var inventory = new InventoryCellData(4, 4);
            var item = new ItemData("rifle", 3, 1);
            inventory.TryAdd(item, 0, 0);
            inventory.TryAdd(new ItemData("blocker", 1, 1), 0, 2);
            Assert.That(inventory.TryRotate(item), Is.False);
            Assert.That(item.itemDirection, Is.EqualTo(ItemDirection.Horizontal));
            inventory.Remove(inventory.GetItemAt(0, 2));
            Assert.That(inventory.TryRotate(item), Is.True);
            Assert.That(inventory.GetItemAt(0, 2), Is.SameAs(item));
        }
        [Test] public void RemoveClearsEveryOccupiedCell()
        {
            var inventory = new InventoryCellData(2, 2);
            var item = new ItemData("rifle", 2, 2);
            inventory.TryAdd(item); inventory.Remove(item);
            Assert.That(inventory.TryAdd(new ItemData("replacement", 2, 2)), Is.True);
        }
        [Test] public void SameDefinitionCreatesIndependentInstances()
        {
            var inventory = new InventoryCellData(2, 2);
            var a = new ItemData("pistol", 1, 1); var b = new ItemData("pistol", 1, 1);
            Assert.That(inventory.TryAdd(a), Is.True);
            Assert.That(inventory.TryAdd(b), Is.True);
            Assert.That(inventory.TryAdd(a), Is.False);
            Assert.That(a.InstanceId, Is.Not.EqualTo(b.InstanceId));
        }
    }
}
