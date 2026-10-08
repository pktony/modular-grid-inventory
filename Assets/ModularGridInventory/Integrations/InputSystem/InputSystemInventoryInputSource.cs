#if MGI_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using Pktony.GridInventory.Presentation;
using UnityEngine.InputSystem;
namespace Pktony.GridInventory.InputSystem
{
    public sealed class InputSystemInventoryInputSource : InventoryInputSourceBehaviour
    {
        public override bool IsAvailable => true;
        public override InventoryInputFrame Read()
        {
            var mouse = Mouse.current; var keyboard = Keyboard.current;
            return new InventoryInputFrame(mouse != null ? mouse.position.ReadValue() : default,
                mouse != null && (mouse.leftButton.isPressed || mouse.rightButton.isPressed),
                keyboard != null && keyboard.escapeKey.wasPressedThisFrame,
                keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame),
                keyboard != null && keyboard.rKey.wasPressedThisFrame, keyboard != null && keyboard.deleteKey.wasPressedThisFrame);
        }
    }
}
#endif
