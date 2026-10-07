using System;
using System.Collections.Generic;
using System.Linq;
using Pktony.GridInventory.Domain;
using Pktony.GridInventory.Presentation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Tests
{
    public sealed class ExpandedCatalogTests
    {
        private Texture2D texture;
        private Sprite icon;
        private readonly CategoryDefinitionView category = new("Ammo", "Ammo");
        [SetUp] public void SetUp() { texture = new Texture2D(1, 1); icon = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero); }
        [TearDown] public void TearDown() { UnityEngine.Object.DestroyImmediate(icon); UnityEngine.Object.DestroyImmediate(texture); }
        private ItemDefinitionView Item(ContainerDefinitionView container = null, int max = 1, string categoryId = "Ammo")
            => new(new DefinitionId("sample"), "Sample", 1, 1, max, icon, categoryId, container);
        [Test] public void ParentCycleAndMissingParentRejectCatalog()
        {
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { new CategoryDefinitionView("Ammo", "Ammo", "Child"), new CategoryDefinitionView("Child", "Child", "Ammo") }, new[] { Item() }));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { new CategoryDefinitionView("Ammo", "Ammo", "Missing") }, new[] { Item() }));
        }
        [Test] public void DuplicateCategoryAndMissingItemCategoryRejectCatalog()
        {
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category, category }, new[] { Item() }));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(categoryId: "Missing") }));
        }
        [Test] public void PolicyReferencesMustExist()
        {
            var c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2) }, new AcceptancePolicyView(AcceptanceMode.AllowListed, new[] { "Missing" }));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
            c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2) }, new AcceptancePolicyView(AcceptanceMode.AllowAll, deniedItems: new[] { new DefinitionId("Missing") }));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
        }
        [Test] public void ContainerCannotStack()
        {
            var c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2) });
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c, 2) }));
        }
        [Test] public void DuplicateSectionAndInvalidSizeRejectCatalog()
        {
            var c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2), new GridSectionDefinitionView("main", 2, 2) });
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
            c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 0, 2) });
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
        }
        [Test] public void MissingLayoutSectionAndNonfinitePositionRejectCatalog()
        {
            var c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2) },
                layout: new ContainerLayoutView(Array.Empty<SectionLayoutView>()));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
            c = new ContainerDefinitionView("bag", new[] { new GridSectionDefinitionView("main", 2, 2) },
                layout: new ContainerLayoutView(new[] { new SectionLayoutView("main", float.NaN, 0) }));
            Assert.Throws<CatalogValidationException>(() => new InventoryCatalog(new[] { category }, new[] { Item(c) }));
        }
        [Test] public void PolicyArraysAndLayoutsAreDefensiveCopies()
        {
            var allowed = new[] { "Ammo" }; var policy = new AcceptancePolicyView(AcceptanceMode.AllowListed, allowed);
            allowed[0] = "Missing"; Assert.That(policy.AllowedCategories[0], Is.EqualTo("Ammo"));
            Assert.Throws<NotSupportedException>(() => ((IList<string>)policy.AllowedCategories)[0] = "Missing");
            var positions = new[] { new SectionLayoutView("main", 0, 0) }; var layout = new ContainerLayoutView(positions);
            positions[0] = null; Assert.That(layout.Sections[0].SectionId, Is.EqualTo("main"));
        }
        [Test] public void SectionAllowanceCannotOverrideCommonDenial()
        {
            var catalog = new InventoryCatalog(new[] { category }, new[] { Item() });
            var definition = new ContainerDefinitionView("restricted", new[] { new GridSectionDefinitionView("main", 2, 2,
                new AcceptancePolicyView(AcceptanceMode.AllowListed, new[] { "Ammo" })) },
                new AcceptancePolicyView(AcceptanceMode.AllowAll, deniedCategories: new[] { "Ammo" }));
            using var runtime = new InventoryRuntime(catalog, definition);
            var before = runtime.ReadModel.Snapshot;
            Assert.That(runtime.Edit.Add(new AddRequest(new DefinitionId("sample"), 1,
                new PlacementTarget(before.RootContainerId, new GridSectionId("main"), 0, 0))).Success, Is.False);
            Assert.That(runtime.ReadModel.Snapshot, Is.SameAs(before));
        }
        [Test] public void PreparationExceptionPreservesStateAndReleasesGuard()
        {
            var catalog = new InventoryCatalog(new[] { category }, new[] { Item() });
            using var runtime = new InventoryRuntime(catalog, new ContainerDefinitionView("stash", new[] { new GridSectionDefinitionView("main", 2, 2) }));
            var before = runtime.ReadModel.Snapshot;
            Assert.That(runtime.Reset.Replace(null).Success, Is.False); Assert.That(runtime.ReadModel.Snapshot, Is.SameAs(before));
            Assert.That(runtime.Edit.Add(new AddRequest(new DefinitionId("sample"), 1,
                new PlacementTarget(before.RootContainerId, new GridSectionId("main"), 0, 0))).Success, Is.True);
        }
        [Test] public void PresetCatalogHasNineteenDefinitionsAndTwentyBlackRockCells()
        {
            var asset = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/ModularGridInventory/Samples/Catalog/Catalog.asset");
            var catalog = new InventoryCatalogSnapshotFactory().Create(asset);
            Assert.That(catalog.Definitions.Count, Is.EqualTo(19));
            catalog.TryGet(new DefinitionId("rig"), out var rig); Assert.That(rig.Container.Sections.Sum(s => s.Width * s.Height), Is.EqualTo(20));
            Assert.That(rig.Container.Sections.Count, Is.EqualTo(11));
        }
        [Test] public void SourceContainerEditsDoNotChangeLiveSession()
        {
            var asset = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/ModularGridInventory/Samples/Catalog/Catalog.asset");
            var clone = UnityEngine.Object.Instantiate(asset);
            var original = AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/ModularGridInventory/Samples/Catalog/item-mbss.asset");
            var item = UnityEngine.Object.Instantiate(original);
            var container = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<ContainerDefinition>("Assets/ModularGridInventory/Samples/Catalog/container-mbss.asset"));
            try
            {
                var source = new SerializedObject(clone); source.FindProperty("items").GetArrayElementAtIndex(0).objectReferenceValue = item; source.ApplyModifiedPropertiesWithoutUndo();
                var itemData = new SerializedObject(item); itemData.FindProperty("container").objectReferenceValue = container; itemData.ApplyModifiedPropertiesWithoutUndo();
                var factory = new InventoryCatalogSnapshotFactory(); var live = factory.Create(clone);
                var data = new SerializedObject(container); data.FindProperty("sections").GetArrayElementAtIndex(0).FindPropertyRelative("width").intValue = 8;
                data.FindProperty("policy").FindPropertyRelative("mode").enumValueIndex = 1; data.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(live.Definitions[0].Container.Sections[0].Width, Is.EqualTo(4));
                Assert.That(live.Definitions[0].Container.Policy.Mode, Is.EqualTo(AcceptanceMode.AllowAll));
                Assert.That(factory.Create(clone).Definitions[0].Container.Sections[0].Width, Is.EqualTo(8));
            }
            finally { UnityEngine.Object.DestroyImmediate(container); UnityEngine.Object.DestroyImmediate(item); UnityEngine.Object.DestroyImmediate(clone); }
        }
        [Test] public void PresentationCannotReferenceMutableRuntimeImplementation()
        {
            var names = typeof(InventoryInteractionController).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
            Assert.That(names, Does.Not.Contain("Pktony.GridInventory.Runtime"));
            Assert.That(typeof(InventorySnapshot).Assembly.GetType("Pktony.GridInventory.Domain.InventorySession").IsNotPublic, Is.True);
            Assert.That(typeof(InventorySnapshot).Assembly.GetReferencedAssemblies().Select(a => a.Name), Does.Not.Contain("Pktony.GridInventory.Presentation"));
        }
    }
}
