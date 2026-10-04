using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem.Editor
{
    public sealed class InventoryWalkthroughScenario
    {
        private readonly Inventory inventory;
        private readonly InventoryGridGeometry geometry;
        private ItemData item;
        public InventoryWalkthroughScenario(Inventory inventory)
        {
            this.inventory = inventory;
            geometry = new InventoryGridGeometry(GameObject.Find("Grid").GetComponent<RectTransform>(), 80);
        }
        private PointerEventData Pointer(int x, int y)
        {
            var p = RectTransformUtility.WorldToScreenPoint(null, geometry.WorldPosition(x, y) + new Vector3(8, -8));
            return new PointerEventData(EventSystem.current) { position = p, pressPosition = p };
        }
        private void Begin(int x, int y)
        {
            item = inventory.Model.GetItemAt(x, y);
            inventory.View.PointerEvents.Select(item, Pointer(x, y));
            inventory.View.PointerEvents.Begin(item, Pointer(x, y));
        }
        private void Move(int x, int y) => inventory.View.PointerEvents.Drag(Pointer(x, y));
        private void End(int x, int y) => inventory.View.PointerEvents.End(Pointer(x, y));
        private void Reset() => GameObject.Find("Reset").GetComponent<Button>().onClick.Invoke();
        public void Apply(int stage)
        {
            switch (stage)
            {
                case 1: item = inventory.Model.GetItemAt(0, 0); inventory.View.PointerEvents.Select(item, Pointer(0, 0)); break;
                case 2: Begin(0, 0); Move(0, 3); break;
                case 3: End(0, 3); break;
                case 4: Reset(); break;
                case 5: Begin(8, 1); break;
                case 6: inventory.Interaction.Rotate(); Move(2, 3); break;
                case 7: End(2, 3); break;
                case 8: Begin(2, 3); Move(5, 2); break;
                case 9: End(5, 2); break;
                case 10: Begin(2, 3); Move(6, 3); break;
                case 11: inventory.Interaction.Cancel(); break;
                case 12: inventory.View.PointerEvents.Select(inventory.Model.GetItemAt(7, 2), Pointer(7, 2)); inventory.Interaction.RemoveSelected(); break;
                case 13: GameObject.Find("AddItem").GetComponent<Button>().onClick.Invoke(); break;
                case 14: Reset(); break;
            }
            string[] captions = {"01 / Different sizes. One grid. 7 items, 180 cells.", "02 / Select an item to inspect its size and orientation.",
                "03 / Drag to a valid area. Green cells preview the placement.", "04 / Release to commit. The original cells are now free.",
                "05 / Reset restores the starting layout.", "06 / Pick up a vertical pistol.", "07 / R rotates the preview while keeping the grabbed cell.",
                "08 / Release to commit the rotated placement.", "09 / Overlap is rejected. Red cells show the blocked area.",
                "10 / Rejected drop preserves the original placement.", "11 / Move again. The model remains unchanged during preview.",
                "12 / Esc cancels the preview and restores the original item.", "13 / Delete removes the selected item and releases its cells.",
                "14 / Add creates an independent instance in the next available space.", "15 / Reset. Ready for the next round."};
            inventory.View.SetStatus(captions[stage]);
        }
    }
}
