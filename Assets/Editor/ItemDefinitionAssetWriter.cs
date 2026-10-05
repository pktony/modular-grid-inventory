using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class ItemDefinitionAssetWriter
    {
        public static ItemDefinition Write(string id, string title, ItemCategoryDefinition category, int width, int height,
            int maxStack, Sprite icon, ContainerDefinition container)
        {
            var asset = InventoryAssetStore.GetOrCreate<ItemDefinition>("item-" + id); var data = new SerializedObject(asset);
            InventorySerializedFields.String(data, "identifier", id); InventorySerializedFields.String(data, "title", title);
            data.FindProperty("category").objectReferenceValue = category;
            data.FindProperty("width").intValue = width; data.FindProperty("height").intValue = height;
            data.FindProperty("maxStack").intValue = maxStack; data.FindProperty("icon").objectReferenceValue = icon;
            data.FindProperty("container").objectReferenceValue = container; data.ApplyModifiedPropertiesWithoutUndo(); return asset;
        }
    }
}
