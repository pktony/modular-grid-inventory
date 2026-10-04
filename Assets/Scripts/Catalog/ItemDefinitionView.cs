using UnityEngine;
namespace InventorySystem
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
        public ItemDefinitionView(DefinitionId id, string name, int width, int height, int maxStack, Sprite icon)
        { Id = id; DisplayName = name; Width = width; Height = height; MaxStack = maxStack; Icon = icon; }
    }
}
