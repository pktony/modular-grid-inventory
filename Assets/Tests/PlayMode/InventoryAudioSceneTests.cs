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
namespace Pktony.GridInventory.Tests
{
    public sealed class InventoryAudioSceneTests
    {
        private InventoryBootstrapper inventory;
        private InventoryAudioSettings settings;
        private InventorySoundPresenter observer;
        private InventoryAudioProbe probe;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        private ContainerId Root => State.RootContainerId;
        [UnitySetUp] public IEnumerator LoadScene()
        {
            yield return SceneManager.LoadSceneAsync("Inventory"); yield return null;
            inventory = Object.FindAnyObjectByType<InventoryBootstrapper>(); inventory.enabled = false;
            settings = Resources.Load<InventoryAudioSettings>("InventoryAudioSettings"); probe = new InventoryAudioProbe();
            var catalog = (InventoryCatalog)typeof(InventoryBootstrapper).GetField("definitions",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(inventory);
            observer = new InventorySoundPresenter(inventory.ReadModel, inventory.Interaction, inventory.Windows, inventory.Screen,
                new InventorySoundResolver(settings, catalog), probe);
            inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.78f; Canvas.ForceUpdateCanvases();
        }
        [UnityTearDown] public IEnumerator CleanUp() { observer?.Dispose(); yield return null; }
        private ItemInstanceId Find(string definition, int quantity = 0) => State.Items.Values.First(i => i.Definition.Identifier == definition
            && (quantity == 0 || i.Quantity == quantity) && State.Containers[Root].Entries.ContainsKey(i.Id)).Id;
        private Vector2 Point(int x, int y) => inventory.Screen.Stash.Sections[new GridSectionId("main")].Geometry.ScreenPoint(x, y, new Vector2(25, 25));
        private PointerEventData Begin(ItemInstanceId id)
        {
            var entry = State.Containers[Root].Entries[id]; var point = Point(entry.X, entry.Y);
            var data = new PointerEventData(EventSystem.current) { position = point, pressPosition = point };
            var view = GameObject.Find("Item-" + id); ExecuteEvents.Execute(view, data, ExecuteEvents.beginDragHandler); return data;
        }
        private void Drop(ItemInstanceId id, PointerEventData data, Vector2 point)
        { data.position = point; var view = GameObject.Find("Item-" + id); ExecuteEvents.Execute(view, data, ExecuteEvents.dragHandler); ExecuteEvents.Execute(view, data, ExecuteEvents.endDragHandler); }
        private AudioClip Default(InventoryFeedbackAction action) => settings.defaults.Single(b => b.action == action).clips[0];
        [UnityTest] public IEnumerator PreviewIsSilentAndRejectedDropPlaysOnceWithoutCancelOrMutation()
        {
            var ammo = Find("ammo-light", 20); var before = State; var data = Begin(ammo); probe.Clips.Clear();
            for (int i = 0; i < 20; i++) inventory.Interaction.UpdatePointer(new Vector2(-10, -10));
            Assert.That(probe.Clips, Is.Empty);
            Drop(ammo, data, new Vector2(-10, -10)); inventory.Interaction.Drop(new Vector2(-10, -10)); inventory.Interaction.Cancel();
            Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Reject) })); Assert.That(State, Is.SameAs(before));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator SuccessfulPlacementAndExplicitRotateCancelHaveDistinctFeedback()
        {
            var ammo = Find("ammo-light", 20); var data = Begin(ammo); var pickup = probe.Clips.Single(); probe.Clips.Clear();
            Drop(ammo, data, Point(8, 13));
            Assert.That(probe.Clips.Count, Is.EqualTo(1)); Assert.That(probe.Clips[0], Is.Not.SameAs(pickup));
            Assert.That(probe.Clips.Contains(Default(InventoryFeedbackAction.Cancel)), Is.False);
            data = Begin(ammo); probe.Clips.Clear(); inventory.Interaction.Rotate(); inventory.Interaction.Escape(); inventory.Interaction.Escape();
            Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Rotate), Default(InventoryFeedbackAction.Cancel) }));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ConsumedMergeSourceStillReportsExactlyOneMergeSound()
        {
            var source = Find("ammo-light", 40); Assert.That(inventory.Stack.Split(new SplitRequest(source, 5, new PlacementTarget(Root, new GridSectionId("main"), 8, 13))).Success, Is.True);
            var split = Find("ammo-light", 5); var destination = Find("ammo-light", 20); var entry = State.Containers[Root].Entries[destination];
            var data = Begin(split); probe.Clips.Clear(); Drop(split, data, Point(entry.X, entry.Y));
            Assert.That(State.Items.ContainsKey(split), Is.False); Assert.That(State.Items[destination].Quantity, Is.EqualTo(25));
            Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Merge) })); yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ExistingWindowFocusIsSilentAndResetDoesNotPlayMultipleCloseSounds()
        {
            var bag = Find("pack-large"); inventory.Interaction.Open(bag); inventory.Interaction.Open(bag);
            Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Open) }));
            inventory.Interaction.Escape(); Assert.That(probe.Clips.Last(), Is.SameAs(Default(InventoryFeedbackAction.Close)));
            inventory.Interaction.Open(bag); inventory.Interaction.Open(Find("case-ammo")); probe.Clips.Clear();
            inventory.Screen.Reset.onClick.Invoke(); Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Reset) }));
            yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator SplitValidationAndPlacementHaveSeparateSoundsAndDisposeUnsubscribes()
        {
            var source = Find("ammo-light", 40); inventory.Interaction.Split(source); probe.Clips.Clear();
            GameObject.Find("Input").GetComponent<TMP_InputField>().text = "40"; inventory.Screen.Quantity.Confirm();
            Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Reject) })); Assert.That(inventory.Screen.Quantity.IsOpen, Is.True);
            GameObject.Find("Input").GetComponent<TMP_InputField>().text = "5"; inventory.Screen.Quantity.Confirm(); probe.Clips.Clear();
            inventory.Interaction.Drop(Point(8, 13)); Assert.That(probe.Clips, Is.EqualTo(new[] { Default(InventoryFeedbackAction.Split) }));
            observer.Dispose(); probe.Clips.Clear(); inventory.Interaction.Split(source); inventory.Interaction.Cancel(); inventory.Screen.Reset.onClick.Invoke();
            Assert.That(probe.Clips, Is.Empty); yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator SceneHasOneListenerAndSixTwoDimensionalVoices()
        {
            Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Count(l => l.enabled), Is.EqualTo(1));
            var output = Object.FindAnyObjectByType<InventoryAudioOutput>(); var voices = output.GetComponents<AudioSource>();
            Assert.That(voices.Length, Is.EqualTo(6)); Assert.That(voices.All(v => v.spatialBlend == 0 && !v.playOnAwake && !v.loop), Is.True);
            output.Play(Default(InventoryFeedbackAction.Open), 0.5f); yield return null;
            Assert.That(voices.Any(v => v.isPlaying), Is.True); Assert.That(voices.All(v => v.volume == settings.masterVolume), Is.True);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator MutedAndZeroVolumeDoNotQueueSoundsAndVolumeEditsApplyLive()
        {
            var temporary = Object.Instantiate(settings); temporary.muted = true;
            var root = new GameObject("TemporaryAudio"); var output = root.AddComponent<InventoryAudioOutput>(); output.Initialize(temporary);
            var voices = output.GetComponents<AudioSource>(); var clip = Default(InventoryFeedbackAction.Open);
            output.Play(clip, 0.5f); Assert.That(voices.Any(v => v.isPlaying), Is.False);
            temporary.muted = false; temporary.masterVolume = 0; output.Play(clip, 0.5f);
            Assert.That(voices.Any(v => v.isPlaying), Is.False);
            temporary.masterVolume = 0.35f;
            for (int i = 0; i < 20; i++) output.Play(clip, 0.5f);
            Assert.That(output.GetComponents<AudioSource>().Length, Is.EqualTo(6));
            temporary.masterVolume = 0.25f; temporary.muted = true; yield return null;
            Assert.That(voices.All(v => v.mute && v.volume == 0.25f), Is.True);
            Object.Destroy(root); Object.Destroy(temporary); yield return null; LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator RemovingInventoryComponentReleasesItsAudioObject()
        {
            observer.Dispose(); var output = Object.FindAnyObjectByType<InventoryAudioOutput>();
            Object.Destroy(inventory); yield return null; yield return null;
            Assert.That(output == null, Is.True); Assert.That(Object.FindAnyObjectByType<InventoryAudioOutput>(), Is.Null);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
