using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string identifier;
        [SerializeField] private string displayName;
        [SerializeField, Min(1)] private int width = 1;
        [SerializeField, Min(1)] private int height = 1;
        [SerializeField, Min(1)] private int maxStack = 1;
        [SerializeField] private Sprite icon;
        internal string Identifier => identifier;
        internal string DisplayName => displayName;
        internal int Width => width;
        internal int Height => height;
        internal int MaxStack => maxStack;
        internal Sprite Icon => icon;
    }
}
