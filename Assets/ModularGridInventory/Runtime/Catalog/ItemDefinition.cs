using UnityEngine;
namespace Pktony.GridInventory
{
    [CreateAssetMenu(menuName = "Modular Grid Inventory/Item")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string identifier;
        [SerializeField] private string title;
        [SerializeField] private ItemCategoryDefinition category;
        [SerializeField, Min(1)] private int width = 1;
        [SerializeField, Min(1)] private int height = 1;
        [SerializeField, Min(1)] private int maxStack = 1;
        [SerializeField] private Sprite icon;
        [SerializeField] private ContainerDefinition container;
        public DefinitionId Id => new(identifier);
        internal ItemDefinitionView Freeze(ContainerDefinitionView frozenContainer) => new(new DefinitionId(identifier),
            title, width, height, maxStack, icon, category != null ? category.Id : null, frozenContainer);
        internal ContainerDefinition Container => container;
    }
}
