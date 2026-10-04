namespace InventorySystem
{
    public class ItemData
    {
        public string InstanceId { get; } = System.Guid.NewGuid().ToString("N");
        public string itemId { get; }
        public int width { get; }
        public int height { get; }

        public ItemDirection itemDirection;

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
