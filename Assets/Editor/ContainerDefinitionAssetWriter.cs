using System;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class ContainerDefinitionAssetWriter
    {
        public static ContainerDefinition Write(string id, (string id, int width, int height, float x, float y)[] sections,
            ContainerLayoutDefinition layout, ItemCategoryDefinition allowed)
        {
            var asset = InventoryAssetStore.GetOrCreate<ContainerDefinition>("container-" + id); var data = new SerializedObject(asset);
            InventorySerializedFields.String(data, "identifier", id); var grids = data.FindProperty("sections"); grids.arraySize = sections.Length;
            for (int i = 0; i < sections.Length; i++)
            {
                var grid = grids.GetArrayElementAtIndex(i); grid.FindPropertyRelative("identifier").stringValue = sections[i].id;
                grid.FindPropertyRelative("width").intValue = sections[i].width; grid.FindPropertyRelative("height").intValue = sections[i].height;
            }
            var policy = data.FindProperty("policy"); policy.FindPropertyRelative("mode").enumValueIndex = allowed == null ? 0 : 1;
            InventorySerializedFields.References(policy.FindPropertyRelative("allowedCategories"),
                allowed == null ? Array.Empty<UnityEngine.Object>() : new UnityEngine.Object[] { allowed });
            data.FindProperty("layout").objectReferenceValue = layout; data.ApplyModifiedPropertiesWithoutUndo(); return asset;
        }
    }
}
