using System;
using System.Linq;
using Pktony.GridInventory.Domain;
using Pktony.GridInventory.Presentation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Tests
{
    public sealed class InventoryIntegrationTests
    {
        private const string Settings = "Assets/ModularGridInventory/Samples/Settings/";
        private InventoryInitialState seed;
        private InventoryCatalog catalog;
        private ContainerDefinitionView root;
        [SetUp] public void SetUp()
        {
            catalog = new InventoryCatalogSnapshotFactory().Create(AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/ModularGridInventory/Samples/Catalog/Catalog.asset"));
            root = new ContainerDefinitionSnapshotFactory().Create(AssetDatabase.LoadAssetAtPath<ContainerDefinition>(Settings + "RootContainer.asset"));
            seed = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<InventoryInitialState>(Settings + "InitialState.asset"));
        }
        [TearDown] public void TearDown() => UnityEngine.Object.DestroyImmediate(seed);
        private void Set(int index, string field, string value)
        {
            using var data = new SerializedObject(seed);
            data.FindProperty("entries").GetArrayElementAtIndex(index).FindPropertyRelative(field).stringValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
        [Test] public void SeedCreatesNestedContainersWithIndependentIds()
        {
            var state = new InventorySeedBuilder().Create(catalog, root, seed);
            Assert.That(state.Items.Count, Is.EqualTo(22));
            var inner = state.Items.Values.Single(i => i.Definition.Identifier == "pack-small");
            Assert.That(state.Registry.TryGetOwner(inner.Id, out var owner), Is.True);
            Assert.That(owner, Is.Not.EqualTo(state.RootContainerId));
            var second = new InventorySeedBuilder().Create(catalog, root, seed);
            Assert.That(second.RootContainerId, Is.Not.EqualTo(state.RootContainerId));
            Assert.That(second.Items.Keys.Intersect(state.Items.Keys), Is.Empty);
        }
        [Test] public void SeedSupportsChildrenBeforeParents()
        {
            using var data = new SerializedObject(seed);
            data.FindProperty("entries").MoveArrayElement(2, 0); data.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(new InventorySeedBuilder().Create(catalog, root, seed).Items.Count, Is.EqualTo(22));
        }
        [Test] public void NullSeedCreatesEmptyStorage()
        { Assert.That(new InventorySeedBuilder().Create(catalog, root, null).Items, Is.Empty); }
        [Test] public void MissingParentRejectsSeed()
        { Set(0, "parentKey", "missing"); Assert.Throws<InvalidOperationException>(() => new InventorySeedBuilder().Create(catalog, root, seed)); }
        [Test] public void ContainerCycleRejectsSeed()
        { Set(0, "parentKey", "pack-inner"); Assert.Throws<InvalidOperationException>(() => new InventorySeedBuilder().Create(catalog, root, seed)); }
        [Test] public void NonContainerParentRejectsSeed()
        { Set(0, "parentKey", "medical"); Assert.Throws<InvalidOperationException>(() => new InventorySeedBuilder().Create(catalog, root, seed)); }
        [Test] public void DuplicateKeysRejectSeed()
        { Set(1, "key", "pack-outer"); Assert.Throws<InvalidOperationException>(() => new InventorySeedBuilder().Create(catalog, root, seed)); }
        [Test] public void InvalidSeedCannotMutateRunningInventory()
        {
            using var runtime = new InventoryRuntime(catalog, root); var before = runtime.ReadModel.Snapshot;
            Set(0, "parentKey", "missing");
            Assert.Throws<InvalidOperationException>(() => runtime.Reset.Replace(new InventorySeedBuilder().Create(catalog, root, seed)));
            Assert.That(runtime.ReadModel.Snapshot, Is.SameAs(before));
        }
        [Test] public void UiSettingsFreezeSourceColorsAndPitch()
        {
            var theme = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<InventoryUiTheme>(Settings + "InventoryTheme.asset"));
            try
            {
                var settings = new InventoryUiSettings(theme); var originalColor = settings.Palette.Cell;
                theme.Palette.Cell = Color.magenta;
                using var data = new SerializedObject(theme); data.FindProperty("cellPitch").floatValue = 64; data.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(settings.Palette.Cell, Is.EqualTo(originalColor)); Assert.That(settings.CellPitch, Is.EqualTo(50));
                Assert.That(new InventoryUiSettings(theme).CellPitch, Is.EqualTo(64));
            }
            finally { UnityEngine.Object.DestroyImmediate(theme); }
        }
        [Test] public void InvalidGridPitchRejectsUiInstallation()
        {
            var theme = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<InventoryUiTheme>(Settings + "InventoryTheme.asset"));
            try
            {
                using var data = new SerializedObject(theme); data.FindProperty("cellPitch").floatValue = 0; data.ApplyModifiedPropertiesWithoutUndo();
                Assert.Throws<ArgumentException>(() => new InventoryUiSettings(theme));
            }
            finally { UnityEngine.Object.DestroyImmediate(theme); }
        }
    }
}
