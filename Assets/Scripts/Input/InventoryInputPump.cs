using System;
using UnityEngine;
namespace InventorySystem
{
    public sealed class InventoryInputPump
    {
        private readonly Action<Vector2> pointer;
        private readonly Action rotate, cancel, remove;
        public InventoryInputPump(Action<Vector2> pointer, Action rotate, Action cancel, Action remove)
        { this.pointer = pointer; this.rotate = rotate; this.cancel = cancel; this.remove = remove; }
        public void Tick()
        {
            pointer(Input.mousePosition);
            if (Input.GetKeyDown(KeyCode.Escape)) { cancel(); return; }
            if (Input.GetKeyDown(KeyCode.R)) rotate();
            if (Input.GetKeyDown(KeyCode.Delete)) remove();
        }
    }
}
