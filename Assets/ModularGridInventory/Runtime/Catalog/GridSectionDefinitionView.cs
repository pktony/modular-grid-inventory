namespace Pktony.GridInventory
{
    public sealed class GridSectionDefinitionView
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        public AcceptancePolicyView Policy { get; }
        public GridSectionDefinitionView(string id, int width, int height, AcceptancePolicyView policy = null)
        { Id = id; Width = width; Height = height; Policy = policy ?? AcceptancePolicyView.All; }
    }
}
