using System;
using System.Linq;
using InventorySystem.Domain;
namespace InventorySystem
{
    public sealed class InventoryScenarioVerifier
    {
        public void Verify(ExpandedInventory inventory, int stage)
        {
            var state = inventory.ReadModel.Snapshot; var root = state.RootContainerId;
            var pst = state.Items.Values.Where(i => i.Definition.Identifier == "pst").ToArray();
            switch (stage)
            {
                case 0: Require(state.Items.Count == 13, "Initial seed count"); break;
                case 1: Require(inventory.Screen.Bag.Container != root && inventory.Screen.Bag.Root.gameObject.activeSelf, "Open backpack"); break;
                case 2: Require(state.Registry.TryGetBag(inventory.Screen.Bag.Container, out var bag) && state.Items[bag].Definition.Identifier == "mbss", "Nested backpack"); break;
                case 3: Require(!inventory.Screen.Bag.Root.gameObject.activeSelf, "Close container"); break;
                case 4: Require(inventory.Screen.Bag.Policy.text.Contains("Ammo"), "Ammo policy label"); break;
                case 5: Require(inventory.Interaction.Drag != null, "Begin stack drag"); break;
                case 6: Require(state.Containers[inventory.Screen.Bag.Container].Entries.Count == 1, "Cross-panel stack placement"); break;
                case 7: Require(state.Containers[root].Entries.Values.Any(e => state.Items[e.ItemId].Definition.Identifier == "aks74u"), "Invalid preview preserves source"); break;
                case 8: Require(inventory.Screen.Bag.Sections.Count == 10, "Rig compartment count"); break;
                case 9: Require(inventory.Interaction.Drag != null, "Rig preview"); break;
                case 10: Require(state.Containers[inventory.Screen.Bag.Container].Entries.Values.Single().SectionId.Value == "small-a", "Rig section placement"); break;
                case 11: Require(pst.Sum(i => i.Quantity) == 60, "Merge preview conserves quantity"); break;
                case 12: Require(pst.Any(i => i.Quantity == 50) && pst.Any(i => i.Quantity == 10) && inventory.Screen.Quantity.IsOpen, "Partial merge and quantity dialog"); break;
                case 13: Require(inventory.Interaction.Drag?.SplitQuantity == 5 && pst.Length == 2, "Split preview has no instance"); break;
                case 14: Require(pst.Length == 3 && pst.Any(i => i.Quantity == 5) && pst.Sum(i => i.Quantity) == 60 && inventory.Interaction.Drag.Rotated, "Split commit and rotation preview"); break;
                case 15:
                    Require(state.Containers[root].Entries.Values.Single(e => state.Items[e.ItemId].Definition.Identifier == "aks74u").Rotated, "Rotated placement"); break;
                case 16: Require(inventory.Interaction.Drag == null && !state.Items.Values.Any(i => i.Definition.Identifier == "rk0"), "Cancel and delete"); break;
                case 17: Require(state.Items.Count == 13 && pst.Any(i => i.Quantity == 40) && pst.Any(i => i.Quantity == 20), "Atomic demo reset"); break;
            }
            foreach (var item in state.Items.Values)
                Require(item.Quantity >= 1 && item.Quantity <= item.Definition.MaxStack && state.Registry.TryGetOwner(item.Id, out _), "Quantity and ownership");
        }
        private static void Require(bool valid, string condition)
        { if (!valid) throw new InvalidOperationException("Scenario check failed: " + condition); }
    }
}
