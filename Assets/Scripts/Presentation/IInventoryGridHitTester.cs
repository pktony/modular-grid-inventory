using InventorySystem.Domain;
using UnityEngine;
namespace InventorySystem.Presentation
{
    public interface IInventoryGridHitTester
    {
        bool TryHit(Vector2 screen, out PlacementTarget target);
        InventorySectionView Find(PlacementTarget target);
    }
}
