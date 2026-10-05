using System;
using InventorySystem.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.Tests
{
    public sealed class ContainerAuthoringTests
    {
        private ContainerDefinition container;
        private ContainerLayoutDefinition layout;
        private ContainerAuthoringSession session;

        [SetUp] public void SetUp()
        {
            container = ScriptableObject.CreateInstance<ContainerDefinition>(); layout = ScriptableObject.CreateInstance<ContainerLayoutDefinition>();
            using var data = new SerializedObject(container); data.FindProperty("identifier").stringValue = "authoring-test";
            data.FindProperty("layout").objectReferenceValue = layout; var sections = data.FindProperty("sections"); sections.arraySize = 2;
            using var positions = new SerializedObject(layout); var entries = positions.FindProperty("sections"); entries.arraySize = 2;
            for (int i = 0; i < 2; i++)
            {
                var section = sections.GetArrayElementAtIndex(i); section.FindPropertyRelative("identifier").stringValue = "pocket-" + i;
                section.FindPropertyRelative("width").intValue = 1; section.FindPropertyRelative("height").intValue = 2;
                var entry = entries.GetArrayElementAtIndex(i); entry.FindPropertyRelative("sectionId").stringValue = "pocket-" + i;
                entry.FindPropertyRelative("position").vector2Value = new Vector2(i * 1.12f, 0);
            }
            data.ApplyModifiedPropertiesWithoutUndo(); positions.ApplyModifiedPropertiesWithoutUndo(); session = new ContainerAuthoringSession(container);
            Undo.ClearAll();
        }

        [TearDown] public void TearDown()
        {
            Undo.ClearAll();
            using var data = new SerializedObject(container); var current = data.FindProperty("layout").objectReferenceValue;
            if (current != null && current != layout) UnityEngine.Object.DestroyImmediate(current);
            UnityEngine.Object.DestroyImmediate(container); if (layout != null) UnityEngine.Object.DestroyImmediate(layout);
        }

        [Test] public void RenameResizeAndMoveUndoTogether()
        {
            session.Update(0, "large", 2, 3, new Vector2(4, 2));
            AssertPocket(session.Read()[0], "large", 2, 3, new Vector2(4, 2));
            AssertLayoutId(0, "large");
            Undo.PerformUndo(); AssertPocket(session.Read()[0], "pocket-0", 1, 2, Vector2.zero); AssertLayoutId(0, "pocket-0");
            Undo.PerformRedo(); AssertPocket(session.Read()[0], "large", 2, 3, new Vector2(4, 2)); AssertLayoutId(0, "large");
        }

        [Test] public void AddedPocketHasUniqueIdAndFreshRules()
        {
            using (var data = new SerializedObject(container))
            {
                var rules = data.FindProperty("sections").GetArrayElementAtIndex(1).FindPropertyRelative("policy");
                rules.FindPropertyRelative("mode").enumValueIndex = 1;
                var denied = rules.FindPropertyRelative("deniedItemIds"); denied.arraySize = 1; denied.GetArrayElementAtIndex(0).stringValue = "pst";
                data.ApplyModifiedPropertiesWithoutUndo();
            }
            int index = session.Add(); var added = session.Read()[index]; Assert.That(added.Id, Is.EqualTo("pocket-2"));
            Assert.That(added.Position.x, Is.GreaterThan(2.12f)); AssertLayoutId(index, added.Id);
            using var updated = new SerializedObject(container); var policy = updated.FindProperty("sections").GetArrayElementAtIndex(index).FindPropertyRelative("policy");
            Assert.That(policy.FindPropertyRelative("mode").enumValueIndex, Is.Zero); Assert.That(policy.FindPropertyRelative("deniedItemIds").arraySize, Is.Zero);
            Undo.PerformUndo(); Assert.That(session.Read().Length, Is.EqualTo(2));
            Undo.PerformRedo(); Assert.That(session.Read().Length, Is.EqualTo(3));
        }

        [Test] public void RemoveSynchronizesLayoutAndUndoRestoresBoth()
        {
            session.Remove(0); Assert.That(session.Read()[0].Id, Is.EqualTo("pocket-1")); AssertLayoutId(0, "pocket-1");
            Undo.PerformUndo(); Assert.That(session.Read().Length, Is.EqualTo(2)); AssertLayoutId(0, "pocket-0"); AssertLayoutId(1, "pocket-1");
        }

        [TestCase("pocket-1", 1, 2, 0f)]
        [TestCase("", 1, 2, 0f)]
        [TestCase("valid", 0, 2, 0f)]
        [TestCase("valid", 1, 2, -1f)]
        [TestCase("valid", 1, 2, float.NaN)]
        public void InvalidEditDoesNotChangeEitherAsset(string id, int width, int height, float x)
        {
            var containerBefore = EditorJsonUtility.ToJson(container); var layoutBefore = EditorJsonUtility.ToJson(layout);
            Assert.Throws<ArgumentException>(() => session.Update(0, id, width, height, new Vector2(x, 0)));
            Assert.That(EditorJsonUtility.ToJson(container), Is.EqualTo(containerBefore)); Assert.That(EditorJsonUtility.ToJson(layout), Is.EqualTo(layoutBefore));
        }

        [Test] public void MissingLayoutIsCreatedUsingExistingFallbackAndUndoRestoresNull()
        {
            using (var data = new SerializedObject(container)) { data.FindProperty("layout").objectReferenceValue = null; data.ApplyModifiedPropertiesWithoutUndo(); }
            session.Update(0, "first", 2, 2, new Vector2(0, 1));
            Assert.That(session.Read()[1].Position, Is.EqualTo(new Vector2(2, 0)));
            Undo.PerformUndo(); using var reverted = new SerializedObject(container);
            Assert.That(reverted.FindProperty("layout").objectReferenceValue, Is.Null); Assert.That(session.Read()[0].Id, Is.EqualTo("pocket-0"));
            Undo.PerformRedo(); AssertPocket(session.Read()[0], "first", 2, 2, new Vector2(0, 1));
        }

        [Test] public void NextCatalogSnapshotReadsDirectAssetEditsWithoutGenerator()
        {
            var source = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset");
            var catalog = UnityEngine.Object.Instantiate(source);
            var rig = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/Items/Expansion/item-rig.asset"));
            try
            {
                using (var data = new SerializedObject(rig)) { data.FindProperty("container").objectReferenceValue = container; data.ApplyModifiedPropertiesWithoutUndo(); }
                using (var data = new SerializedObject(catalog))
                {
                    var items = data.FindProperty("items");
                    for (int i = 0; i < items.arraySize; i++)
                        if (items.GetArrayElementAtIndex(i).objectReferenceValue.name == "item-rig") items.GetArrayElementAtIndex(i).objectReferenceValue = rig;
                    data.ApplyModifiedPropertiesWithoutUndo();
                }
                var factory = new InventoryCatalogSnapshotFactory(); var before = factory.Create(catalog); before.TryGet(new DefinitionId("rig"), out var oldRig);
                session.Update(0, "wide", 2, 2, new Vector2(3, 0));
                var after = factory.Create(catalog); after.TryGet(new DefinitionId("rig"), out var newRig);
                Assert.That(oldRig.Container.Sections[0].Width, Is.EqualTo(1)); Assert.That(newRig.Container.Sections[0].Width, Is.EqualTo(2));
                Assert.That(newRig.Container.Layout.Sections[0].X, Is.EqualTo(3));
            }
            finally { UnityEngine.Object.DestroyImmediate(catalog); UnityEngine.Object.DestroyImmediate(rig); }
        }

        [Test] public void ContainerAssetUsesCustomInspector()
        {
            var editor = UnityEditor.Editor.CreateEditor(container);
            try { Assert.That(editor, Is.TypeOf<ContainerDefinitionEditor>()); }
            finally { UnityEngine.Object.DestroyImmediate(editor); }
        }

        private static void AssertPocket(PocketAuthoringData pocket, string id, int width, int height, Vector2 position)
        { Assert.That((pocket.Id, pocket.Width, pocket.Height, pocket.Position), Is.EqualTo((id, width, height, position))); }
        private void AssertLayoutId(int index, string id)
        {
            using var data = new SerializedObject(layout);
            Assert.That(data.FindProperty("sections").GetArrayElementAtIndex(index).FindPropertyRelative("sectionId").stringValue, Is.EqualTo(id));
        }
    }
}
