using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class InventoryScreenFactory
    {
        private readonly InventoryPanelFactory panels;
        public InventoryScreenFactory(InventoryPanelFactory panels) { this.panels = panels; }
        public InventoryScreenBindings Create(Transform parent)
        {
            var root = InventoryElementFactory.Panel("InventoryScreen", parent, Vector2.zero, new Vector2(1280, 720), InventoryPalette.Background).rectTransform;
            var events = new InventoryInputEvents();
            InventoryElementFactory.Label("Heading", root, new Vector2(24, -18), new Vector2(500, 32), "CHARACTER / INVENTORY", 24, InventoryPalette.Text);
            InventoryElementFactory.Label("Session", root, new Vector2(995, -24), new Vector2(260, 22), "LOCAL INVENTORY SESSION", 12, InventoryPalette.Muted);
            InventoryElementFactory.Panel("HeaderLine", root, new Vector2(24, -64), new Vector2(1232, 1), InventoryPalette.Border);
            InventoryElementFactory.Label("ContainersLabel", root, new Vector2(24, -80), new Vector2(320, 22), "CONTAINERS", 16, InventoryPalette.Text);
            InventoryElementFactory.Label("Controls", root, new Vector2(24, -642), new Vector2(730, 18), "DRAG   |   R ROTATE   |   ESC CANCEL / CLOSE   |   DOUBLE-CLICK OPEN", 11, InventoryPalette.Muted);
            var bindings = new InventoryScreenBindings { Root = root, Events = events,
                Stash = panels.Create(root, "STASH", new Vector2(782, -80), events),
                Open = InventoryElementFactory.Button("Open", root, new Vector2(24, -602), new Vector2(110, 28), "OPEN"),
                Split = InventoryElementFactory.Button("Split", root, new Vector2(142, -602), new Vector2(110, 28), "SPLIT"),
                Delete = InventoryElementFactory.Button("Delete", root, new Vector2(260, -602), new Vector2(110, 28), "DELETE"),
                Reset = InventoryElementFactory.Button("Reset", root, new Vector2(650, -602), new Vector2(104, 28), "RESET"),
                EmptyWindows = InventoryElementFactory.Label("EmptyWindows", root, new Vector2(60, -260), new Vector2(650, 100), "OPEN A CONTAINER\nDouble-click a bag, case or rig.\nDrag its title bar to arrange your workspace.", 18, InventoryPalette.Muted),
                Status = InventoryElementFactory.Label("Status", root, new Vector2(24, -688), new Vector2(1232, 25), "Ready", 14, InventoryPalette.Accent),
                Inspector = new InventoryInspectorPresenter(InventoryElementFactory.Label("Inspector", root, new Vector2(24, -662), new Vector2(1232, 24), "", 12, InventoryPalette.Text)) };
            InventoryElementFactory.Panel("FooterLine", root, new Vector2(24, -656), new Vector2(1232, 1), InventoryPalette.Border);
            bindings.WindowLayer = InventoryElementFactory.Rect("ContainerWindows", root, Vector2.zero, new Vector2(1280, 720));
            bindings.Overlay = InventoryElementFactory.Rect("InteractionOverlay", root, Vector2.zero, new Vector2(1280, 720));
            var overlay = bindings.Overlay;
            var ghost = InventoryElementFactory.Panel("DragGhost", overlay, Vector2.zero, Vector2.one, InventoryPalette.Valid).rectTransform;
            var icon = InventoryElementFactory.Panel("Icon", ghost, Vector2.zero, Vector2.one, Color.white);
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            var count = InventoryElementFactory.Label("Quantity", ghost, Vector2.zero, new Vector2(50, 24), "", 18, Color.white);
            bindings.Drag = new InventoryDragPresenter(overlay, ghost, ghost.GetComponent<Image>(), icon, count,
                InventoryElementFactory.Panel("ContainerDropHighlight", overlay, Vector2.zero, Vector2.one, InventoryPalette.Valid));
            var menu = InventoryElementFactory.Panel("ContextMenu", overlay, Vector2.zero, new Vector2(180, 144), InventoryPalette.Panel, true).rectTransform;
            bindings.Context = new InventoryContextMenu(menu, overlay,
                InventoryElementFactory.Button("OpenBag", menu, new Vector2(4, -4), new Vector2(172, 42), "OPEN CONTAINER"),
                InventoryElementFactory.Button("SplitStack", menu, new Vector2(4, -51), new Vector2(172, 42), "SPLIT STACK"),
                InventoryElementFactory.Button("DeleteItem", menu, new Vector2(4, -98), new Vector2(172, 42), "DELETE"));
            bindings.Quantity = CreateQuantity(overlay);
            return bindings;
        }
        private static StackQuantityDialog CreateQuantity(RectTransform root)
        {
            var modal = InventoryElementFactory.Panel("QuantityDialog", root, Vector2.zero, new Vector2(1280, 720), new Color(0, 0, 0, 0.8f), true).rectTransform;
            var panel = InventoryElementFactory.Panel("Dialog", modal, new Vector2(450, -230), new Vector2(380, 230), InventoryPalette.Panel, true).rectTransform;
            var prompt = InventoryElementFactory.Label("Prompt", panel, new Vector2(20, -20), new Vector2(340, 42), "", 20, InventoryPalette.Text);
            var box = InventoryElementFactory.Panel("Input", panel, new Vector2(20, -80), new Vector2(340, 44), InventoryPalette.Cell, true).rectTransform;
            var input = box.gameObject.AddComponent<TMP_InputField>();
            var text = InventoryElementFactory.Label("Value", box, new Vector2(10, -8), new Vector2(320, 34), "", 22, InventoryPalette.Text);
            input.textViewport = box; input.textComponent = text; input.targetGraphic = box.GetComponent<Image>();
            input.contentType = TMP_InputField.ContentType.IntegerNumber; input.characterLimit = 6;
            return new StackQuantityDialog(modal, input, prompt,
                InventoryElementFactory.Button("Confirm", panel, new Vector2(20, -165), new Vector2(160, 44), "PICK UP SPLIT"),
                InventoryElementFactory.Button("Cancel", panel, new Vector2(200, -165), new Vector2(160, 44), "CANCEL"));
        }
    }
}
