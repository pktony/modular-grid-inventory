namespace InventorySystem
{
    public sealed class InventoryHudPresenter
    {
        private readonly InventoryScreenBindings bindings;
        public InventoryHudPresenter(InventoryScreenBindings bindings) => this.bindings = bindings;
        public void ShowItem(ItemData item)
        {
            if (item == null) { bindings.Detail.text = "Select an item to inspect it.\nDrag any part of an item to move it."; return; }
            var size = item.GetAbsoluteSize();
            bindings.Detail.text = $"{item.Definition.DisplayName}\n<size=16>{size.absWidth} x {size.absHeight} cells\n{item.itemDirection}</size>";
        }
        public void SetCount(IInventoryModel model)
        {
            int occupied = 0;
            foreach (var entry in model.Entries) occupied += entry.Item.width * entry.Item.height;
            bindings.Count.text = $"{model.Count} ITEMS / {occupied} OF {model.capacityWidth * model.capacityHeight} CELLS";
            bindings.Delete.interactable = model.Count > 0;
        }
        public void SetStatus(string message) => bindings.Status.text = message;
    }
}
