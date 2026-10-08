using System;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Expanded Catalog")]
    public sealed class InventoryCatalogAsset : ScriptableObject
    {
        [SerializeField] private ItemCategoryDefinition[] categories = Array.Empty<ItemCategoryDefinition>();
        [SerializeField] private ItemDefinition[] items = Array.Empty<ItemDefinition>();
        internal ItemCategoryDefinition[] Categories => categories;
        internal ItemDefinition[] Items => items;
    }
}
