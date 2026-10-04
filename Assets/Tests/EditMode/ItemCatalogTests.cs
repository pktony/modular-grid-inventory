using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Tests
{
    public sealed class ItemCatalogTests
    {
        private readonly List<UnityEngine.Object> resources = new();
        private Sprite icon;
        [SetUp] public void SetUp()
        {
            var texture = new Texture2D(1, 1); resources.Add(texture);
            icon = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero); resources.Add(icon);
        }
        [TearDown] public void TearDown()
        {
            for (int i = resources.Count - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(resources[i]);
            resources.Clear();
        }
        [Test] public void SourceEditsDoNotChangeLiveItemOrCatalogLookup()
        {
            var source = CreateItem("ammo");
            var asset = CreateCatalog(source);
            var factory = new ItemCatalogSnapshotFactory();
            var catalog = factory.Create(asset);
            var item = new ItemData(catalog.Definitions[0]);
            var edits = new SerializedObject(source);
            edits.FindProperty("identifier").stringValue = "replacement";
            edits.FindProperty("displayName").stringValue = "New name";
            edits.FindProperty("width").intValue = 3;
            edits.FindProperty("maxStack").intValue = 60;
            edits.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(item.Definition.Identifier, Is.EqualTo("ammo"));
            Assert.That(item.Definition.DisplayName, Is.EqualTo("Sample"));
            Assert.That(item.Definition.Width, Is.EqualTo(item.width).And.EqualTo(1));
            Assert.That(item.Definition.MaxStack, Is.EqualTo(50));
            Assert.That(catalog.TryGet(new DefinitionId("ammo"), out var shared), Is.True);
            Assert.That(shared, Is.SameAs(item.Definition));
            Assert.That(catalog.TryGet(new DefinitionId("replacement"), out _), Is.False);
            var next = factory.Create(asset);
            Assert.That(next.Definitions[0].Width, Is.EqualTo(3));
            Assert.That(next.Definitions[0].MaxStack, Is.EqualTo(60));
        }
        [Test] public void CatalogReplacementDoesNotChangeExistingSnapshot()
        {
            var asset = CreateCatalog(CreateItem("first"));
            var factory = new ItemCatalogSnapshotFactory();
            var previous = factory.Create(asset);
            var data = new SerializedObject(asset);
            data.FindProperty("definitions").GetArrayElementAtIndex(0).objectReferenceValue = CreateItem("second");
            data.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(previous.Definitions[0].Identifier, Is.EqualTo("first"));
            Assert.That(factory.Create(asset).Definitions[0].Identifier, Is.EqualTo("second"));
        }
        [Test] public void CallerArrayAndCollectionCannotReplaceSnapshotDefinitions()
        {
            var view = new ItemDefinitionView(new DefinitionId("ammo"), "Sample", 1, 1, 50, icon);
            var input = new[] { view };
            var catalog = new ItemCatalogSnapshot(input);
            input[0] = null;
            Assert.That(catalog.Definitions[0], Is.SameAs(view));
            var exposed = (IList<ItemDefinitionView>)catalog.Definitions;
            Assert.Throws<NotSupportedException>(() => exposed[0] = null);
            Assert.That(catalog.TryGet(view.Id, out var found), Is.True);
            Assert.That(found, Is.SameAs(view));
        }
        [Test] public void DuplicateDefinitionIdsRejectWholeCatalog()
        {
            var source = CreateCatalog(CreateItem("ammo"), CreateItem("ammo"));
            var error = Assert.Throws<CatalogValidationException>(() => new ItemCatalogSnapshotFactory().Create(source));
            Assert.That(error.Message, Does.Contain("duplicate item ID 'ammo'"));
        }
        [Test] public void EmptyCatalogRejectsStartup() => Assert.Throws<CatalogValidationException>(
            () => new ItemCatalogSnapshotFactory().Create(CreateCatalog()));
        [Test] public void MissingReferenceRejectsWholeCatalog() => Assert.Throws<CatalogValidationException>(
            () => new ItemCatalogSnapshotFactory().Create(CreateCatalog(CreateItem("valid"), null)));
        [TestCase(0, 1, 50)] [TestCase(1, 0, 50)] [TestCase(-1, 1, 50)]
        [TestCase(1, 1, 0)] [TestCase(1, 1, -1)]
        public void InvalidSizeOrStackRejectsCatalog(int width, int height, int maxStack)
        {
            var view = new ItemDefinitionView(new DefinitionId("ammo"), "Sample", width, height, maxStack, icon);
            Assert.Throws<CatalogValidationException>(() => new ItemCatalogSnapshot(new[] { view }));
        }
        [TestCase("")] [TestCase(" ")]
        public void BlankIdentityRejectsCatalog(string id) => Assert.Throws<CatalogValidationException>(
            () => new ItemCatalogSnapshot(new[] { new ItemDefinitionView(new DefinitionId(id), "Sample", 1, 1, 1, icon) }));
        [Test] public void MissingIconRejectsCatalog() => Assert.Throws<CatalogValidationException>(
            () => new ItemCatalogSnapshot(new[] { new ItemDefinitionView(new DefinitionId("ammo"), "Sample", 1, 1, 1, null) }));
        private ItemDefinition CreateItem(string id)
        {
            var item = ScriptableObject.CreateInstance<ItemDefinition>(); resources.Add(item);
            var data = new SerializedObject(item);
            data.FindProperty("identifier").stringValue = id;
            data.FindProperty("displayName").stringValue = "Sample";
            data.FindProperty("width").intValue = 1;
            data.FindProperty("height").intValue = 1;
            data.FindProperty("maxStack").intValue = 50;
            data.FindProperty("icon").objectReferenceValue = icon;
            data.ApplyModifiedPropertiesWithoutUndo();
            return item;
        }
        private ItemCatalog CreateCatalog(params ItemDefinition[] definitions)
        {
            var catalog = ScriptableObject.CreateInstance<ItemCatalog>(); resources.Add(catalog);
            var data = new SerializedObject(catalog);
            var items = data.FindProperty("definitions"); items.arraySize = definitions.Length;
            for (int i = 0; i < definitions.Length; i++) items.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];
            data.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }
    }
}
