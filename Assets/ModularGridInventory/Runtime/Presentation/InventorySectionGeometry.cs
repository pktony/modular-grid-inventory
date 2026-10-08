using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventorySectionGeometry
    {
        public float Pitch { get; }
        public RectTransform Rect { get; }
        public GridSectionId Id { get; }
        public GridSectionDefinitionView Definition { get; }
        public InventorySectionGeometry(RectTransform rect, GridSectionDefinitionView definition, float pitch = 50)
        { Pitch = pitch; Rect = rect; Definition = definition; Id = new GridSectionId(definition.Id); }
        public bool TryCell(Vector2 screen, out Vector2Int cell)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screen, null, out var local);
            cell = new Vector2Int(Mathf.FloorToInt(local.x / Pitch), Mathf.FloorToInt(-local.y / Pitch));
            return cell.x >= 0 && cell.y >= 0 && cell.x < Definition.Width && cell.y < Definition.Height;
        }
        public Vector2 GripFraction(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screen, null, out var local);
            return new Vector2(Mathf.Repeat(local.x, Pitch) / Pitch, Mathf.Repeat(-local.y, Pitch) / Pitch);
        }
        public Vector2 ScreenPoint(int x, int y, Vector2 offset) => RectTransformUtility.WorldToScreenPoint(null,
            Rect.TransformPoint(new Vector3(x * Pitch + offset.x, -y * Pitch - offset.y)));
        public static Vector2 Size(int width, int height, float pitch = 50, float gap = 2) => new(width * pitch - gap, height * pitch - gap);
    }
}
