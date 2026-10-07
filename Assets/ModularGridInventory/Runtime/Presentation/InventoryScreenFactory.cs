using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryScreenFactory
    {
        private readonly InventoryElementFactory elements;
        private readonly InventoryPanelFactory panels;
        public InventoryScreenFactory(InventoryPanelFactory panels, InventoryElementFactory elements) { this.panels = panels; this.elements = elements; }
        public InventoryScreenBindings Create(Transform parent)
        {
            var root = elements.Panel("InventoryScreen", parent, Vector2.zero, new Vector2(1280, 720), elements.Theme.Palette.Background).rectTransform;
            var events = new InventoryInputEvents();
            elements.Label("Heading", root, new Vector2(24, -18), new Vector2(500, 32), elements.Theme.Heading, 24, elements.Theme.Palette.Text);
            elements.Label("Session", root, new Vector2(995, -24), new Vector2(260, 22), "LOCAL INVENTORY SESSION", 12, elements.Theme.Palette.Muted);
            elements.Panel("HeaderLine", root, new Vector2(24, -64), new Vector2(1232, 1), elements.Theme.Palette.Border);
            elements.Label("ContainersLabel", root, new Vector2(24, -80), new Vector2(320, 22), "CONTAINERS", 16, elements.Theme.Palette.Text);
            elements.Label("Controls", root, new Vector2(24, -642), new Vector2(730, 18), "DRAG   |   R ROTATE   |   ESC CANCEL / CLOSE   |   DOUBLE-CLICK OPEN", 11, elements.Theme.Palette.Muted);
            var bindings = new InventoryScreenBindings { Root = root, Events = events, Theme = elements.Theme,
                Stash = panels.Create(root, elements.Theme.RootTitle, new Vector2(782, -80), events),
                Open = elements.Button("Open", root, new Vector2(24, -602), new Vector2(110, 28), "OPEN"),
                Split = elements.Button("Split", root, new Vector2(142, -602), new Vector2(110, 28), "SPLIT"),
                Delete = elements.Button("Delete", root, new Vector2(260, -602), new Vector2(110, 28), "DELETE"),
                Reset = elements.Button("Reset", root, new Vector2(650, -602), new Vector2(104, 28), "RESET"),
                EmptyWindows = elements.Label("EmptyWindows", root, new Vector2(60, -260), new Vector2(650, 100), "OPEN A CONTAINER\nDouble-click a bag, case or rig.\nDrag its title bar to arrange your workspace.", 18, elements.Theme.Palette.Muted),
                Status = elements.Label("Status", root, new Vector2(24, -688), new Vector2(1232, 25), "Ready", 14, elements.Theme.Palette.Accent),
                Inspector = new InventoryInspectorPresenter(elements.Label("Inspector", root, new Vector2(24, -662), new Vector2(1232, 24), "", 12, elements.Theme.Palette.Text)) };
            elements.Panel("FooterLine", root, new Vector2(24, -656), new Vector2(1232, 1), elements.Theme.Palette.Border);
            bindings.WindowLayer = elements.Rect("ContainerWindows", root, Vector2.zero, new Vector2(1280, 720));
            bindings.Overlay = elements.Rect("InteractionOverlay", root, Vector2.zero, new Vector2(1280, 720));
            var overlay = bindings.Overlay;
            var ghost = elements.Panel("DragGhost", overlay, Vector2.zero, Vector2.one, elements.Theme.Palette.Valid).rectTransform;
            var icon = elements.Panel("Icon", ghost, Vector2.zero, Vector2.one, Color.white);
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            var count = elements.Label("Quantity", ghost, Vector2.zero, new Vector2(50, 24), "", 18, Color.white);
            bindings.Drag = new InventoryDragPresenter(overlay, ghost, ghost.GetComponent<Image>(), icon, count,
                elements.Panel("ContainerDropHighlight", overlay, Vector2.zero, Vector2.one, elements.Theme.Palette.Valid), elements.Theme);
            var menu = elements.Panel("ContextMenu", overlay, Vector2.zero, new Vector2(180, 144), elements.Theme.Palette.Panel, true).rectTransform;
            bindings.Context = new InventoryContextMenu(menu, overlay,
                elements.Button("OpenBag", menu, new Vector2(4, -4), new Vector2(172, 42), "OPEN CONTAINER"),
                elements.Button("SplitStack", menu, new Vector2(4, -51), new Vector2(172, 42), "SPLIT STACK"),
                elements.Button("DeleteItem", menu, new Vector2(4, -98), new Vector2(172, 42), "DELETE"));
            bindings.Quantity = CreateQuantity(overlay);
            return bindings;
        }
        private StackQuantityDialog CreateQuantity(RectTransform root)
        {
            var modal = elements.Panel("QuantityDialog", root, Vector2.zero, new Vector2(1280, 720), new Color(0, 0, 0, 0.8f), true).rectTransform;
            var panel = elements.Panel("Dialog", modal, new Vector2(450, -230), new Vector2(380, 230), elements.Theme.Palette.Panel, true).rectTransform;
            var prompt = elements.Label("Prompt", panel, new Vector2(20, -20), new Vector2(340, 42), "", 20, elements.Theme.Palette.Text);
            var box = elements.Panel("Input", panel, new Vector2(20, -80), new Vector2(340, 44), elements.Theme.Palette.Cell, true).rectTransform;
            var input = box.gameObject.AddComponent<TMP_InputField>();
            var text = elements.Label("Value", box, new Vector2(10, -8), new Vector2(320, 34), "", 22, elements.Theme.Palette.Text);
            input.textViewport = box; input.textComponent = text; input.targetGraphic = box.GetComponent<Image>();
            input.contentType = TMP_InputField.ContentType.IntegerNumber; input.characterLimit = 6;
            return new StackQuantityDialog(modal, input, prompt,
                elements.Button("Confirm", panel, new Vector2(20, -165), new Vector2(160, 44), "PICK UP SPLIT"),
                elements.Button("Cancel", panel, new Vector2(200, -165), new Vector2(160, 44), "CANCEL"));
        }
    }
}
