using System;
using System.Linq;
using Pktony.GridInventory.Domain;
namespace Pktony.GridInventory.Editor
{
    public sealed class ContainerShowcaseVerifier
    {
        private InventorySnapshot beforeRejection;
        private static ItemInstance Find(InventorySnapshot state, string definition, int quantity = 0) => state.Items.Values.First(i =>
            i.Definition.Identifier == definition && (quantity == 0 || i.Quantity == quantity));
        private static ContainerId Owner(InventorySnapshot state, ItemInstanceId item)
        { Require(state.Registry.TryGetOwner(item, out var owner), "Item owner exists"); return owner; }
        private static void Require(bool valid, string condition)
        { if (!valid) throw new InvalidOperationException("Container showcase: " + condition); }
        public void Verify(InventoryBootstrapper inventory, int stage)
        {
            var state = inventory.ReadModel.Snapshot;
            var rig = Find(state, "rig");
            switch (stage)
            {
                case 0: Require(state.Items.Count == 22 && inventory.Windows.Windows.Count == 1, "Initial rig window"); break;
                case 3: Require(Owner(state, Find(state, "pst", 40).Id) == rig.ChildContainerId, "Ammo inside rig"); break;
                case 5: Require(state.Containers[rig.ChildContainerId].Entries.Count == 2, "Medical item inside rig"); break;
                case 7: Require(Owner(state, Find(state, "rk0").Id) == rig.ChildContainerId, "Weapon part inside rig"); break;
                case 8: case 10: case 12: case 22: case 30: case 36: case 42: case 45:
                    Require(inventory.Interaction.Drag != null, "Rejection starts with a real drag"); beforeRejection = state; break;
                case 9: case 11: case 13: case 23: case 31: case 37: case 43: case 46:
                    Require(ReferenceEquals(beforeRejection, state), "Rejected drop leaves the same snapshot");
                    Require(inventory.Interaction.Drag == null, "Rejected gesture released"); break;
                case 17:
                    var rigOwner = Owner(state, rig.Id);
                    Require(rigOwner != state.RootContainerId && state.Containers[rig.ChildContainerId].Entries.Count == 3, "Filled rig stored in bag");
                    Require(inventory.Windows.Windows.ContainsKey(rig.ChildContainerId), "Rig window survives storage"); break;
                case 25: Require(Owner(state, Find(state, "mbss").Id) == state.RootContainerId, "Filled MBSS extracted"); break;
                case 27: case 29:
                    rigOwner = Owner(state, rig.Id);
                    Require(state.Registry.TryGetBag(rigOwner, out var bagId), "Rig owner is a bag");
                    var bag = state.Items[bagId];
                    var outerOwner = Owner(state, bag.Id);
                    Require(outerOwner != state.RootContainerId, "Bag inside another bag");
                    Require(state.Containers[rig.ChildContainerId].Entries.Count == 3, "Nested rig contents preserved"); break;
                case 35: Require(state.Containers[Find(state, "medicine-case").ChildContainerId].Entries.Count == 1, "Medical case receives AI-2"); break;
                case 41: Require(Owner(state, Find(state, "pst", 20).Id) == Find(state, "ammo-case").ChildContainerId, "Ammo case receives ammo"); break;
                case 48: Require(state.Items.Count == 22 && inventory.Windows.Windows.Count == 0, "Reset after showcase"); break;
            }
            foreach (var item in state.Items.Values)
                Require(item.Quantity >= 1 && item.Quantity <= item.Definition.MaxStack && state.Registry.TryGetOwner(item.Id, out _), "Quantity and ownership preserved");
        }
    }
}
