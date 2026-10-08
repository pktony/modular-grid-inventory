using UnityEngine;
namespace Pktony.GridInventory.Presentation
{
    public readonly struct InventoryInputFrame
    {
        public Vector2 Pointer { get; }
        public bool Pressed { get; }
        public bool Cancel { get; }
        public bool Confirm { get; }
        public bool Rotate { get; }
        public bool Delete { get; }
        public InventoryInputFrame(Vector2 pointer, bool pressed, bool cancel, bool confirm, bool rotate, bool delete)
        { Pointer = pointer; Pressed = pressed; Cancel = cancel; Confirm = confirm; Rotate = rotate; Delete = delete; }
    }
}
