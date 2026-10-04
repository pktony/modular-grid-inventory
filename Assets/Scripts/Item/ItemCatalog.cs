using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Item Catalog")]
    public sealed class ItemCatalog : ScriptableObject
    {
        [SerializeField] private ItemDefinition[] definitions;
        internal System.Collections.Generic.IReadOnlyList<ItemDefinition> Definitions => definitions;
    }
}
