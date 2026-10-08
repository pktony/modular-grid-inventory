using System;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    [Serializable]
    public sealed class InventoryItemSoundProfile
    {
        public string categoryId;
        public InventorySoundBinding[] sounds = Array.Empty<InventorySoundBinding>();
    }
}
