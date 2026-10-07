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
        [SerializeField] private Sprite icon;
        public string Identifier => identifier;
        public string DisplayName => displayName;
        public int Width => width;
        public int Height => height;
        public Sprite Icon => icon;
        public void Configure(string id, string title, int w, int h, Sprite sprite)
        { identifier = id; displayName = title; width = w; height = h; icon = sprite; }
    }
}
