namespace InventorySystem
{
    public class ItemData
    {
        public string itemId;
        public int width;
        public int height;

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