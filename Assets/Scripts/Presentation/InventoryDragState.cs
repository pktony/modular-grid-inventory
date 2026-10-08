using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public sealed class InventoryDragState
    {
        public ItemInstanceId Item { get; }
        public int SplitQuantity { get; }
        public bool Rotated { get; private set; }
        public Vector2Int Grip { get; private set; }
        public Vector2 Fraction { get; private set; }
        private readonly ItemDefinitionView definition;
        public int Width => Rotated ? definition.Height : definition.Width;
        public int Height => Rotated ? definition.Width : definition.Height;
        public InventoryDragState(ItemInstance item, bool rotated, Vector2Int grip, Vector2 fraction, int splitQuantity = 0)
        { Item = item.Id; definition = item.Definition; Rotated = rotated; Grip = grip; Fraction = fraction; SplitQuantity = splitQuantity; }
        public void Rotate()
        { Grip = new Vector2Int(Height - 1 - Grip.y, Grip.x); Fraction = new Vector2(1 - Fraction.y, Fraction.x); Rotated = !Rotated; }
        public PlacementTarget Placement(PlacementTarget hit) => new(hit.Container, hit.Section, hit.X - Grip.x, hit.Y - Grip.y, Rotated);
    }
}
