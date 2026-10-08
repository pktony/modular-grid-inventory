using System.Collections.Generic;
namespace InventorySystem
{
    public sealed class ContainerLayoutFactory
    {
        public ContainerLayoutView CreateHorizontal(IEnumerable<GridSectionDefinitionView> sections)
        {
            var positions = new List<SectionLayoutView>(); float x = 0;
            foreach (var section in sections)
            {
                if (section == null) continue;
                positions.Add(new SectionLayoutView(section.Id, x, 0)); x += section.Width + 1;
            }
            return new ContainerLayoutView(positions);
        }
    }
}
