using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public interface IInventoryGridHitTester
    {
        bool TryHit(Vector2 screen, out PlacementTarget target);
        InventorySectionView Find(PlacementTarget target);
        RectTransform FindItem(ItemInstanceId id);
    }
}
