using System;
using System.Collections.Generic;
using System.Linq;
namespace Pktony.GridInventory
{
    public sealed class SectionLayoutView
    {
        public string SectionId { get; }
        public float X { get; }
        public float Y { get; }
        public SectionLayoutView(string sectionId, float x, float y) { SectionId = sectionId; X = x; Y = y; }
    }
    public sealed class ContainerLayoutView
    {
        public IReadOnlyList<SectionLayoutView> Sections { get; }
        public ContainerLayoutView(IEnumerable<SectionLayoutView> sections)
        { Sections = Array.AsReadOnly(sections.ToArray()); }
    }
}
