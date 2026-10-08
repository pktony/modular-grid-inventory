using System;
namespace InventorySystem
{
    public sealed class DemoInventoryActions : IDisposable
    {
        private readonly IDemoInventoryModel model;
        private readonly IItemCatalog catalog;
        private readonly IDemoInventoryView view;
        private readonly Action cancel;
        private int next;
        public DemoInventoryActions(IDemoInventoryModel model, IItemCatalog catalog, IDemoInventoryView view, Action cancel)
        {
            this.model = model; this.catalog = catalog; this.view = view; this.cancel = cancel;
            view.AddRequested += Add; view.ResetRequested += Reset;
        }
        public void Add()
        {
            cancel();
            var definition = catalog.Definitions[next % catalog.Definitions.Count];
            if (model.TryAdd(new ItemData(definition))) { next++; view.SetStatus($"Added {definition.DisplayName}."); }
            else view.SetStatus("Not enough space for this item. Remove or rearrange items.");
        }
        public void Reset()
        {
            cancel(); model.Clear(); next = 0;
            var seed = new DemoInventoryFactory(catalog).Create();
            foreach (var entry in seed.Entries) model.TryAdd(entry.Item, entry.X, entry.Y);
            view.SetStatus("Demo reset. Original items restored.");
        }
        public void Dispose() { view.AddRequested -= Add; view.ResetRequested -= Reset; }
    }
}
