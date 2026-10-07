using System;
using Pktony.GridInventory.Domain;
using Pktony.GridInventory.Presentation;
using UnityEngine;
namespace Pktony.GridInventory
{
    public sealed class InventoryBootstrapper : MonoBehaviour
    {
        [SerializeField] private InventoryCatalogAsset catalog;
        [SerializeField] private ContainerDefinition rootContainer;
        [SerializeField] private InventoryInitialState initialState;
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryUiTheme theme;
        [SerializeField] private InventoryInputSourceBehaviour inputSource;
        [SerializeField] private InventoryAudioSettings audioSettings;
        private InventoryRuntime runtime;
        private InventoryUiInstaller ui;
        private InventorySoundPresenter sound;
        private InventoryAudioOutput audioOutput;
        private InventoryCatalog definitions;
        private ContainerDefinitionView rootDefinition;
        public IInventoryReadModel ReadModel => runtime?.ReadModel;
        public IInventoryTransferService Transfer => runtime?.Transfer;
        public IInventoryStackService Stack => runtime?.Stack;
        public IInventoryStorageService Storage => runtime?.Storage;
        public IInventoryEditService Edit => runtime?.Edit;
        public ContainerWindowManager Windows => ui?.Windows;
        public InventoryInteractionController Interaction => ui?.Interaction;
        public InventoryScreenBindings Screen => ui?.Screen;
        public ClickIndicator Indicator => ui?.Indicator;
        private void Start()
        {
            if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay || theme == null || theme.Font == null)
                throw new InvalidOperationException("Assign an overlay Canvas and a UI theme with a font.");
            IInventoryInputSource source = inputSource != null ? inputSource : new LegacyInventoryInputSource();
            if (!source.IsAvailable) throw new InvalidOperationException("Assign an input adapter compatible with the project's Active Input Handling.");
            definitions = new InventoryCatalogSnapshotFactory().Create(catalog);
            rootDefinition = new ContainerDefinitionSnapshotFactory().Create(rootContainer);
            runtime = new InventoryRuntime(definitions, rootDefinition, Debug.LogException);
            ResetState();
            ui = new InventoryUiInstaller(runtime, canvas, new InventoryUiSettings(theme), source);
            Screen.Reset.onClick.AddListener(ResetState);
            if (audioSettings == null) return;
            var audio = new GameObject("InventoryAudio"); audio.transform.SetParent(transform, false);
            audioOutput = audio.AddComponent<InventoryAudioOutput>(); audioOutput.Initialize(audioSettings);
            sound = new InventorySoundPresenter(runtime.ReadModel, Interaction, Windows, Screen,
                new InventorySoundResolver(audioSettings, definitions), audioOutput);
        }
        public void ResetState()
        {
            var result = runtime.Reset.Replace(new InventorySeedBuilder().Create(definitions, rootDefinition, initialState));
            if (!result.Success) throw new InvalidOperationException(result.Reason);
        }
        public void ResetDemo() => ResetState();
        private void Update() => ui?.Tick();
        private void OnDestroy()
        {
            if (Screen?.Reset != null) Screen.Reset.onClick.RemoveListener(ResetState);
            sound?.Dispose(); ui?.Dispose(); runtime?.Dispose();
            if (audioOutput != null) Destroy(audioOutput.gameObject);
        }
    }
}
