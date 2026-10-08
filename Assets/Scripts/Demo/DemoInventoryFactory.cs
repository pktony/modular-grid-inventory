namespace InventorySystem
{
    public sealed class DemoInventoryFactory
    {
        private readonly IItemCatalog catalog;
        public DemoInventoryFactory(IItemCatalog catalog) => this.catalog = catalog;
        public InventoryCellData Create()
        {
            var model = new InventoryCellData(9, 20);
            for (int i = 0; i < catalog.Definitions.Count; i++)
            {
                var direction = i == 3 || i == 5 || i == 6 ? ItemDirection.Vertical : ItemDirection.Horizontal;
                model.TryAdd(new ItemData(catalog.Definitions[i], direction));
            }
            return model;
        }
    }
}
