using System;
namespace Pktony.GridInventory.Editor
{
    [Serializable]
    internal sealed class RigPresetTable
    {
        public string source;
        public float displayStride;
        public RigPreset[] rigs;
    }
}
