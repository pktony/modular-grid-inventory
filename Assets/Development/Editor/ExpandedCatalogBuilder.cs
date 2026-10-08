using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class ExpandedCatalogBuilder
    {
        private const string Folder = "Assets/ModularGridInventory/Samples/Catalog";
        [MenuItem("Inventory/Samples/Restore Catalog Defaults...")]
        private static void RestoreDefaults()
        {
            if (EditorUtility.DisplayDialog("Restore sample catalog", "Replace item, container and layout edits with the sample defaults?", "Restore", "Cancel")) Build();
        }
        public static void Build()
        {
            Directory.CreateDirectory(Folder); AssetDatabase.Refresh();
            var categories = new Dictionary<string, ItemCategoryDefinition>();
            foreach (var id in new[] { "Weapon", "Weapon/Carbine", "Consumable", "Consumable/Medical", "Consumable/Medical/Medkit",
                "Consumable/Food", "Consumable/Drink", "Ammo", "Ammo/Light", "Ammo/Heavy", "WeaponPart", "WeaponPart/Foregrip",
                "Container", "Container/Backpack", "Container/Case", "Container/Rig" })
            {
                var asset = InventoryAssetStore.GetOrCreate<ItemCategoryDefinition>("category-" + id.Replace('/', '-').Replace('.', '-'));
                int slash = id.LastIndexOf('/');
                CategoryAssetWriter.Write(asset, id, id.Split('/').Last(), slash < 0 ? null : categories[id.Substring(0, slash)]);
                categories.Add(id, asset);
            }
            var mbss = Container("pack-small", new[] { ("main", 4, 4, 0f, 0f) });
            var berkut = Container("pack-large", new[] { ("main", 4, 5, 0f, 0f) });
            var ammo = Container("case-ammo", new[] { ("main", 7, 7, 0f, 0f) }, categories["Ammo"]);
            var medical = Container("case-medical", new[] { ("main", 7, 7, 0f, 0f) }, categories["Consumable/Medical"]);
            var items = new[] {
                Item("pack-small", "Compact Expedition Pack", categories["Container/Backpack"], 4, 4, 1, mbss),
                Item("pack-large", "Large Expedition Pack", categories["Container/Backpack"], 4, 5, 1, berkut),
                Item("case-ammo", "Ammunition Case", categories["Container/Case"], 2, 2, 1, ammo),
                Item("case-medical", "Medical Case", categories["Container/Case"], 3, 3, 1, medical),
                Item("ammo-light", "Light Ammunition", categories["Ammo/Light"], 1, 1, 50),
                Item("ammo-heavy", "Heavy Ammunition", categories["Ammo/Heavy"], 1, 1, 60),
                Item("medical-kit", "Field Medical Kit", categories["Consumable/Medical/Medkit"], 1, 1, 1),
                Item("carbine", "Modular Carbine", categories["Weapon/Carbine"], 4, 2, 1),
                Item("grip", "Rail Grip", categories["WeaponPart/Foregrip"], 1, 1, 1) }
                .Concat(RigCatalogBuilder.Build(categories["Container/Rig"])).ToArray();
            var catalog = InventoryAssetStore.GetOrCreate<InventoryCatalogAsset>("Catalog"); var source = new SerializedObject(catalog);
            InventorySerializedFields.References(source.FindProperty("categories"), categories.Values.Cast<UnityEngine.Object>().ToArray());
            InventorySerializedFields.References(source.FindProperty("items"), items); source.ApplyModifiedPropertiesWithoutUndo();
            new InventoryCatalogSnapshotFactory().Create(catalog);
            AssetDatabase.SaveAssets(); Debug.Log($"Expanded catalog validated: {items.Length} definitions.");
        }
        private static ItemDefinition Item(string id, string title, ItemCategoryDefinition category, int width, int height,
            int maxStack, ContainerDefinition container = null) => ItemDefinitionAssetWriter.Write(id, title, category, width, height,
                maxStack, SpriteImportUtility.Load("Assets/ModularGridInventory/Samples/Icons/Items/" + id + ".png"), container);
        private static ContainerDefinition Container(string id, (string id, int width, int height, float x, float y)[] sections,
            ItemCategoryDefinition allowed = null)
        {
            var layout = ContainerLayoutAssetWriter.Write(id, sections);
            return ContainerDefinitionAssetWriter.Write(id, sections, layout, allowed);
        }
    }
}
