using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public sealed class LegacyInventoryInputSource : IInventoryInputSource
    {
        public bool IsAvailable
        {
            get
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                return true;
#else
                return false;
#endif
            }
        }
        public InventoryInputFrame Read()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return new InventoryInputFrame(Input.mousePosition, Input.GetMouseButton(0) || Input.GetMouseButton(1),
                Input.GetKeyDown(KeyCode.Escape), Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter),
                Input.GetKeyDown(KeyCode.R), Input.GetKeyDown(KeyCode.Delete));
#else
            return default;
#endif
        }
    }
}
