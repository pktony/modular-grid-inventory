using System;
namespace Pktony.GridInventory.Editor
{
    [Serializable]
    internal sealed class RigPreset
    {
        public string id;
        public string title;
        public string gameId;
        public int width;
        public int height;
        public string layoutReference;
        public string iconSource;
        public RigPocketPreset[] pockets;
    }
}
