using UnityEditor;
namespace Pktony.GridInventory.Editor
{
    public static class CategoryAssetWriter
    {
        public static void Write(ItemCategoryDefinition asset, string id, string title, ItemCategoryDefinition parent)
        {
            var data = new SerializedObject(asset);
            InventorySerializedFields.String(data, "identifier", id); InventorySerializedFields.String(data, "title", title);
            data.FindProperty("parent").objectReferenceValue = parent; data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
