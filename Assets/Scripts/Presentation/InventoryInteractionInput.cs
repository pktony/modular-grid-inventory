using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventoryInteractionInput
    {
        private readonly InventoryInteractionController controller;
        private readonly StackQuantityDialog quantity;
        private readonly ClickIndicator indicator;
        public InventoryInteractionInput(InventoryInteractionController controller, StackQuantityDialog quantity, ClickIndicator indicator)
        { this.controller = controller; this.quantity = quantity; this.indicator = indicator; }
        public void Tick()
        {
            controller.UpdatePointer(Input.mousePosition);
            indicator.Show(Input.mousePosition, Input.GetMouseButton(0) || Input.GetMouseButton(1));
            if (Input.GetKeyDown(KeyCode.Escape)) controller.Escape();
            if (quantity.IsOpen) { if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) quantity.Confirm(); return; }
            if (Input.GetKeyDown(KeyCode.R)) controller.Rotate();
            if (Input.GetKeyDown(KeyCode.Delete)) controller.DeleteSelected();
        }
    }
}
