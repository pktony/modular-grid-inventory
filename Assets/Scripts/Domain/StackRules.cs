using System;
namespace InventorySystem.Domain
{
    public sealed class StackRules
    {
        public int MergeQuantity(ItemInstance source, ItemInstance destination) => source.Id == destination.Id
            || source.Definition.Id != destination.Definition.Id || source.Definition.MaxStack <= 1
            ? 0 : Math.Min(source.Quantity, destination.Definition.MaxStack - destination.Quantity);
        public bool CanSplit(ItemInstance item, int quantity) => item.Definition.MaxStack > 1 && quantity > 0 && quantity < item.Quantity;
    }
}
