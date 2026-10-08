using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    [System.Serializable]
    public sealed class InventoryPalette
    {
        public Color Background = new(0.025f, 0.027f, 0.031f);
        public Color Panel = new(0.045f, 0.049f, 0.055f);
        public Color Cell = new(0.105f, 0.114f, 0.12f);
        public Color Item = new(0.15f, 0.17f, 0.17f);
        public Color Text = new(0.91f, 0.94f, 0.96f);
        public Color Muted = new(0.68f, 0.72f, 0.67f);
        public Color Accent = new(0.37f, 0.86f, 0.82f);
        public Color Border = new(0.3f, 0.32f, 0.33f);
        public Color Title = new(0.075f, 0.079f, 0.085f);
        public Color ActiveTitle = new(0.08f, 0.20f, 0.21f);
        public Color Close = new(0.3f, 0.065f, 0.05f);
        public Color ErrorText = new(1, 0.62f, 0.52f);
        public Color Valid = new(0.18f, 0.43f, 0.25f, 0.9f);
        public Color Invalid = new(0.68f, 0.23f, 0.19f, 0.9f);
    }
}
