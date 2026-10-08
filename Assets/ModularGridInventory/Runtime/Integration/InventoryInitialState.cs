using System;
using System.Collections.Generic;
using UnityEngine;
namespace Pktony.GridInventory
{
    [CreateAssetMenu(menuName = "Modular Grid Inventory/Initial State")]
    public sealed class InventoryInitialState : ScriptableObject
    {
        [SerializeField] private InventorySeedEntry[] entries = Array.Empty<InventorySeedEntry>();
        public IReadOnlyList<InventorySeedEntry> Entries => Array.AsReadOnly(entries);
    }
}
