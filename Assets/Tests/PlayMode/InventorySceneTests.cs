using System.Collections;
using System.Linq;
using InventorySystem.Domain;
using InventorySystem.Presentation;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace InventorySystem.Tests
{
    public sealed class InventorySceneTests
    {
        private ExpandedInventory inventory;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        private ContainerId Root => State.RootContainerId;
        private readonly GridSectionId main = new("main");
        [UnitySetUp] public IEnumerator LoadScene()
        {
            yield return SceneManager.LoadSceneAsync("Inventory"); yield return null;
            inventory = Object.FindAnyObjectByType<ExpandedInventory>(); inventory.enabled = false;
            Canvas.ForceUpdateCanvases();
        }
        private ItemInstanceId Find(string definition, ContainerId owner = default, int quantity = 0)
            => State.Items.Values.First(i => i.Definition.Identifier == definition && (quantity == 0 || i.Quantity == quantity)
                && (owner.IsEmpty || State.Containers[owner].Entries.ContainsKey(i.Id))).Id;
        private GameObject View(ItemInstanceId id) => GameObject.Find("Item-" + id);
        private Vector2 Point(ContainerId container, GridSectionId section, int x, int y)
        {
            var panel = container == Root ? inventory.Screen.Stash : inventory.Screen.Bag;
            return panel.Sections[section].Geometry.ScreenPoint(x, y, new Vector2(25, 25));
        }
        private PointerEventData Data(Vector2 point, int clicks = 1) => new(EventSystem.current)
            { position = point, pressPosition = point, button = PointerEventData.InputButton.Left, clickCount = clicks };
        private void Open(ItemInstanceId id)
        { var data = Data(RectTransformUtility.WorldToScreenPoint(null, View(id).transform.position), 2); ExecuteEvents.Execute(View(id), data, ExecuteEvents.pointerClickHandler); Canvas.ForceUpdateCanvases(); }
        private void RevealLower() { inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.3f; Canvas.ForceUpdateCanvases(); }
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
            Assert.That(State.Items.Count, Is.EqualTo(13)); Assert.That(State.Containers[Root].Entries.Count, Is.EqualTo(11));
            Assert.That(Object.FindObjectsByType<ExpandedInventory>().Length, Is.EqualTo(1));
            Assert.That(GameObject.Find("InventoryScreen"), Is.Not.Null);
            Assert.That(Object.FindObjectsByType<InventoryPointerHandler>().Length, Is.EqualTo(11));
            foreach (var item in State.Items.Values) Assert.That(item.Definition.Icon, Is.Not.Null);
            LogAssert.NoUnexpectedReceived(); yield return null;
        }
        [UnityTest] public IEnumerator DoubleClickOpensNestedBagsAndUpReturnsToParent()
        {
            var berkut = Find("berkut", Root); Open(berkut);
            Assert.That(inventory.Screen.Bag.Container, Is.EqualTo(State.Items[berkut].ChildContainerId));
            var mbss = Find("mbss"); Open(mbss);
            Assert.That(inventory.Screen.Bag.Container, Is.EqualTo(State.Items[mbss].ChildContainerId));
            Assert.That(GameObject.Find("Item-" + Find("ai2", State.Items[mbss].ChildContainerId)), Is.Not.Null);
            inventory.Screen.Back.onClick.Invoke(); yield return null;
            Assert.That(inventory.Screen.Bag.Container, Is.EqualTo(State.Items[berkut].ChildContainerId)); LogAssert.NoUnexpectedReceived();
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
        [UnityTest] public IEnumerator RigHasTenSectionsAndGapIsNotDropTarget()
        {
            Open(Find("rig", Root)); RevealLower(); Assert.That(inventory.Screen.Bag.Sections.Count, Is.EqualTo(10));
            var ammo = Find("pst", Root, 20); var before = State; var data = Begin(ammo);
            var tall = inventory.Screen.Bag.Sections[new GridSectionId("tall-a")];
            var gap = tall.Geometry.ScreenPoint(1, 0, new Vector2(25, 25)); Drop(ammo, data, gap);
            Assert.That(State, Is.SameAs(before));
            data = Begin(ammo); Drop(ammo, data, Point(inventory.Screen.Bag.Container, new GridSectionId("small-a"), 0, 0)); yield return null;
            Assert.That(State.Registry.TryGetOwner(ammo, out var owner), Is.True); Assert.That(owner, Is.EqualTo(inventory.Screen.Bag.Container)); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator NavigationCancelsDragAndResetDoesNotDuplicateViews()
        {
            var berkut = Find("berkut", Root); Open(berkut); Begin(Find("mbss"));
            inventory.Screen.Close.onClick.Invoke(); Assert.That(inventory.Interaction.Drag, Is.Null);
            inventory.Screen.Reset.onClick.Invoke(); inventory.Screen.Reset.onClick.Invoke(); yield return null;
            Assert.That(State.Items.Count, Is.EqualTo(13)); Assert.That(Object.FindObjectsByType<InventoryPointerHandler>().Length, Is.EqualTo(11));
            Assert.That(inventory.Screen.Bag.Root.gameObject.activeSelf, Is.False); LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ClickIndicatorIsHiddenWhenReleased()
        {
            inventory.Indicator.Show(new Vector2(300, 300), true); Assert.That(GameObject.Find("ClickIndicator"), Is.Not.Null);
            inventory.Indicator.Show(new Vector2(300, 300), false); Assert.That(GameObject.Find("ClickIndicator"), Is.Null);
            yield return null; LogAssert.NoUnexpectedReceived();
        }
    }
}
