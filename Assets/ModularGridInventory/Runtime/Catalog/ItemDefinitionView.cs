using UnityEngine;
namespace Pktony.GridInventory
{
    public sealed class ItemDefinitionView
    {
        public DefinitionId Id { get; }
        public string Identifier => Id.Value;
        public string DisplayName { get; }
        public int Width { get; }
        public int Height { get; }
        public int MaxStack { get; }
        public Sprite Icon { get; }
        public string CategoryId { get; }
        public ContainerDefinitionView Container { get; }
        public ItemDefinitionView(DefinitionId id, string name, int width, int height, int maxStack, Sprite icon)
            : this(id, name, width, height, maxStack, icon, null, null) { }
        public ItemDefinitionView(DefinitionId id, string name, int width, int height, int maxStack, Sprite icon,
            string categoryId, ContainerDefinitionView container)
        { Id = id; DisplayName = name; Width = width; Height = height; MaxStack = maxStack; Icon = icon;
            CategoryId = categoryId; Container = container; }
    }
}
