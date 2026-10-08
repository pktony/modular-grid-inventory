using InventorySystem;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class DemoCatalogBuilder
    {
        public static ItemCatalog Build()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Items")) AssetDatabase.CreateFolder("Assets", "Items");
            string[] ids = {"Assault rifle 1", "Assault rifle 2", "Grenade launcher 3", "Assault rifle 3", "9 x 19 mm", "Pistol 1", "Pistol 2"};
            int[] widths = {4,4,5,4,1,2,2}; int[] heights = {2,2,1,2,1,1,1};
            var definitions = new ItemDefinition[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                string path = $"Assets/Items/{ids[i]}.asset";
                var definition = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                if (definition == null) { definition = ScriptableObject.CreateInstance<ItemDefinition>(); AssetDatabase.CreateAsset(definition, path); }
                string category = i == 4 ? "Bullets" : "Weapons";
                var sprite = SpriteImportUtility.Load($"Assets/Resources/Sprites/{category}/{ids[i]}.png");
                ItemDefinitionAssetWriter.Write(definition, ids[i], ids[i], widths[i], heights[i], sprite, i == 4 ? 50 : 1);
                EditorUtility.SetDirty(definition); definitions[i] = definition;
            }
            var catalog = AssetDatabase.LoadAssetAtPath<ItemCatalog>("Assets/Items/DemoCatalog.asset");
            if (catalog == null) { catalog = ScriptableObject.CreateInstance<ItemCatalog>(); AssetDatabase.CreateAsset(catalog, "Assets/Items/DemoCatalog.asset"); }
            ItemCatalogAssetWriter.Write(catalog, definitions); EditorUtility.SetDirty(catalog); AssetDatabase.SaveAssets(); return catalog;
        }
    }
}
