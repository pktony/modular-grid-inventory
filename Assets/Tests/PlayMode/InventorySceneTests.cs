using System.Collections;
using System.Linq;
using Pktony.GridInventory.Domain;
using Pktony.GridInventory.Presentation;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace Pktony.GridInventory.Tests
{
    public sealed class InventorySceneTests
    {
        private InventoryBootstrapper inventory;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        private ContainerId Root => State.RootContainerId;
        private readonly GridSectionId main = new("main");
        [UnitySetUp] public IEnumerator LoadScene()
        {
            yield return SceneManager.LoadSceneAsync("Inventory"); yield return null;
            inventory = Object.FindAnyObjectByType<InventoryBootstrapper>(); inventory.enabled = false;
            Canvas.ForceUpdateCanvases();
        }
        private ItemInstanceId Find(string definition, ContainerId owner = default, int quantity = 0)
            => State.Items.Values.First(i => i.Definition.Identifier == definition && (quantity == 0 || i.Quantity == quantity)
                && (owner.IsEmpty || State.Containers[owner].Entries.ContainsKey(i.Id))).Id;
        private GameObject View(ItemInstanceId id) => GameObject.Find("Item-" + id);
        private Vector2 Point(ContainerId container, GridSectionId section, int x, int y)
        {
            var panel = container == Root ? inventory.Screen.Stash : inventory.Windows.Find(container).Panel;
            return panel.Sections[section].Geometry.ScreenPoint(x, y, new Vector2(25, 25));
        }
        private PointerEventData Data(Vector2 point, int clicks = 1) => new(EventSystem.current)
            { position = point, pressPosition = point, button = PointerEventData.InputButton.Left, clickCount = clicks };
        private void Open(ItemInstanceId id)
        { var data = Data(RectTransformUtility.WorldToScreenPoint(null, View(id).transform.position), 2); ExecuteEvents.Execute(View(id), data, ExecuteEvents.pointerClickHandler); Canvas.ForceUpdateCanvases(); }
        private void RevealLower() { inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.78f; Canvas.ForceUpdateCanvases(); }
        private PointerEventData Begin(ItemInstanceId id)
        {
            State.Registry.TryGetOwner(id, out var owner); var entry = State.Containers[owner].Entries[id];
            var data = Data(Point(owner, entry.SectionId, entry.X, entry.Y));
            ExecuteEvents.Execute(View(id), data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(View(id), data, ExecuteEvents.beginDragHandler); return data;
        }
        private void Drop(ItemInstanceId id, PointerEventData data, Vector2 point)
        { data.position = point; ExecuteEvents.Execute(View(id), data, ExecuteEvents.dragHandler); ExecuteEvents.Execute(View(id), data, ExecuteEvents.endDragHandler); }
        [UnityTest] public IEnumerator SceneHasFrozenDefinitionsAndOneScreen()
        {
            Assert.That(State.Items.Count, Is.EqualTo(22)); Assert.That(State.Containers[Root].Entries.Count, Is.EqualTo(20));
            Assert.That(Object.FindObjectsByType<InventoryBootstrapper>().Length, Is.EqualTo(1));
            Assert.That(GameObject.Find("InventoryScreen"), Is.Not.Null);
            Assert.That(inventory.Screen.Stash.Policy.text, Does.Contain("9 x 48"));
            Assert.That(Object.FindObjectsByType<InventoryPointerHandler>().Length, Is.EqualTo(20));
            foreach (var item in State.Items.Values) Assert.That(item.Definition.Icon, Is.Not.Null);
            LogAssert.NoUnexpectedReceived(); yield return null;
        }
        [UnityTest] public IEnumerator DoubleClickOpensIndependentNestedWindowsAndCloseRevealsParent()
        {
            var berkut = Find("berkut", Root); Open(berkut);
            Assert.That(inventory.Windows.Frontmost.Panel.Container, Is.EqualTo(State.Items[berkut].ChildContainerId));
            var mbss = Find("mbss"); Open(mbss);
            Assert.That(inventory.Windows.Frontmost.Panel.Container, Is.EqualTo(State.Items[mbss].ChildContainerId));
            Assert.That(GameObject.Find("Item-" + Find("ai2", State.Items[mbss].ChildContainerId)), Is.Not.Null);
            inventory.Windows.Frontmost.Close.onClick.Invoke(); yield return null;
            Assert.That(inventory.Windows.Frontmost.Panel.Container, Is.EqualTo(State.Items[berkut].ChildContainerId)); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator PointerDragCrossesPanelsAndRejectsWrongCaseType()
        {
            var ammoCase = Find("ammo-case", Root); Open(ammoCase); RevealLower();
            var ammo = Find("pst", Root, 20); var child = State.Items[ammoCase].ChildContainerId;
            var data = Begin(ammo); Drop(ammo, data, Point(child, main, 0, 0)); yield return null;
            Assert.That(State.Registry.TryGetOwner(ammo, out var owner), Is.True); Assert.That(owner, Is.EqualTo(child));
            var rifle = Find("aks74u", Root); var before = State; data = Begin(rifle); Drop(rifle, data, Point(child, main, 2, 1));
            Assert.That(State, Is.SameAs(before)); Assert.That(inventory.Screen.Status.text, Does.Contain("type")); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator RotateThenCancelDoesNotMutateAndValidDropUpdatesFootprint()
        {
            RevealLower(); var rifle = Find("aks74u", Root); var before = State; var data = Begin(rifle);
            inventory.Interaction.Rotate(); inventory.Interaction.Cancel(); Assert.That(State, Is.SameAs(before));
            data = Begin(rifle); inventory.Interaction.Rotate(); Drop(rifle, data, Point(Root, main, 1, 14)); yield return null;
            var entry = State.Containers[Root].Entries[rifle]; Assert.That(entry.Rotated, Is.True);
            Assert.That(State.Containers[Root].Sections[main].GetAt(0, 17), Is.EqualTo(rifle)); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator PartialMergeThroughPointerEventsKeepsRemainder()
        {
            RevealLower(); var source = Find("pst", Root, 20); var destination = Find("pst", Root, 40);
            var data = Begin(source); Drop(source, data, Point(Root, main, 5, 11)); yield return null;
            Assert.That(State.Items[destination].Quantity, Is.EqualTo(50)); Assert.That(State.Items[source].Quantity, Is.EqualTo(10));
            Assert.That(View(destination).GetComponentsInChildren<TextMeshProUGUI>().Any(t => t.text == "50"), Is.True); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator QuantityDialogSplitCreatesInstanceOnGridClickAndEscapeCancels()
        {
            RevealLower(); var source = Find("pst", Root, 40); var before = State;
            inventory.Interaction.Split(source); Assert.That(inventory.Screen.Quantity.IsOpen, Is.True);
            GameObject.Find("Input").GetComponent<TMP_InputField>().text = "5";
            inventory.Screen.Quantity.Confirm(); Assert.That(State, Is.SameAs(before)); Assert.That(inventory.Interaction.Drag.SplitQuantity, Is.EqualTo(5));
            var point = Point(Root, main, 8, 13); var data = Data(point);
            ExecuteEvents.Execute(inventory.Screen.Stash.Viewport.gameObject, data, ExecuteEvents.pointerClickHandler); yield return null;
            Assert.That(State.Items[source].Quantity, Is.EqualTo(35)); Assert.That(State.Items.Values.Count(i => i.Definition.Identifier == "pst"), Is.EqualTo(3));
            before = State; inventory.Interaction.Split(source); inventory.Screen.Quantity.Confirm(); inventory.Interaction.Cancel();
            Assert.That(State, Is.SameAs(before)); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator InvalidQuantityLeavesDialogOpenAndStateUnchanged()
        {
            var source = Find("pst", Root, 40); var before = State; inventory.Interaction.Split(source);
            GameObject.Find("Input").GetComponent<TMP_InputField>().text = "40"; inventory.Screen.Quantity.Confirm();
            Assert.That(inventory.Screen.Quantity.IsOpen, Is.True); Assert.That(State, Is.SameAs(before));
            inventory.Interaction.Cancel(); yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator BlackRockHasElevenSectionsAndGapIsNotDropTarget()
        {
            Open(Find("rig", Root)); RevealLower(); Assert.That(inventory.Windows.Frontmost.Panel.Sections.Count, Is.EqualTo(11));
            var ammo = Find("pst", Root, 20); var before = State; var data = Begin(ammo);
            var tall = inventory.Windows.Frontmost.Panel.Sections[new GridSectionId("tall-a")];
            var gap = tall.Geometry.ScreenPoint(1, 0, new Vector2(1.5f, 25)); Drop(ammo, data, gap);
            Assert.That(State, Is.SameAs(before));
            data = Begin(ammo); Drop(ammo, data, Point(inventory.Windows.Frontmost.Panel.Container, new GridSectionId("small-a"), 0, 0)); yield return null;
            Assert.That(State.Registry.TryGetOwner(ammo, out var owner), Is.True); Assert.That(owner, Is.EqualTo(inventory.Windows.Frontmost.Panel.Container)); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator NavigationCancelsDragAndResetDoesNotDuplicateViews()
        {
            var berkut = Find("berkut", Root); Open(berkut); Begin(Find("mbss"));
            inventory.Windows.Frontmost.Close.onClick.Invoke(); Assert.That(inventory.Interaction.Drag, Is.Null);
            inventory.Screen.Reset.onClick.Invoke(); inventory.Screen.Reset.onClick.Invoke(); yield return null;
            Assert.That(State.Items.Count, Is.EqualTo(22)); Assert.That(Object.FindObjectsByType<InventoryPointerHandler>().Length, Is.EqualTo(20));
            Assert.That(inventory.Windows.Windows.Count, Is.Zero); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator AllTenRigsOpenDistinctWindowsAndReuseTheirOwnWindow()
        {
            var rigs = State.Items.Values.Where(i => i.Definition.CategoryId == "Container/Rig").ToArray();
            Assert.That(rigs.Length, Is.EqualTo(10));
            foreach (var rig in rigs)
            {
                Open(rig.Id); var window = inventory.Windows.Find(rig.ChildContainerId);
                Assert.That(window.Panel.Sections.Count, Is.EqualTo(rig.Definition.Container.Sections.Count));
                foreach (var section in window.Panel.Sections.Values)
                    Assert.That(section.Geometry.Rect.Find("PocketOutline").GetComponentsInChildren<Image>().All(i => !i.raycastTarget), Is.True);
                Assert.That(window.Panel.Sections.Values.Sum(s => s.Geometry.Definition.Width * s.Geometry.Definition.Height),
                    Is.EqualTo(rig.Definition.Container.Sections.Sum(s => s.Width * s.Height)));
            }
            Assert.That(inventory.Windows.Windows.Count, Is.EqualTo(10));
            var original = inventory.Windows.Find(rigs[0].ChildContainerId); Open(rigs[0].Id);
            Assert.That(inventory.Windows.Frontmost, Is.SameAs(original)); Assert.That(inventory.Windows.Windows.Count, Is.EqualTo(10));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ClickIndicatorIsHiddenWhenReleased()
        {
            inventory.Indicator.Show(new Vector2(300, 300), true); Assert.That(GameObject.Find("ClickIndicator"), Is.Not.Null);
            inventory.Indicator.Show(new Vector2(300, 300), false); Assert.That(GameObject.Find("ClickIndicator"), Is.Null);
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ReopeningSameBagPreservesWindowAndRaisesIt()
        {
            var bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray(); Open(bags[0].Id);
            var original = inventory.Windows.Find(bags[0].ChildContainerId); original.Panel.Root.anchoredPosition = new Vector2(40, -120);
            Open(bags[1].Id); Assert.That(inventory.Windows.Windows.Count, Is.EqualTo(2));
            Open(bags[0].Id); Assert.That(inventory.Windows.Windows.Count, Is.EqualTo(2));
            Assert.That(inventory.Windows.Frontmost, Is.SameAs(original)); Assert.That(original.Panel.Root.anchoredPosition, Is.EqualTo(new Vector2(40, -120)));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator HeaderDragRaisesWindowWithoutChangingInventory()
        {
            var bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray(); Open(bags[0].Id); Open(bags[1].Id);
            var window = inventory.Windows.Find(bags[0].ChildContainerId); var before = State; var previous = window.Panel.Root.anchoredPosition;
            var data = Data(RectTransformUtility.WorldToScreenPoint(null, window.Header.TransformPoint(new Vector3(30, -10))));
            ExecuteEvents.Execute(window.Header.gameObject, data, ExecuteEvents.beginDragHandler); data.position += new Vector2(100, -50);
            ExecuteEvents.Execute(window.Header.gameObject, data, ExecuteEvents.dragHandler); ExecuteEvents.Execute(window.Header.gameObject, data, ExecuteEvents.endDragHandler);
            var scale = window.Panel.Root.GetComponentInParent<Canvas>().scaleFactor;
            Assert.That(window.Panel.Root.anchoredPosition, Is.EqualTo(previous + new Vector2(100, -50) / scale)); Assert.That(inventory.Windows.Frontmost, Is.SameAs(window));
            Assert.That(State, Is.SameAs(before)); yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator FrontWindowHeaderAndEmptySpaceBlockGridBehindIt()
        {
            var bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray(); Open(bags[0].Id); Open(bags[1].Id);
            var back = inventory.Windows.Find(bags[0].ChildContainerId); var front = inventory.Windows.Find(bags[1].ChildContainerId);
            back.Panel.Root.anchoredPosition = front.Panel.Root.anchoredPosition = new Vector2(24, -110); Canvas.ForceUpdateCanvases();
            var hit = new InventoryGridHitTester(inventory.Windows);
            var headerPoint = RectTransformUtility.WorldToScreenPoint(null, front.Header.TransformPoint(new Vector3(20, -10)));
            Assert.That(hit.TryHit(headerPoint, out _), Is.False);
            Assert.That(hit.TryHit(Point(front.Id, main, 0, 0), out var target), Is.True); Assert.That(target.Container, Is.EqualTo(front.Id));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator DropOnClosedBagItemStoresInsideIt()
        {
            inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.86f; Canvas.ForceUpdateCanvases();
            var ammo = Find("pst", Root, 20); var box = Find("ammo-case", Root);
            var data = Begin(ammo); Drop(ammo, data, Point(Root, main, 0, 6)); yield return null;
            Assert.That(State.Registry.TryGetOwner(ammo, out var owner), Is.True); Assert.That(owner, Is.EqualTo(State.Items[box].ChildContainerId));
            Assert.That(inventory.Windows.Windows.Count, Is.Zero); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator NestedBagMovementKeepsItsExistingWindow()
        {
            var bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray(); Open(bags[0].Id); var nested = Find("mbss"); Open(nested);
            var existing = inventory.Windows.Find(State.Items[nested].ChildContainerId); var container = existing.Id;
            Assert.That(inventory.Storage.Store(new ContainerStoreRequest(nested, bags[1].ChildContainerId)).Success, Is.True);
            Assert.That(inventory.Windows.Find(container), Is.SameAs(existing)); Assert.That(existing.Panel.Root.gameObject.activeSelf, Is.True);
            yield return null; LogAssert.NoUnexpectedReceived();
        }
    }
}
