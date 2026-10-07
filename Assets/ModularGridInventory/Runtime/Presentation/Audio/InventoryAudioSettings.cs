using System;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    [CreateAssetMenu(menuName = "Inventory/Audio Settings")]
    public sealed class InventoryAudioSettings : ScriptableObject
    {
        [Range(0, 1)] public float masterVolume = 0.65f;
        public bool muted;
        public InventorySoundBinding[] defaults = Array.Empty<InventorySoundBinding>();
        public InventoryItemSoundProfile[] itemProfiles = Array.Empty<InventoryItemSoundProfile>();
    }
}
