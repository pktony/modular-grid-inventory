using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryScreenBuilder
    {
        public InventoryScreenBindings Build(Transform parent)
        {
            var b = new InventoryScreenBindings();
            b.Root = UIElementFactory.Rect("InventoryScreen", parent, Vector2.zero, Vector2.zero);
            b.Root.anchorMin = Vector2.zero; b.Root.anchorMax = Vector2.one; b.Root.offsetMin = b.Root.offsetMax = Vector2.zero;
            var bg = b.Root.gameObject.AddComponent<Image>(); bg.color = InventoryTheme.Background;
            UIElementFactory.Label("Eyebrow", b.Root, new(44, -28), new(1000, 24), "UNITY / SYSTEM STUDY / 01", 13, InventoryTheme.Muted);
            UIElementFactory.Label("Title", b.Root, new(42, -64), new(1000, 50), "TACTICAL INVENTORY", 38, InventoryTheme.Text);
            UIElementFactory.Label("Subtitle", b.Root, new(44, -120), new(1000, 30), "Grid placement. Precise movement. No wasted space.", 16, InventoryTheme.Muted);
            var left = UIElementFactory.Panel("InspectPanel", b.Root, new(44, -182), new(366, 470), InventoryTheme.Panel);
            UIElementFactory.Label("Label", left.transform, new(24, -20), new(300, 24), "ITEM INSPECTOR", 13, InventoryTheme.Accent);
            b.Detail = UIElementFactory.Label("Detail", left.transform, new(24, -60), new(310, 116), "Select an item to inspect it.\nDrag any part of an item to move it.", 20, InventoryTheme.Text);
            UIElementFactory.Label("Controls", left.transform, new(24, -198), new(310, 115), "DRAG   Move item\nR          Rotate while dragging\nESC      Cancel movement\nDELETE   Remove selected item\nWHEEL   Scroll inventory", 15, InventoryTheme.Muted);
            b.Add = UIElementFactory.Button("AddItem", left.transform, new(24, -330), new(151, 42), "ADD ITEM");
            b.Delete = UIElementFactory.Button("DeleteItem", left.transform, new(191, -330), new(151, 42), "REMOVE");
            b.Reset = UIElementFactory.Button("Reset", left.transform, new(24, -384), new(318, 42), "RESET DEMO");
            var right = UIElementFactory.Panel("StashPanel", b.Root, new(438, -182), new(798, 470), InventoryTheme.Panel);
            UIElementFactory.Label("StashTitle", right.transform, new(24, -20), new(230, 30), "STASH / 9 x 20", 19, InventoryTheme.Text);
            b.Count = UIElementFactory.Label("Count", right.transform, new(370, -22), new(370, 24), "", 14, InventoryTheme.Muted);
            var scroll = UIElementFactory.Panel("Scroll", right.transform, new(24, -64), new(750, 384), InventoryTheme.Background, true);
            b.Scroll = scroll.gameObject.AddComponent<ScrollRect>();
            b.Scroll.horizontal = false; b.Scroll.vertical = true;
            b.Scroll.movementType = ScrollRect.MovementType.Clamped; b.Scroll.scrollSensitivity = 32;
            b.Viewport = UIElementFactory.Rect("Viewport", scroll.transform, Vector2.zero, new(750, 384));
            b.Viewport.gameObject.AddComponent<RectMask2D>();
            b.Grid = UIElementFactory.Rect("Grid", b.Viewport, new(8, -8), new(734, 1600));
            b.ItemLayer = UIElementFactory.Rect("Items", b.Grid, Vector2.zero, Vector2.zero);
            b.Scroll.viewport = b.Viewport; b.Scroll.content = b.Grid;
            b.Status = UIElementFactory.Label("Status", b.Root, new(44, -673), new(1150, 30), "Ready. Try moving and rotating the items.", 15, InventoryTheme.Accent);
            b.Overlay = UIElementFactory.Rect("DragOverlay", b.Root, Vector2.zero, new(1280, 720));
            return b;
        }
    }
}
