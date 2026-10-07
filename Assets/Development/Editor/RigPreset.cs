using System;
namespace Pktony.GridInventory.Editor
{
    [Serializable]
    internal sealed class RigPreset
    {
        public string id;
        public string title;
        public int width;
        public int height;
        public RigPocketPreset[] pockets;
    }
}
