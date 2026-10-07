using UnityEngine;
namespace Pktony.GridInventory
{
    [CreateAssetMenu(menuName = "Modular Grid Inventory/Category")]
    public sealed class ItemCategoryDefinition : ScriptableObject
    {
        [SerializeField] private string identifier;
        [SerializeField] private string title;
        [SerializeField] private ItemCategoryDefinition parent;
        internal string Id => identifier;
        internal CategoryDefinitionView Freeze() => new(identifier, title, parent != null ? parent.Id : null);
    }
}
