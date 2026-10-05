using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventorySoundResolver
    {
        private readonly InventoryAudioSettings settings;
        private readonly Dictionary<string, string> parents;
        private readonly Dictionary<InventorySoundBinding, int> next = new();
        public InventorySoundResolver(InventoryAudioSettings settings, InventoryCatalog catalog)
        { this.settings = settings; parents = catalog.Categories.ToDictionary(c => c.Id, c => c.ParentId); }
        public AudioClip Resolve(InventoryFeedbackAction action, ItemDefinitionView item, out float volume)
        {
            volume = 0;
            if (settings == null) return null;
            string category = item?.CategoryId;
            while (!string.IsNullOrEmpty(category))
            {
                foreach (var profile in settings.itemProfiles ?? Array.Empty<InventoryItemSoundProfile>())
                {
                    if (profile == null || profile.categoryId != category) continue;
                    var clip = Choose(profile.sounds, action, out volume);
                    if (clip != null) return clip;
                }
                category = parents.TryGetValue(category, out var parent) ? parent : null;
            }
            return Choose(settings.defaults, action, out volume);
        }
        private AudioClip Choose(InventorySoundBinding[] bindings, InventoryFeedbackAction action, out float volume)
        {
            volume = 0;
            foreach (var binding in bindings ?? Array.Empty<InventorySoundBinding>())
            {
                if (binding == null || binding.action != action || binding.clips == null || binding.clips.Length == 0) continue;
                next.TryGetValue(binding, out int start);
                for (int offset = 0; offset < binding.clips.Length; offset++)
                {
                    int index = (start + offset) % binding.clips.Length;
                    if (binding.clips[index] == null) continue;
                    next[binding] = (index + 1) % binding.clips.Length;
                    volume = Mathf.Clamp01(binding.volume); return binding.clips[index];
                }
            }
            return null;
        }
    }
}
