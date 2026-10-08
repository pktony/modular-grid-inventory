using InventorySystem.Domain;
using InventorySystem.Presentation;
using UnityEngine;
namespace InventorySystem
{
    public sealed class ExpandedInventory : MonoBehaviour
    {
        [SerializeField] private InventoryCatalogAsset catalog;
        [SerializeField] private Canvas canvas;
        private InventoryRuntime runtime;
        private InventoryPresenter presenter;
        private InventoryInteractionInput input;
        private ClickIndicator indicator;
        private InventoryCatalog definitions;
        public IInventoryReadModel ReadModel => runtime?.ReadModel;
        public IInventoryTransferService Transfer => runtime.Transfer;
        public IInventoryStackService Stack => runtime.Stack;
        public IInventoryEditService Edit => runtime.Edit;
        public InventoryInteractionController Interaction { get; private set; }
        public Presentation.InventoryScreenBindings Screen { get; private set; }
        public ClickIndicator Indicator => indicator;
        private void Start()
        {
            definitions = new InventoryCatalogSnapshotFactory().Create(catalog);
            runtime = new InventoryRuntime(definitions, ExpandedDemoSeed.StashDefinition(), Debug.LogException);
            ResetState();
            var panels = new InventoryPanelFactory(); Screen = new InventoryScreenFactory(panels).Create(canvas.transform);
            var navigation = new ContainerNavigation();
            presenter = new InventoryPresenter(runtime.ReadModel, Screen, panels, new InventoryItemVisualFactory(), navigation);
            Interaction = new InventoryInteractionController(runtime.ReadModel, runtime.Transfer, runtime.Stack, runtime.Edit,
                presenter, new InventoryGridHitTester(Screen.Stash, Screen.Bag), Screen);
            indicator = new ClickIndicator(Screen.Root);
            input = new InventoryInteractionInput(Interaction, Screen.Quantity, indicator);
            Screen.Reset.onClick.AddListener(ResetDemo);
        }
        public void ResetDemo() { Interaction.Cancel(); ResetState(); }
        private void ResetState()
        {
            var result = runtime.Reset.Replace(new ExpandedDemoSeed().Create(definitions));
            if (!result.Success) throw new System.InvalidOperationException(result.Reason);
        }
        private void Update() => input?.Tick();
        private void OnDestroy()
        {
            if (Screen?.Reset != null) Screen.Reset.onClick.RemoveListener(ResetDemo);
            Interaction?.Dispose(); presenter?.Dispose(); indicator?.Dispose(); runtime?.Dispose();
        }
    }
}
