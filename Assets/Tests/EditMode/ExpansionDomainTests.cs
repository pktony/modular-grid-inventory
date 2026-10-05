using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain;
using NUnit.Framework;
using UnityEngine;
using Entry = InventorySystem.Domain.InventoryEntry;
namespace InventorySystem.Tests
{
    public sealed class ExpansionDomainTests
    {
        private Texture2D texture;
        private Sprite icon;
        private InventoryCatalog catalog;
        private InventoryRuntime runtime;
        private ContainerId root;
        private readonly GridSectionId main = new("main");
        private InventorySnapshot State => runtime.ReadModel.Snapshot;
        [SetUp] public void SetUp()
        {
            texture = new Texture2D(1, 1); icon = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero);
            var bag = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 6, 6) });
            var ammoCase = new ContainerDefinitionView("ammo-case", new[] { new GridSectionDefinitionView("main", 4, 4) },
                new AcceptancePolicyView(AcceptanceMode.AllowListed, new[] { "Ammo" }));
            var rig = new ContainerDefinitionView("rig", new[] { new GridSectionDefinitionView("pocket", 1, 2), new GridSectionDefinitionView("large", 2, 2) });
            catalog = new InventoryCatalog(new[] { new CategoryDefinitionView("Ammo", "Ammo"), new CategoryDefinitionView("9mm", "9mm", "Ammo"),
                new CategoryDefinitionView("Bag", "Bag"), new CategoryDefinitionView("Weapon", "Weapon") }, new[] {
                Def("ammo", "9mm", 1, 1, 50), Def("other-ammo", "9mm", 1, 1, 50), Def("weapon", "Weapon", 3, 1),
                Def("bag", "Bag", 2, 2, 1, bag), Def("case", "Bag", 2, 2, 1, ammoCase), Def("rig", "Bag", 2, 2, 1, rig) });
            runtime = new InventoryRuntime(catalog, new ContainerDefinitionView("stash", new[] { new GridSectionDefinitionView("main", 12, 20) }));
            root = State.RootContainerId;
        }
        private ItemDefinitionView Def(string id, string category, int width, int height, int max = 1, ContainerDefinitionView container = null)
            => new(new DefinitionId(id), id, width, height, max, icon, category, container);
        [TearDown] public void TearDown() { runtime.Dispose(); UnityEngine.Object.DestroyImmediate(icon); UnityEngine.Object.DestroyImmediate(texture); }
        private ItemInstanceId Add(string id, int x, int y, int quantity = 1, ContainerId container = default, GridSectionId section = default)
        {
            var result = runtime.Edit.Add(new AddRequest(new DefinitionId(id), quantity,
                new PlacementTarget(container.IsEmpty ? root : container, section.IsEmpty ? main : section, x, y)));
            Assert.That(result.Success, Is.True, result.Reason); return result.CreatedItemId;
        }
        [TestCase(-1, 0)] [TestCase(0, -1)] [TestCase(12, 0)] [TestCase(0, 20)]
        public void BoundaryAddRejectsRegistration(int x, int y)
        {
            var before = State;
            var result = runtime.Edit.Add(new AddRequest(new DefinitionId("weapon"), 1, new PlacementTarget(root, main, x, y)));
            Assert.That(result.Success, Is.False); Assert.That(State, Is.SameAs(before));
        }
        [Test] public void SelfMergeAndOversizedQuantityLeaveStateUnchanged()
        {
            var ammo = Add("ammo", 0, 0, 20); var before = State;
            Assert.That(runtime.Stack.Merge(new MergeRequest(ammo, ammo)).Success, Is.False);
            Assert.That(runtime.Edit.Add(new AddRequest(new DefinitionId("ammo"), 51, new PlacementTarget(root, main, 1, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void BagRegistrationPublishesItsChildContainer()
        {
            InventoryChangeBatch observed = null; runtime.ReadModel.Changed += batch => observed = batch;
            var bag = Add("bag", 0, 0);
            Assert.That(observed.Containers, Does.Contain(State.Items[bag].ChildContainerId));
            Assert.That(observed.Containers, Does.Contain(root));
        }
        [Test] public void TwoBagsHaveIndependentContents()
        {
            var a = Add("bag", 0, 0); var b = Add("bag", 3, 0);
            var childA = State.Items[a].ChildContainerId; var childB = State.Items[b].ChildContainerId;
            Assert.That(childA, Is.Not.EqualTo(childB));
            Add("ammo", 0, 0, 20, childA);
            Assert.That(State.Containers[childB].Entries.Count, Is.Zero);
        }
        [Test] public void ThreeLevelsKeepIdsAfterOuterBagMoves()
        {
            var outer = Add("bag", 0, 0); var child = State.Items[outer].ChildContainerId;
            var inner = Add("bag", 0, 0, 1, child); var deepest = State.Items[inner].ChildContainerId;
            var ammo = Add("ammo", 0, 0, 21, deepest); var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(outer, new PlacementTarget(root, main, 6, 0))).Success, Is.True);
            Assert.That(State.Items[outer].ChildContainerId, Is.EqualTo(child));
            Assert.That(State.Items[inner].ChildContainerId, Is.EqualTo(deepest));
            Assert.That(State.Items[ammo], Is.SameAs(before.Items[ammo]));
            Assert.That(State.Containers[deepest].Entries[ammo], Is.SameAs(before.Containers[deepest].Entries[ammo]));
        }
        [Test] public void SelfAndAncestorCyclesFailWithoutChanges()
        {
            var outer = Add("bag", 0, 0); var child = State.Items[outer].ChildContainerId;
            var inner = Add("bag", 0, 0, 1, child); var deepest = State.Items[inner].ChildContainerId;
            var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(outer, new PlacementTarget(child, main, 3, 0))).Success, Is.False);
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(outer, new PlacementTarget(deepest, main, 0, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void AmmoCaseAcceptsDescendantsAndRejectsWeapons()
        {
            var caseId = Add("case", 0, 0); var child = State.Items[caseId].ChildContainerId;
            Add("ammo", 0, 0, 12, child); var weapon = Add("weapon", 3, 0); var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(weapon, new PlacementTarget(child, main, 0, 1))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void DenyHasPrecedenceAndEmptyAllowListDeniesAll()
        {
            var rule = new AcceptanceRules(catalog); catalog.TryGet(new DefinitionId("ammo"), out var ammo);
            Assert.That(rule.Allows(new AcceptancePolicyView(AcceptanceMode.AllowListed), ammo), Is.False);
            Assert.That(rule.Allows(new AcceptancePolicyView(AcceptanceMode.AllowListed, new[] { "Ammo" }, new[] { "9mm" }), ammo), Is.False);
            Assert.That(rule.Allows(new AcceptancePolicyView(AcceptanceMode.AllowAll, deniedItems: new[] { ammo.Id }), ammo), Is.False);
            Assert.That(rule.Allows(new AcceptancePolicyView(AcceptanceMode.AllowListed, allowedItems: new[] { ammo.Id }), ammo), Is.True);
        }
        [Test] public void SectionBoundaryFailsEvenWithEnoughTotalCells()
        {
            var rig = Add("rig", 0, 0); var child = State.Items[rig].ChildContainerId; var weapon = Add("weapon", 3, 0); var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(weapon, new PlacementTarget(child, new GridSectionId("large"), 0, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void SectionMovePublishesOneBatchAndKeepsOwner()
        {
            var rig = Add("rig", 0, 0); var child = State.Items[rig].ChildContainerId;
            var ammo = Add("ammo", 0, 0, 20, child, new GridSectionId("pocket")); int notifications = 0;
            runtime.ReadModel.Changed += batch => { notifications++; Assert.That(batch.Containers.Count, Is.EqualTo(1)); };
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(ammo, new PlacementTarget(child, new GridSectionId("large"), 1, 1))).Success, Is.True);
            Assert.That(notifications, Is.EqualTo(1)); Assert.That(State.Registry.TryGetOwner(ammo, out var owner), Is.True);
            Assert.That(owner, Is.EqualTo(child)); Assert.That(State.Containers[child].Sections[new GridSectionId("pocket")].GetAt(0, 0).IsEmpty, Is.True);
        }
        [Test] public void RotationUpdatesOccupancyAndOldSnapshotStaysFrozen()
        {
            var weapon = Add("weapon", 0, 0); var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(weapon, new PlacementTarget(root, main, 4, 1, true))).Success, Is.True);
            Assert.That(State.Containers[root].Sections[main].GetAt(4, 3), Is.EqualTo(weapon));
            Assert.That(State.Containers[root].Sections[main].GetAt(0, 0).IsEmpty, Is.True);
            Assert.That(before.Containers[root].Sections[main].GetAt(2, 0), Is.EqualTo(weapon));
            Assert.That(before.Containers[root].Entries[weapon].Rotated, Is.False);
        }
        [Test] public void FailedPlacementLeavesEveryIndexAndVersionUnchanged()
        {
            var a = Add("weapon", 0, 0); Add("bag", 4, 0); var before = State;
            Assert.That(runtime.Transfer.Transfer(new TransferRequest(a, new PlacementTarget(root, main, 4, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void PartialMergeKeepsRemainderAtSourceAndConservesTotal()
        {
            var destination = Add("ammo", 0, 0, 40); var source = Add("ammo", 1, 0, 20); var before = State;
            var result = runtime.Stack.Merge(new MergeRequest(source, destination));
            Assert.That(result.Success, Is.True); Assert.That(result.MovedQuantity, Is.EqualTo(10));
            Assert.That(State.Items[destination].Quantity, Is.EqualTo(50)); Assert.That(State.Items[source].Quantity, Is.EqualTo(10));
            Assert.That(State.Containers[root].Entries[source], Is.SameAs(before.Containers[root].Entries[source]));
            Assert.That(State.Items.Values.Sum(i => i.Quantity), Is.EqualTo(60));
        }
        [Test] public void FullMergeRemovesSourceRegistrationAndOccupancy()
        {
            var a = Add("ammo", 0, 0, 20); var b = Add("ammo", 1, 0, 20);
            Assert.That(runtime.Stack.Merge(new MergeRequest(b, a)).Success, Is.True);
            Assert.That(State.Items.ContainsKey(b), Is.False); Assert.That(State.Registry.TryGetOwner(b, out _), Is.False);
            Assert.That(State.Containers[root].Sections[main].GetAt(1, 0).IsEmpty, Is.True);
        }
        [Test] public void FullOrDifferentDefinitionMergeFails()
        {
            var a = Add("ammo", 0, 0, 50); var b = Add("ammo", 1, 0, 20); var c = Add("other-ammo", 2, 0, 20); var before = State;
            Assert.That(runtime.Stack.Merge(new MergeRequest(b, a)).Success, Is.False);
            Assert.That(runtime.Stack.Merge(new MergeRequest(b, c)).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [TestCase(0)] [TestCase(20)] [TestCase(21)] [TestCase(-1)] public void InvalidSplitCreatesNoInstance(int quantity)
        {
            var ammo = Add("ammo", 0, 0, 20); var before = State;
            Assert.That(runtime.Stack.Split(new SplitRequest(ammo, quantity, new PlacementTarget(root, main, 1, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
        }
        [Test] public void SplitCreatesNewIdOnlyAfterSuccessfulPlacement()
        {
            var a = Add("ammo", 0, 0, 20); var before = State;
            Assert.That(runtime.Stack.PreviewSplit(new SplitRequest(a, 5, new PlacementTarget(root, main, 1, 0))).Success, Is.True);
            Assert.That(State, Is.SameAs(before));
            Assert.That(runtime.Stack.Split(new SplitRequest(a, 5, new PlacementTarget(root, main, 0, 0))).Success, Is.False);
            Assert.That(State, Is.SameAs(before));
            var result = runtime.Stack.Split(new SplitRequest(a, 5, new PlacementTarget(root, main, 1, 0)));
            Assert.That(result.Success, Is.True); Assert.That(result.CreatedItemId, Is.Not.EqualTo(a));
            Assert.That(State.Items[a].Quantity, Is.EqualTo(15)); Assert.That(State.Items[result.CreatedItemId].Quantity, Is.EqualTo(5));
        }
        [Test] public void CrossContainerMergeAndSplitConserveQuantity()
        {
            var bag = Add("bag", 0, 0); var child = State.Items[bag].ChildContainerId;
            var a = Add("ammo", 3, 0, 30); var b = Add("ammo", 0, 0, 10, child);
            Assert.That(runtime.Stack.Merge(new MergeRequest(a, b)).Success, Is.True);
            Assert.That(runtime.Stack.Split(new SplitRequest(b, 12, new PlacementTarget(root, main, 4, 0))).Success, Is.True);
            Assert.That(State.Items.Values.Where(i => i.Definition.Identifier == "ammo").Sum(i => i.Quantity), Is.EqualTo(40));
        }
        [Test] public void NonemptyBagDeleteFailsAndEmptyDeleteRemovesChildContainer()
        {
            var bag = Add("bag", 0, 0); var child = State.Items[bag].ChildContainerId; var ammo = Add("ammo", 0, 0, 20, child);
            var before = State; Assert.That(runtime.Edit.Delete(bag).Success, Is.False); Assert.That(State, Is.SameAs(before));
            Assert.That(runtime.Edit.Delete(ammo).Success, Is.True); Assert.That(runtime.Edit.Delete(bag).Success, Is.True);
            Assert.That(State.Containers.ContainsKey(child), Is.False);
        }
        [Test] public void SubscriberExceptionIsIsolatedAndReentryFails()
        {
            var reported = new List<Exception>(); runtime.Dispose();
            runtime = new InventoryRuntime(catalog, new ContainerDefinitionView("stash", new[] { new GridSectionDefinitionView("main", 12, 20) }), reported.Add);
            root = State.RootContainerId; int received = 0; MutationResult nested = null;
            runtime.ReadModel.Changed += batch => { throw new Exception("listener"); };
            runtime.ReadModel.Changed += batch => { nested = runtime.Reset.Clear(); received++; Assert.That(State.Version, Is.EqualTo(batch.Version)); };
            Add("ammo", 0, 0, 20);
            Assert.That(received, Is.EqualTo(1)); Assert.That(reported.Count, Is.EqualTo(1)); Assert.That(nested.Success, Is.False);
            Assert.That(State.Items.Count, Is.EqualTo(1)); Assert.That(runtime.Edit.Delete(State.Items.Keys.Single()).Success, Is.True);
        }
        [Test] public void ResetRemovesAllNestedRegistrationsInOneNotification()
        {
            var bag = Add("bag", 0, 0); Add("ammo", 0, 0, 20, State.Items[bag].ChildContainerId);
            int count = 0; runtime.ReadModel.Changed += batch => { count++; Assert.That(batch.Reset, Is.True); };
            Assert.That(runtime.Reset.Clear().Success, Is.True);
            Assert.That(State.Items.Count, Is.Zero); Assert.That(State.Containers.Count, Is.EqualTo(1)); Assert.That(count, Is.EqualTo(1));
        }
        [Test] public void DisposedSessionRejectsMutations()
        { runtime.Dispose(); Assert.That(runtime.Edit.Add(new AddRequest(new DefinitionId("ammo"), 2, new PlacementTarget(root, main, 0, 0))).Success, Is.False); }
        [Test] public void PublicSnapshotsRejectCollectionMutation()
        {
            Add("ammo", 0, 0, 20);
            Assert.Throws<NotSupportedException>(() => ((IDictionary<ItemInstanceId, ItemInstance>)State.Items).Clear());
            Assert.Throws<NotSupportedException>(() => ((IDictionary<ItemInstanceId, Entry>)State.Containers[root].Entries).Clear());
        }
    }
}
