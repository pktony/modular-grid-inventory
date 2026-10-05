namespace InventorySystem
{
    public sealed class CategoryDefinitionView
    {
        public string Id { get; }
        public string Name { get; }
        public string ParentId { get; }
        public CategoryDefinitionView(string id, string name, string parentId = null)
        { Id = id; Name = name; ParentId = parentId; }
    }
}
