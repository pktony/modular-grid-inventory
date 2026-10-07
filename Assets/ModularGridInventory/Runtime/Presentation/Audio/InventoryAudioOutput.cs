using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventoryAudioOutput : MonoBehaviour, IInventoryAudioOutput
    {
        private readonly AudioSource[] voices = new AudioSource[6];
        private InventoryAudioSettings settings;
        private int next;
        public void Initialize(InventoryAudioSettings settings)
        {
            this.settings = settings;
            for (int i = 0; i < voices.Length; i++)
            {
                var voice = gameObject.AddComponent<AudioSource>();
                voice.playOnAwake = false; voice.loop = false; voice.spatialBlend = 0;
                voice.ignoreListenerPause = true; voice.priority = 32; voices[i] = voice;
            }
            Update();
        }
        public void Play(AudioClip clip, float volume)
        {
            if (clip == null || settings == null || settings.muted || settings.masterVolume <= 0 || volume <= 0) return;
            int index = next;
            for (int offset = 0; offset < voices.Length; offset++)
            {
                int candidate = (next + offset) % voices.Length;
                if (voices[candidate].isPlaying) continue;
                index = candidate; break;
            }
            var voice = voices[index]; next = (index + 1) % voices.Length;
            voice.Stop(); voice.volume = Mathf.Clamp01(settings.masterVolume); voice.mute = false;
            voice.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
        private void Update()
        {
            foreach (var voice in voices)
                if (voice != null) { voice.volume = settings == null ? 0 : Mathf.Clamp01(settings.masterVolume); voice.mute = settings == null || settings.muted; }
        }
    }
}
