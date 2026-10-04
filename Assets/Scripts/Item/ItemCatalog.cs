using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Item Catalog")]
    public sealed class ItemCatalog : ScriptableObject
    {
        [SerializeField] private ItemDefinition[] definitions;
        public System.Collections.Generic.IReadOnlyList<ItemDefinition> Definitions => definitions;
        public void Configure(ItemDefinition[] items) => definitions = items;
    }
}
