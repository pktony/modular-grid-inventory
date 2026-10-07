using TMPro;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    [CreateAssetMenu(menuName = "Modular Grid Inventory/UI Theme")]
    public sealed class InventoryUiTheme : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private InventoryPalette palette = new();
        [SerializeField, Range(32, 64)] private float cellPitch = 50;
        [SerializeField, Range(1, 6)] private float cellGap = 2;
        [SerializeField] private string heading = "MODULAR / INVENTORY";
        [SerializeField] private string rootTitle = "STORAGE";
        [SerializeField] private bool showClickIndicator;
        public TMP_FontAsset Font => font;
        public InventoryPalette Palette => palette;
        public float CellPitch => cellPitch;
        public float CellGap => cellGap;
        public string Heading => heading;
        public string RootTitle => rootTitle;
        public bool ShowClickIndicator => showClickIndicator;
        public Vector2 ReferenceSize => new(1280, 720);
    }
}
