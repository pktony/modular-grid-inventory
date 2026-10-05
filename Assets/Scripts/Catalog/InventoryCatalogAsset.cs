using System;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Expanded Catalog")]
    public sealed class InventoryCatalogAsset : ScriptableObject
    {
        [SerializeField] private ItemCategoryDefinition[] categories = Array.Empty<ItemCategoryDefinition>();
        [SerializeField] private InventoryItemDefinition[] items = Array.Empty<InventoryItemDefinition>();
        internal ItemCategoryDefinition[] Categories => categories;
        internal InventoryItemDefinition[] Items => items;
    }
}
