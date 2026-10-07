using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryPaletteView
    {
        public Color Background { get; }
        public Color Panel { get; }
        public Color Cell { get; }
        public Color Item { get; }
        public Color Text { get; }
        public Color Muted { get; }
        public Color Accent { get; }
        public Color Border { get; }
        public Color Title { get; }
        public Color ActiveTitle { get; }
        public Color Close { get; }
        public Color ErrorText { get; }
        public Color Valid { get; }
        public Color Invalid { get; }
        public InventoryPaletteView(InventoryPalette source)
        {
            Background = source.Background;
            Panel = source.Panel;
            Cell = source.Cell;
            Item = source.Item;
            Text = source.Text;
            Muted = source.Muted;
            Accent = source.Accent;
            Border = source.Border;
            Title = source.Title;
            ActiveTitle = source.ActiveTitle;
            Close = source.Close;
            ErrorText = source.ErrorText;
            Valid = source.Valid;
            Invalid = source.Invalid;
        }
    }
}
