using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pktony.GridInventory.Editor
{
    public sealed class ContainerAuthoringSession
    {
        private readonly ContainerDefinition container;
        public ContainerAuthoringSession(ContainerDefinition container) => this.container = container;

        public PocketAuthoringData[] Read()
        {
            using var data = new SerializedObject(container);
            var layout = data.FindProperty("layout").objectReferenceValue;
            using var positions = layout != null ? new SerializedObject(layout) : null;
            var sections = data.FindProperty("sections"); var result = new PocketAuthoringData[sections.arraySize];
            float fallbackX = 0;
            for (int i = 0; i < result.Length; i++)
            {
                var section = sections.GetArrayElementAtIndex(i); var id = section.FindPropertyRelative("identifier").stringValue;
                int width = section.FindPropertyRelative("width").intValue, height = section.FindPropertyRelative("height").intValue;
                var position = positions == null ? new Vector2(fallbackX, 0) : FindPosition(positions, id)?.FindPropertyRelative("position").vector2Value ?? Vector2.zero;
                result[i] = new PocketAuthoringData(id, width, height, position); fallbackX += width + 1;
            }
            return result;
        }

        public void Update(int index, string id, int width, int height, Vector2 position)
        {
            var before = Read();
            if (index < 0 || index >= before.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (string.IsNullOrWhiteSpace(id) || before.Where((_, i) => i != index).Any(p => p.Id == id))
                throw new ArgumentException("Pocket ID must be non-empty and unique.");
            if (width < 1 || height < 1 || !float.IsFinite(position.x) || !float.IsFinite(position.y) || position.x < 0 || position.y < 0)
                throw new ArgumentException("Size must be positive; X/Y must be finite and non-negative.");
            Mutate("Edit inventory pocket", (data, layout) =>
            {
                var section = data.FindProperty("sections").GetArrayElementAtIndex(index);
                section.FindPropertyRelative("identifier").stringValue = id;
                section.FindPropertyRelative("width").intValue = width; section.FindPropertyRelative("height").intValue = height;
                var entry = FindPosition(layout, before[index].Id) ?? AppendPosition(layout);
                entry.FindPropertyRelative("sectionId").stringValue = id; entry.FindPropertyRelative("position").vector2Value = position;
            });
        }

        public int Add()
        {
            var before = Read(); int suffix = 1;
            while (before.Any(p => p.Id == "pocket-" + suffix)) suffix++;
            string id = "pocket-" + suffix;
            float x = before.Length == 0 ? 0 : before.Max(p => p.Position.x + p.Width) + 0.12f;
            Mutate("Add inventory pocket", (data, layout) =>
            {
                var sections = data.FindProperty("sections"); int index = sections.arraySize++;
                var section = sections.GetArrayElementAtIndex(index);
                section.FindPropertyRelative("identifier").stringValue = id;
                section.FindPropertyRelative("width").intValue = 1; section.FindPropertyRelative("height").intValue = 1;
                var policy = section.FindPropertyRelative("policy"); policy.FindPropertyRelative("mode").enumValueIndex = 0;
                foreach (var field in new[] { "allowedCategories", "deniedCategories", "allowedItemIds", "deniedItemIds" })
                    policy.FindPropertyRelative(field).arraySize = 0;
                var entry = AppendPosition(layout); entry.FindPropertyRelative("sectionId").stringValue = id;
                entry.FindPropertyRelative("position").vector2Value = new Vector2(x, 0);
            });
            return before.Length;
        }

        public void Remove(int index)
        {
            var before = Read();
            if (index < 0 || index >= before.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (before.Length <= 1) throw new InvalidOperationException("Keep at least one pocket.");
            Mutate("Remove inventory pocket", (data, layout) =>
            {
                data.FindProperty("sections").DeleteArrayElementAtIndex(index);
                var entries = layout.FindProperty("sections");
                for (int i = entries.arraySize - 1; i >= 0; i--)
                    if (entries.GetArrayElementAtIndex(i).FindPropertyRelative("sectionId").stringValue == before[index].Id)
                        entries.DeleteArrayElementAtIndex(i);
            });
        }

        private void Mutate(string label, Action<SerializedObject, SerializedObject> edit)
        {
            var before = Read(); Undo.IncrementCurrentGroup(); int group = Undo.GetCurrentGroup(); Undo.SetCurrentGroupName(label);
            using var data = new SerializedObject(container);
            var layout = (ContainerLayoutDefinition)data.FindProperty("layout").objectReferenceValue;
            if (layout == null)
            {
                layout = ScriptableObject.CreateInstance<ContainerLayoutDefinition>(); layout.name = container.name + " Layout";
                if (AssetDatabase.Contains(container)) AssetDatabase.AddObjectToAsset(layout, container);
                Undo.RegisterCreatedObjectUndo(layout, label); data.FindProperty("layout").objectReferenceValue = layout;
                using var initial = new SerializedObject(layout); var entries = initial.FindProperty("sections"); entries.arraySize = before.Length;
                for (int i = 0; i < before.Length; i++)
                {
                    var entry = entries.GetArrayElementAtIndex(i); entry.FindPropertyRelative("sectionId").stringValue = before[i].Id;
                    entry.FindPropertyRelative("position").vector2Value = before[i].Position;
                }
                initial.ApplyModifiedPropertiesWithoutUndo();
            }
            Undo.RecordObjects(new UnityEngine.Object[] { container, layout }, label);
            using var layoutData = new SerializedObject(layout); edit(data, layoutData);
            data.ApplyModifiedPropertiesWithoutUndo(); layoutData.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(container); EditorUtility.SetDirty(layout);
            Undo.FlushUndoRecordObjects(); Undo.CollapseUndoOperations(group);
        }

        private static SerializedProperty FindPosition(SerializedObject layout, string id)
        {
            var entries = layout.FindProperty("sections");
            for (int i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                if (entry.FindPropertyRelative("sectionId").stringValue == id) return entry;
            }
            return null;
        }
        private static SerializedProperty AppendPosition(SerializedObject layout)
        { var entries = layout.FindProperty("sections"); int index = entries.arraySize++; return entries.GetArrayElementAtIndex(index); }
    }
}
