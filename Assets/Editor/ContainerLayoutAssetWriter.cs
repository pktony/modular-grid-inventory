using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class ContainerLayoutAssetWriter
    {
        public static ContainerLayoutDefinition Write(string id, (string id, int width, int height, float x, float y)[] sections)
        {
            var layout = InventoryAssetStore.GetOrCreate<ContainerLayoutDefinition>("layout-" + id); var data = new SerializedObject(layout);
            var positions = data.FindProperty("sections"); positions.arraySize = sections.Length;
            for (int i = 0; i < sections.Length; i++)
            {
                var pos = positions.GetArrayElementAtIndex(i); pos.FindPropertyRelative("sectionId").stringValue = sections[i].id;
                pos.FindPropertyRelative("position").vector2Value = new Vector2(sections[i].x, sections[i].y);
            }
            data.ApplyModifiedPropertiesWithoutUndo(); return layout;
        }
    }
}
