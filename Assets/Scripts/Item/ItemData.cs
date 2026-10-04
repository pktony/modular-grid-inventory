namespace InventorySystem
{
    public class ItemData
    {
        public string InstanceId { get; } = System.Guid.NewGuid().ToString("N");
        public string itemId { get; }
        public int width { get; }
        public int height { get; }
        public ItemDefinition Definition { get; }

        public ItemDirection itemDirection { get; internal set; }

        public ItemData(ItemDefinition definition, ItemDirection direction = ItemDirection.Horizontal)
            : this(definition.Identifier, definition.Width, definition.Height)
        {
            Definition = definition;
            itemDirection = direction;
        }

        public ItemData(string itemId, int width, int height)
        {
            this.itemId = itemId;
            this.width = width;
            this.height = height;
        }

        public (int absWidth, int absHeight) GetAbsoluteSize()
        {
            var width = itemDirection == ItemDirection.Horizontal ? this.width : this.height;
            var height = itemDirection == ItemDirection.Horizontal ? this.height : this.width;
            return (width, height);
        }
    }
}
