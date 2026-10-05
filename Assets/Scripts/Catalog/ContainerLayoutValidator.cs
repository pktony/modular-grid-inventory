using System;
using System.Collections.Generic;
namespace InventorySystem
{
    public sealed class ContainerLayoutValidator
    {
        public void Validate(ContainerDefinitionView container, IReadOnlyDictionary<string, GridSectionDefinitionView> sections, ICollection<string> errors)
        {
            var layoutIds = new HashSet<string>(); var positions = new List<SectionLayoutView>();
            foreach (var l in container.Layout.Sections)
            {
                if (l == null || l.SectionId == null || !sections.ContainsKey(l.SectionId) || !layoutIds.Add(l.SectionId))
                { errors.Add("Invalid layout section reference."); continue; }
                if (float.IsNaN(l.X) || float.IsNaN(l.Y) || float.IsInfinity(l.X) || float.IsInfinity(l.Y) || l.X < 0 || l.Y < 0)
                { errors.Add("Invalid layout position."); continue; }
                var size = sections[l.SectionId];
                foreach (var previous in positions)
                {
                    var other = sections[previous.SectionId];
                    if (l.X < previous.X + other.Width && l.X + size.Width > previous.X && l.Y < previous.Y + other.Height && l.Y + size.Height > previous.Y)
                        errors.Add("Layout sections overlap.");
                }
                positions.Add(l);
            }
            if (!layoutIds.SetEquals(sections.Keys)) errors.Add("Layout must cover every section exactly once.");
        }
    }
}
