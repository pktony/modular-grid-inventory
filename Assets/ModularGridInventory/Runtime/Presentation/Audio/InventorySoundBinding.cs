using System;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    [Serializable]
    public sealed class InventorySoundBinding
    {
        public InventoryFeedbackAction action;
        public AudioClip[] clips = Array.Empty<AudioClip>();
        [Range(0, 1)] public float volume = 0.65f;
    }
}
