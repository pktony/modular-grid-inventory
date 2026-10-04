using UnityEditor;
namespace InventorySystem.Editor
{
    public static class ItemCatalogAssetWriter
    {
        public static void Write(ItemCatalog catalog, ItemDefinition[] definitions)
        {
            var data = new SerializedObject(catalog);
            var items = data.FindProperty("definitions");
            items.arraySize = definitions.Length;
            for (int i = 0; i < definitions.Length; i++) items.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
