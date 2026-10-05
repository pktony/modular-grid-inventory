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
            InventoryElementFactory.Label("Heading", root, new Vector2(24, -16), new Vector2(420, 35), "TACTICAL / INVENTORY", 26, InventoryPalette.Text);
            InventoryElementFactory.Label("Controls", root, new Vector2(24, -56), new Vector2(900, 22), "DRAG to move   /   R rotate   /   ESC cancel   /   Double-click bags   /   Right-click actions", 14, InventoryPalette.Muted);
            var bindings = new InventoryScreenBindings { Root = root, Events = events,
                Stash = panels.Create(root, "STASH", new Vector2(24, -130), events), Bag = panels.Create(root, "CONTAINER", new Vector2(648, -130), events),
                Open = InventoryElementFactory.Button("Open", root, new Vector2(676, -20), new Vector2(104, 34), "OPEN"),
                Split = InventoryElementFactory.Button("Split", root, new Vector2(788, -20), new Vector2(104, 34), "SPLIT"),
                Delete = InventoryElementFactory.Button("Delete", root, new Vector2(900, -20), new Vector2(104, 34), "DELETE"),
                Reset = InventoryElementFactory.Button("Reset", root, new Vector2(1012, -20), new Vector2(104, 34), "RESET"),
                Back = InventoryElementFactory.Button("Back", root, new Vector2(648, -90), new Vector2(60, 32), "UP"),
                Close = InventoryElementFactory.Button("Close", root, new Vector2(1196, -90), new Vector2(60, 32), "X"),
                Breadcrumb = InventoryElementFactory.Rect("Breadcrumb", root, new Vector2(716, -90), new Vector2(472, 32)),
                EmptyBag = InventoryElementFactory.Label("EmptyBag", root, new Vector2(672, -300), new Vector2(550, 90), "OPEN A CONTAINER\nDouble-click a backpack, case or rig.", 22, InventoryPalette.Muted),
                Status = InventoryElementFactory.Label("Status", root, new Vector2(24, -679), new Vector2(1232, 30), "Ready", 16, InventoryPalette.Accent),
                Inspector = new InventoryInspectorPresenter(InventoryElementFactory.Label("Inspector", root, new Vector2(24, -636), new Vector2(1232, 38), "", 14, InventoryPalette.Text)) };
            var ghost = InventoryElementFactory.Panel("DragGhost", root, Vector2.zero, Vector2.one, InventoryPalette.Valid).rectTransform;
            var icon = InventoryElementFactory.Panel("Icon", ghost, Vector2.zero, Vector2.one, Color.white);
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            var count = InventoryElementFactory.Label("Quantity", ghost, Vector2.zero, new Vector2(50, 24), "", 18, Color.white);
            bindings.Drag = new InventoryDragPresenter(root, ghost, ghost.GetComponent<Image>(), icon, count);
            var menu = InventoryElementFactory.Panel("ContextMenu", root, Vector2.zero, new Vector2(180, 144), InventoryPalette.Panel, true).rectTransform;
            bindings.Context = new InventoryContextMenu(menu, root,
                InventoryElementFactory.Button("OpenBag", menu, new Vector2(4, -4), new Vector2(172, 42), "OPEN CONTAINER"),
                InventoryElementFactory.Button("SplitStack", menu, new Vector2(4, -51), new Vector2(172, 42), "SPLIT STACK"),
                InventoryElementFactory.Button("DeleteItem", menu, new Vector2(4, -98), new Vector2(172, 42), "DELETE"));
            bindings.Quantity = CreateQuantity(root);
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
