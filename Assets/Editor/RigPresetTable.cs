using System;
namespace InventorySystem.Editor
{
    [Serializable]
    internal sealed class RigPresetTable
    {
        public string source;
        public float displayStride;
        public RigPreset[] rigs;
    }
}
