using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryGridGeometry
    {
        public float CellSize { get; }
        public float Pitch => CellSize + 2;
        private readonly RectTransform grid;
        public InventoryGridGeometry(RectTransform grid, float cellSize) { this.grid = grid; CellSize = cellSize; }
        public Vector2 Position(int x, int y) => new(x * Pitch, -y * Pitch);
        public Vector2 Size(int width, int height) => new(width * Pitch - 2, height * Pitch - 2);
        public Vector2Int CellAt(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, screenPosition, null, out var local);
            return new Vector2Int(Mathf.FloorToInt(local.x / Pitch), Mathf.FloorToInt(-local.y / Pitch));
        }
        public Vector3 WorldPosition(int x, int y) => grid.TransformPoint(Position(x, y));
    }
}
