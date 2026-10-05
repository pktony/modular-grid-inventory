using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class ExpandedCatalogBuilder
    {
        private const string Folder = "Assets/Items/Expansion";
        [MenuItem("Inventory/Build Expanded Catalog")]
        public static void Build()
        {
            Directory.CreateDirectory(Folder); AssetDatabase.Refresh();
            var categories = new Dictionary<string, ItemCategoryDefinition>();
            foreach (var id in new[] { "Weapon", "Weapon/AssaultRifle", "Consumable", "Consumable/Medical", "Consumable/Medical/Medkit",
                "Consumable/Food", "Consumable/Drink", "Ammo", "Ammo/9x19", "Ammo/5.45x39", "WeaponPart", "WeaponPart/Foregrip",
                "Container", "Container/Backpack", "Container/Case", "Container/Rig" })
            {
                var asset = Asset<ItemCategoryDefinition>("category-" + id.Replace('/', '-').Replace('.', '-'));
                var data = new SerializedObject(asset); Set(data, "identifier", id); Set(data, "title", id.Split('/').Last());
                int slash = id.LastIndexOf('/');
                data.FindProperty("parent").objectReferenceValue = slash < 0 ? null : categories[id.Substring(0, slash)];
                data.ApplyModifiedPropertiesWithoutUndo(); categories.Add(id, asset);
            }
            var mbss = Container("mbss", new[] { ("main", 4, 4, 0f, 0f) });
            var berkut = Container("berkut", new[] { ("main", 4, 5, 0f, 0f) });
            var ammo = Container("ammo-case", new[] { ("main", 7, 7, 0f, 0f) }, categories["Ammo"]);
            var medical = Container("medicine-case", new[] { ("main", 7, 7, 0f, 0f) }, categories["Consumable/Medical"]);
            var rig = Container("rig", new[] { ("tall-a", 1, 2, 0f, 0f), ("tall-b", 1, 2, 2f, 0f), ("tall-c", 1, 2, 4f, 0f), ("tall-d", 1, 2, 6f, 0f),
                ("large-a", 2, 2, 0f, 3f), ("large-b", 2, 2, 3f, 3f), ("small-a", 1, 1, 0f, 6f), ("small-b", 1, 1, 2f, 6f),
                ("small-c", 1, 1, 4f, 6f), ("small-d", 1, 1, 6f, 6f) });
            var items = new[] {
                Item("mbss", "Flyye MBSS", categories["Container/Backpack"], 4, 4, 1, mbss),
                Item("berkut", "WARTECH Berkut", categories["Container/Backpack"], 4, 5, 1, berkut),
                Item("ammo-case", "Ammunition case", categories["Container/Case"], 2, 2, 1, ammo),
                Item("medicine-case", "Medicine case", categories["Container/Case"], 3, 3, 1, medical),
                Item("pst", "9x19 Pst gzh", categories["Ammo/9x19"], 1, 1, 50),
                Item("ps", "5.45x39 PS gs", categories["Ammo/5.45x39"], 1, 1, 60),
                Item("ai2", "AI-2 medkit", categories["Consumable/Medical/Medkit"], 1, 1, 1),
                Item("aks74u", "AKS-74U", categories["Weapon/AssaultRifle"], 4, 2, 1),
                Item("rk0", "Zenit RK-0", categories["WeaponPart/Foregrip"], 1, 1, 1),
                Item("rig", "Split rig / BlackRock study", categories["Container/Rig"], 3, 4, 1, rig) };
            var catalog = Asset<InventoryCatalogAsset>("Catalog"); var source = new SerializedObject(catalog);
            WriteReferences(source.FindProperty("categories"), categories.Values.Cast<UnityEngine.Object>().ToArray());
            WriteReferences(source.FindProperty("items"), items); source.ApplyModifiedPropertiesWithoutUndo();
            new InventoryCatalogSnapshotFactory().Create(catalog);
            AssetDatabase.SaveAssets(); Debug.Log("Expanded catalog validated: 10 definitions.");
        }
        private static InventoryItemDefinition Item(string id, string title, ItemCategoryDefinition category, int width, int height,
            int maxStack, ContainerDefinition container = null)
        {
            string path = "Assets/Resources/ExpansionIcons/" + id + ".png";
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("Missing generated icon: " + path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.maxTextureSize = 512; importer.mipmapEnabled = false;
            importer.spritePixelsPerUnit = 100; importer.SaveAndReimport();
            var asset = Asset<InventoryItemDefinition>("item-" + id); var data = new SerializedObject(asset);
            Set(data, "identifier", id); Set(data, "title", title);
            data.FindProperty("category").objectReferenceValue = category;
            data.FindProperty("width").intValue = width; data.FindProperty("height").intValue = height;
            data.FindProperty("maxStack").intValue = maxStack;
            data.FindProperty("icon").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            data.FindProperty("container").objectReferenceValue = container; data.ApplyModifiedPropertiesWithoutUndo(); return asset;
        }
        private static ContainerDefinition Container(string id, (string id, int width, int height, float x, float y)[] sections,
            ItemCategoryDefinition allowed = null)
        {
            var layout = Asset<ContainerLayoutDefinition>("layout-" + id); var layoutData = new SerializedObject(layout);
            var positions = layoutData.FindProperty("sections"); positions.arraySize = sections.Length;
            var asset = Asset<ContainerDefinition>("container-" + id); var data = new SerializedObject(asset);
            Set(data, "identifier", id); var grids = data.FindProperty("sections"); grids.arraySize = sections.Length;
            for (int i = 0; i < sections.Length; i++)
            {
                var entry = sections[i]; var grid = grids.GetArrayElementAtIndex(i); var pos = positions.GetArrayElementAtIndex(i);
                grid.FindPropertyRelative("identifier").stringValue = entry.id;
                grid.FindPropertyRelative("width").intValue = entry.width; grid.FindPropertyRelative("height").intValue = entry.height;
                pos.FindPropertyRelative("sectionId").stringValue = entry.id; pos.FindPropertyRelative("position").vector2Value = new Vector2(entry.x, entry.y);
            }
            var policy = data.FindProperty("policy"); policy.FindPropertyRelative("mode").enumValueIndex = allowed == null ? 0 : 1;
            WriteReferences(policy.FindPropertyRelative("allowedCategories"), allowed == null ? Array.Empty<UnityEngine.Object>() : new UnityEngine.Object[] { allowed });
            data.FindProperty("layout").objectReferenceValue = layout; layoutData.ApplyModifiedPropertiesWithoutUndo(); data.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }
        private static T Asset<T>(string name) where T : ScriptableObject
        {
            string path = Folder + "/" + name + ".asset"; var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset;
        }
        private static void Set(SerializedObject data, string property, string value) => data.FindProperty(property).stringValue = value;
        private static void WriteReferences(SerializedProperty property, UnityEngine.Object[] references)
        {
            property.arraySize = references.Length;
            for (int i = 0; i < references.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = references[i];
        }
    }
}
