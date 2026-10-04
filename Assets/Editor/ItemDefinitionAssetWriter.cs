using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class ItemDefinitionAssetWriter
    {
        public static void Write(ItemDefinition definition, string id, string title, int width, int height, Sprite icon, int maxStack = 1)
        {
            var data = new SerializedObject(definition);
            data.FindProperty("identifier").stringValue = id;
            data.FindProperty("displayName").stringValue = title;
            data.FindProperty("width").intValue = width;
            data.FindProperty("height").intValue = height;
            data.FindProperty("maxStack").intValue = maxStack;
            data.FindProperty("icon").objectReferenceValue = icon;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
