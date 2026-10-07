using System;
using Pktony.GridInventory.Domain;
using Pktony.GridInventory.Presentation;
using UnityEngine;
namespace Pktony.GridInventory
{
    public sealed class InventoryUiInstaller : IDisposable
    {
        private readonly InventoryPresenter presenter;
        private readonly InventoryInteractionInput input;
        public InventoryScreenBindings Screen { get; }
        public ContainerWindowManager Windows { get; }
        public InventoryInteractionController Interaction { get; }
        public ClickIndicator Indicator { get; }
        public InventoryUiInstaller(InventoryRuntime runtime, Canvas canvas, InventoryUiTheme theme, IInventoryInputSource source)
        {
            var elements = new InventoryElementFactory(theme);
            var panels = new InventoryPanelFactory(elements); var items = new InventoryItemVisualFactory(elements);
            Screen = new InventoryScreenFactory(panels, elements).Create(canvas.transform);
            Windows = new ContainerWindowManager(new ContainerWindowFactory(Screen.WindowLayer, panels, items, Screen.Events, elements), Screen.Stash);
            presenter = new InventoryPresenter(runtime.ReadModel, Screen, panels, items, Windows);
            var hitTest = new InventoryGridHitTester(Windows);
            var drops = new InventoryDropResolver(runtime.ReadModel, runtime.Transfer, runtime.Stack, runtime.Storage, hitTest);
            Interaction = new InventoryInteractionController(runtime.ReadModel, runtime.Edit, presenter, hitTest, drops, Screen);
            Indicator = new ClickIndicator(Screen.Overlay, elements);
            input = new InventoryInteractionInput(Interaction, Screen.Quantity, Indicator, source, theme.ShowClickIndicator);
        }
        public void Tick() => input.Tick();
        public void Dispose()
        {
            Interaction.Dispose(); presenter.Dispose(); Indicator.Dispose();
            if (Screen.Root != null) UnityEngine.Object.Destroy(Screen.Root.gameObject);
        }
    }
}
