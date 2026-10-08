using System.Collections.Generic;
using Pktony.GridInventory.Presentation;
using UnityEngine;
namespace Pktony.GridInventory.Tests
{
    public sealed class InventoryAudioProbe : IInventoryAudioOutput
    {
        public readonly List<AudioClip> Clips = new();
        public void Play(AudioClip clip, float volume) { if (clip != null && volume > 0) Clips.Add(clip); }
    }
}
