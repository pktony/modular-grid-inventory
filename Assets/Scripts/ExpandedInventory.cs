using InventorySystem.Domain;
using InventorySystem.Presentation;
using UnityEngine;
namespace InventorySystem
{
    public sealed class ExpandedInventory : MonoBehaviour
    {
        [SerializeField] private InventoryCatalogAsset catalog;
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryAudioSettings audioSettings;
        private InventorySoundPresenter sound;
        private InventoryAudioOutput audioOutput;
        private InventoryRuntime runtime;
        private InventoryPresenter presenter;
        private InventoryInteractionInput input;
        private ClickIndicator indicator;
        private InventoryCatalog definitions;
        public IInventoryReadModel ReadModel => runtime?.ReadModel;
        public IInventoryTransferService Transfer => runtime.Transfer;
        public IInventoryStackService Stack => runtime.Stack;
        public IInventoryStorageService Storage => runtime.Storage;
        public ContainerWindowManager Windows { get; private set; }
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
            var items = new InventoryItemVisualFactory();
            Windows = new ContainerWindowManager(new ContainerWindowFactory(Screen.WindowLayer, panels, items, Screen.Events), Screen.Stash);
            presenter = new InventoryPresenter(runtime.ReadModel, Screen, panels, items, Windows);
            var hitTest = new InventoryGridHitTester(Windows);
            var drops = new InventoryDropResolver(runtime.ReadModel, runtime.Transfer, runtime.Stack, runtime.Storage, hitTest);
            Interaction = new InventoryInteractionController(runtime.ReadModel, runtime.Edit, presenter, hitTest, drops, Screen);
            indicator = new ClickIndicator(Screen.Overlay);
            input = new InventoryInteractionInput(Interaction, Screen.Quantity, indicator);
            Screen.Reset.onClick.AddListener(ResetDemo);
            var audio = new GameObject("InventoryAudio"); audio.transform.SetParent(transform, false);
            audioOutput = audio.AddComponent<InventoryAudioOutput>();
            var settings = audioSettings != null ? audioSettings : Resources.Load<InventoryAudioSettings>("InventoryAudioSettings");
            audioOutput.Initialize(settings);
            sound = new InventorySoundPresenter(runtime.ReadModel, Interaction, Windows, Screen, new InventorySoundResolver(settings, definitions), audioOutput);
        }
        public void ResetDemo() => ResetState();
        private void ResetState()
        {
            var result = runtime.Reset.Replace(new ExpandedDemoSeed().Create(definitions));
            if (!result.Success) throw new System.InvalidOperationException(result.Reason);
        }
        private void Update() => input?.Tick();
        private void OnDestroy()
        {
            if (Screen?.Reset != null) Screen.Reset.onClick.RemoveListener(ResetDemo);
            sound?.Dispose(); Interaction?.Dispose(); presenter?.Dispose(); indicator?.Dispose(); runtime?.Dispose();
            if (audioOutput != null) Destroy(audioOutput.gameObject);
            if (Screen?.Root != null) Destroy(Screen.Root.gameObject);
        }
    }
}
