namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryInteractionInput
    {
        private readonly InventoryInteractionController controller;
        private readonly StackQuantityDialog quantity;
        private readonly ClickIndicator indicator;
        private readonly IInventoryInputSource source;
        private readonly bool showIndicator;
        public InventoryInteractionInput(InventoryInteractionController controller, StackQuantityDialog quantity,
            ClickIndicator indicator, IInventoryInputSource source, bool showIndicator)
        { this.controller = controller; this.quantity = quantity; this.indicator = indicator; this.source = source; this.showIndicator = showIndicator; }
        public void Tick()
        {
            var frame = source.Read(); controller.UpdatePointer(frame.Pointer);
            indicator.Show(frame.Pointer, showIndicator && frame.Pressed);
            if (frame.Cancel) controller.Escape();
            if (quantity.IsOpen) { if (frame.Confirm) quantity.Confirm(); return; }
            if (frame.Rotate) controller.Rotate();
            if (frame.Delete) controller.DeleteSelected();
        }
    }
}
