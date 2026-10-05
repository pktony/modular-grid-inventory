using System;
using System.Linq;
using InventorySystem.Presentation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Tests
{
    public sealed class InventoryAudioSettingsTests
    {
        private InventoryAudioSettings settings;
        private InventoryCatalog catalog;
        private ItemDefinitionView item;
        private AudioClip first, second;
        [SetUp] public void SetUp()
        {
            settings = ScriptableObject.CreateInstance<InventoryAudioSettings>();
            first = AudioClip.Create("First", 100, 1, 44100, false); second = AudioClip.Create("Second", 100, 1, 44100, false);
            var icon = new InventoryCatalogSnapshotFactory().Create(AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset")).Definitions.First().Icon;
            item = new ItemDefinitionView(new DefinitionId("test"), "Test", 1, 1, 1, icon, "leaf", null);
            catalog = new InventoryCatalog(new[] { new CategoryDefinitionView("root", "Root"),
                new CategoryDefinitionView("middle", "Middle", "root"), new CategoryDefinitionView("leaf", "Leaf", "middle") }, new[] { item });
        }
        [TearDown] public void TearDown()
        { UnityEngine.Object.DestroyImmediate(settings); UnityEngine.Object.DestroyImmediate(first); UnityEngine.Object.DestroyImmediate(second); }
        private InventorySoundBinding Binding(InventoryFeedbackAction action, params AudioClip[] clips) => new() { action = action, clips = clips };
        [Test] public void ClosestActualCategoryAncestorOverridesParentAndFallsBackPerAction()
        {
            settings.defaults = new[] { Binding(InventoryFeedbackAction.Rotate, first) };
            settings.itemProfiles = new[] {
                new InventoryItemSoundProfile { categoryId = "root", sounds = new[] { Binding(InventoryFeedbackAction.Place, first) } },
                new InventoryItemSoundProfile { categoryId = "middle", sounds = new[] { Binding(InventoryFeedbackAction.Place, second) } } };
            var resolver = new InventorySoundResolver(settings, catalog);
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Place, item, out _), Is.SameAs(second));
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Rotate, item, out _), Is.SameAs(first));
        }
        [Test] public void EmptyProfilesInheritAndVariantsSkipNullWithoutRepeatingFirst()
        {
            settings.defaults = new[] { Binding(InventoryFeedbackAction.Place, null, first, second) };
            settings.itemProfiles = new[] { new InventoryItemSoundProfile { categoryId = "leaf", sounds = new[] { Binding(InventoryFeedbackAction.Place, (AudioClip)null) } } };
            var resolver = new InventorySoundResolver(settings, catalog);
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Place, item, out _), Is.SameAs(first));
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Place, item, out _), Is.SameAs(second));
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Place, item, out _), Is.SameAs(first));
            Assert.That(resolver.Resolve(InventoryFeedbackAction.Reject, item, out _), Is.Null);
            Assert.That(new InventorySoundResolver(null, catalog).Resolve(InventoryFeedbackAction.Place, item, out _), Is.Null);
        }
        [Test] public void DefaultAssetCoversEveryActionAndItsClipsContainSamples()
        {
            var asset = Resources.Load<InventoryAudioSettings>("InventoryAudioSettings"); Assert.That(asset, Is.Not.Null);
            foreach (InventoryFeedbackAction action in Enum.GetValues(typeof(InventoryFeedbackAction)))
                Assert.That(asset.defaults.Any(b => b.action == action && b.clips.Any(c => c != null)), Is.True, action.ToString());
            var actualCatalog = new InventoryCatalogSnapshotFactory().Create(AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset"));
            foreach (var profile in asset.itemProfiles) Assert.That(actualCatalog.Categories.Any(c => c.Id == profile.categoryId), Is.True, profile.categoryId);
            foreach (var clip in asset.defaults.Concat(asset.itemProfiles.SelectMany(p => p.sounds)).SelectMany(b => b.clips).Distinct())
            {
                var samples = new float[clip.samples * clip.channels];
                Assert.That(clip.GetData(samples, 0), Is.True, clip.name);
                Assert.That(samples.Any(s => Mathf.Abs(s) > 0.0001f), Is.True, clip.name);
            }
        }
    }
}
