using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Samples
{
    public sealed class InventoryModelExample : MonoBehaviour
    {
        [SerializeField] private InventoryCatalogAsset catalog;
        [SerializeField] private ContainerDefinition rootContainer;
        [SerializeField] private InventoryInitialState initialState;
        public InventoryRuntime Runtime { get; private set; }
        public int ItemCount => Runtime?.ReadModel.Snapshot.Items.Count ?? 0;
        private void Start()
        {
            var definitions = new InventoryCatalogSnapshotFactory().Create(catalog);
            var root = new ContainerDefinitionSnapshotFactory().Create(rootContainer);
            Runtime = new InventoryRuntime(definitions, root, Debug.LogException);
            Runtime.ReadModel.Changed += OnChanged;
            Runtime.Reset.Replace(new InventorySeedBuilder().Create(definitions, root, initialState));
        }
        public MutationResult AddToRoot(ItemDefinition item, int quantity, int x, int y)
        {
            var root = Runtime.ReadModel.Snapshot.RootContainerId;
            return Runtime.Edit.Add(new AddRequest(item.Id, quantity, new PlacementTarget(root, new GridSectionId("main"), x, y)));
        }
        private void OnChanged(InventoryChangeBatch change) => Debug.Log($"Inventory version {change.Version}: {ItemCount} items", this);
        private void OnDestroy()
        {
            if (Runtime == null) return;
            Runtime.ReadModel.Changed -= OnChanged; Runtime.Dispose();
        }
    }
}
