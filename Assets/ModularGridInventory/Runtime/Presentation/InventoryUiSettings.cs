using System;
using TMPro;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryUiSettings
    {
        public TMP_FontAsset Font { get; }
        public InventoryPaletteView Palette { get; }
        public float CellPitch { get; }
        public float CellGap { get; }
        public string Heading { get; }
        public string RootTitle { get; }
        public bool ShowClickIndicator { get; }
        public Vector2 ReferenceSize => new(1280, 720);
        public InventoryUiSettings(InventoryUiTheme source)
        {
            if (source == null || source.Font == null) throw new ArgumentException("A UI theme and font are required.", nameof(source));
            if (!float.IsFinite(source.CellPitch) || source.CellPitch < 32 || source.CellPitch > 64
                || !float.IsFinite(source.CellGap) || source.CellGap < 1 || source.CellGap > 6)
                throw new ArgumentException("Cell pitch must be 32–64 and gap must be 1–6.", nameof(source));
            if (source.Palette == null) throw new ArgumentException("A palette is required.", nameof(source));
            Font = source.Font; Palette = new InventoryPaletteView(source.Palette);
            CellPitch = source.CellPitch; CellGap = source.CellGap; Heading = source.Heading;
            RootTitle = source.RootTitle; ShowClickIndicator = source.ShowClickIndicator;
        }
    }
}
