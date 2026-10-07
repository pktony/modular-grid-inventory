using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace InventorySystem.Tests
{
    public sealed class InventorySceneTests
    {
        [UnitySetUp] public IEnumerator LoadScene()
        { yield return SceneManager.LoadSceneAsync("Inventory"); yield return null; }
        [UnityTest] public IEnumerator SceneHasOneViewAndAllIcons()
        {
            var inventory = Object.FindAnyObjectByType<Inventory>();
            Assert.That(inventory.Model.Count, Is.EqualTo(7));
            Assert.That(Object.FindObjectsByType<InventoryUI>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<InventoryItemView>().Length, Is.EqualTo(7));
            foreach (var entry in inventory.Model.Entries) Assert.That(entry.Item.Definition.Icon, Is.Not.Null);
            LogAssert.NoUnexpectedReceived(); yield return null;
        }
        [UnityTest] public IEnumerator PointerDropMovesItemAndResetDoesNotDuplicateViews()
        {
            var inventory = Object.FindAnyObjectByType<Inventory>();
            var itemView = GameObject.Find("Assault rifle 1").GetComponent<InventoryItemView>();
            var geometry = new InventoryGridGeometry(GameObject.Find("Grid").GetComponent<RectTransform>(), 80);
            var start = RectTransformUtility.WorldToScreenPoint(null, geometry.WorldPosition(0, 0) + new Vector3(8, -8));
            var end = RectTransformUtility.WorldToScreenPoint(null, geometry.WorldPosition(0, 3) + new Vector3(8, -8));
            var data = new PointerEventData(EventSystem.current) { position = start, pressPosition = start, button = PointerEventData.InputButton.Left };
            var item = inventory.Model.GetItemAt(0, 0);
            ExecuteEvents.Execute(itemView.gameObject, data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(itemView.gameObject, data, ExecuteEvents.beginDragHandler);
            data.position = end;
            ExecuteEvents.Execute(itemView.gameObject, data, ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(itemView.gameObject, data, ExecuteEvents.endDragHandler);
            Assert.That(inventory.Model.GetEntry(item).Y, Is.EqualTo(3));
            var reset = GameObject.Find("Reset").GetComponent<Button>();
            reset.onClick.Invoke(); reset.onClick.Invoke(); yield return null;
            Assert.That(inventory.Model.Count, Is.EqualTo(7));
            Assert.That(Object.FindObjectsByType<InventoryItemView>().Length, Is.EqualTo(7));
            LogAssert.NoUnexpectedReceived();
        }
    }
}
