using UnityEngine;

namespace Pktony.GridInventory.Editor
{
    public readonly struct PocketAuthoringData
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        public Vector2 Position { get; }
        public PocketAuthoringData(string id, int width, int height, Vector2 position)
        { Id = id; Width = width; Height = height; Position = position; }
    }
}
