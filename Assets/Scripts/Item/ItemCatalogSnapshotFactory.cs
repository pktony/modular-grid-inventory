using System;
namespace InventorySystem
{
    public sealed class ItemCatalogSnapshotFactory
    {
        public IItemCatalog Create(ItemCatalog source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var definitions = source.Definitions;
            var views = new ItemDefinitionView[definitions?.Count ?? 0];
            for (int i = 0; i < views.Length; i++)
            {
                var item = definitions[i];
                if (item != null) views[i] = new ItemDefinitionView(new DefinitionId(item.Identifier),
                    item.DisplayName, item.Width, item.Height, item.MaxStack, item.Icon);
            }
            return new ItemCatalogSnapshot(views);
        }
    }
}
