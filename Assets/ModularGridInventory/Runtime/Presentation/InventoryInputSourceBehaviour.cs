using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public abstract class InventoryInputSourceBehaviour : MonoBehaviour, IInventoryInputSource
    {
        public abstract bool IsAvailable { get; }
        public abstract InventoryInputFrame Read();
    }
}
