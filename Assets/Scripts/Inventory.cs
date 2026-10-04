using UnityEngine;
namespace InventorySystem
{
    public sealed class Inventory : MonoBehaviour
    {
        [SerializeField] private ItemCatalog catalog;
        [SerializeField] private InventoryUI view;
        private InventoryController controller;
        private DemoInventoryActions demo;
        private InventoryInputPump input;
        public InventoryCellData Model { get; private set; }
        public InventoryController Interaction => controller;
        public InventoryUI View => view;
        private void Start()
        {
            Model = new DemoInventoryFactory(catalog).Create();
            view.Initialize(Model);
            controller = new InventoryController(Model, view);
            input = new InventoryInputPump(controller.UpdatePointer, controller.Rotate, controller.Cancel, controller.RemoveSelected);
            demo = new DemoInventoryActions(Model, catalog, view, controller.Cancel);
        }
        private void Update() => input?.Tick();
        private void OnDestroy() { controller?.Dispose(); demo?.Dispose(); }
    }
}
