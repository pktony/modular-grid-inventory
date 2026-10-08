using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem
{
    public sealed class ContainerDefinitionView
    {
        public string Id { get; }
        public IReadOnlyList<GridSectionDefinitionView> Sections { get; }
        public AcceptancePolicyView Policy { get; }
        public ContainerLayoutView Layout { get; }
        public ContainerDefinitionView(string id, IEnumerable<GridSectionDefinitionView> sections,
            AcceptancePolicyView policy = null, ContainerLayoutView layout = null)
        {
            Id = id; Sections = Array.AsReadOnly(sections.ToArray());
            Policy = policy ?? AcceptancePolicyView.All;
            Layout = layout ?? new ContainerLayoutView(Sections.Select((s, i) => new SectionLayoutView(s.Id, i * 9, 0)));
        }
    }
}
