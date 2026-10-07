using System;
using System.Linq;
using Pktony.GridInventory.Domain;
namespace Pktony.GridInventory
{
    public sealed class InventoryScenarioVerifier
    {
        public void Verify(InventoryBootstrapper inventory, int stage)
        {
            var state = inventory.ReadModel.Snapshot; var root = state.RootContainerId;
            var pst = state.Items.Values.Where(i => i.Definition.Identifier == "pst").ToArray();
            var windows = inventory.Windows.Windows;
            switch (stage)
            {
                case 0: Require(state.Items.Count == 22 && windows.Count == 0, "Initial seed"); break;
                case 1: Require(windows.Count == 1, "First window"); break;
                case 2: case 3: Require(windows.Count == 2, "Two independent windows"); break;
                case 4: case 5: case 6: case 7: Require(windows.Count == 3, "Nested window and reuse"); break;
                case 8: case 10: case 13: case 17: Require(inventory.Interaction.Drag != null, "Transfer preview"); break;
                case 9:
                    var nested = state.Items.Values.Single(i => i.Definition.Identifier == "mbss");
                    state.Registry.TryGetOwner(nested.Id, out var owner);
                    Require(state.Containers[owner].Entries.Count == 1 && windows.ContainsKey(nested.ChildContainerId), "Filled bag and window intact"); break;
                case 11:
                    nested = state.Items.Values.Single(i => i.Definition.Identifier == "mbss");
                    Require(state.Containers[nested.ChildContainerId].Entries.Count == 0 && windows.Count == 3, "Bag-icon storage"); break;
                case 12: Require(windows.Count == 1 && inventory.Windows.Frontmost.Panel.Policy.text.Contains("Ammo"), "Case window"); break;
                case 14: Require(state.Containers[inventory.Windows.Frontmost.Id].Entries.Count == 1, "Case receives ammunition"); break;
                case 15: case 16:
                    Require(state.Containers[root].Entries.Values.Any(e => state.Items[e.ItemId].Definition.Identifier == "aks74u") && windows.Count == 2, "Wrong type rejected; rig open"); break;
                case 18: Require(state.Containers[inventory.Windows.Frontmost.Id].Entries.Values.Single().SectionId.Value == "small-a", "Rig pocket transfer"); break;
                case 19: Require(pst.Sum(i => i.Quantity) == 60 && windows.Count == 0, "Reset and merge preview"); break;
                case 20: Require(pst.Any(i => i.Quantity == 50) && pst.Any(i => i.Quantity == 10) && inventory.Screen.Quantity.IsOpen, "Partial merge and dialog"); break;
                case 21: Require(inventory.Interaction.Drag?.SplitQuantity == 5 && pst.Length == 2, "Split preview has no new instance"); break;
                case 22: Require(pst.Length == 3 && pst.Sum(i => i.Quantity) == 60 && inventory.Interaction.Drag.Rotated, "Split and rotation preview"); break;
                case 23: Require(state.Containers[root].Entries.Values.Single(e => state.Items[e.ItemId].Definition.Identifier == "aks74u").Rotated, "Rotated placement"); break;
                case 24: Require(inventory.Interaction.Drag == null && !state.Items.Values.Any(i => i.Definition.Identifier == "rk0"), "Cancel and delete"); break;
                case 25: case 26: Require(state.Items.Count == 22 && windows.Count == 2, "Reset and ready windows"); break;
            }
            foreach (var item in state.Items.Values)
                Require(item.Quantity >= 1 && item.Quantity <= item.Definition.MaxStack && state.Registry.TryGetOwner(item.Id, out _), "Quantity and ownership");
        }
        private static void Require(bool valid, string condition)
        { if (!valid) throw new InvalidOperationException("Scenario check failed: " + condition); }
    }
}
