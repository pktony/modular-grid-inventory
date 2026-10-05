using System;
using UnityEngine;
namespace InventorySystem.Presentation
{
    [Serializable]
    public sealed class InventoryItemSoundProfile
    {
        public string categoryId;
        public InventorySoundBinding[] sounds = Array.Empty<InventorySoundBinding>();
    }
}
