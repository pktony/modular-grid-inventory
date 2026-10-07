using System;
using Pktony.GridInventory.Domain;
namespace Pktony.GridInventory.Presentation
{
    public sealed class InventorySoundPresenter : IDisposable
    {
        private readonly IInventoryReadModel model;
        private readonly InventoryInteractionController interaction;
        private readonly ContainerWindowManager windows;
        private readonly InventoryScreenBindings screen;
        private readonly InventorySoundResolver resolver;
        private readonly IInventoryAudioOutput output;
        public InventorySoundPresenter(IInventoryReadModel model, InventoryInteractionController interaction,
            ContainerWindowManager windows, InventoryScreenBindings screen, InventorySoundResolver resolver, IInventoryAudioOutput output)
        {
            this.model = model; this.interaction = interaction; this.windows = windows;
            this.screen = screen; this.resolver = resolver; this.output = output;
            interaction.Feedback += Play; windows.Opened += Opened; windows.Closed += Closed;
            screen.Quantity.Rejected += Rejected; screen.Quantity.Cancelled += Cancelled;
            screen.Reset.onClick.AddListener(Reset);
        }
        private void Play(InventoryFeedbackAction action, ItemDefinitionView item)
        {
            var clip = resolver.Resolve(action, item, out float volume);
            if (clip != null) output.Play(clip, volume);
        }
        private ItemDefinitionView Definition(ItemInstanceId id) => model.Snapshot.Items.TryGetValue(id, out var item) ? item.Definition : null;
        private void Opened(ItemInstanceId id) => Play(InventoryFeedbackAction.Open, Definition(id));
        private void Closed(ItemInstanceId id) => Play(InventoryFeedbackAction.Close, Definition(id));
        private void Rejected() => Play(InventoryFeedbackAction.Reject, null);
        private void Cancelled() => Play(InventoryFeedbackAction.Cancel, null);
        private void Reset() => Play(InventoryFeedbackAction.Reset, null);
        public void Dispose()
        {
            interaction.Feedback -= Play; windows.Opened -= Opened; windows.Closed -= Closed;
            screen.Quantity.Rejected -= Rejected; screen.Quantity.Cancelled -= Cancelled;
            if (screen.Reset != null) screen.Reset.onClick.RemoveListener(Reset);
        }
    }
}
