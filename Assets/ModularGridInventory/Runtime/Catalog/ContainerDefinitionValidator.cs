using System.Collections.Generic;
namespace Pktony.GridInventory
{
    public sealed class ContainerDefinitionValidator
    {
        public void Validate(ContainerDefinitionView container, ISet<string> categories, ISet<DefinitionId> items, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(container.Id)) errors.Add("Container ID is missing.");
            if (container.Sections.Count == 0) errors.Add($"{container.Id}: sections are empty.");
            var sections = new Dictionary<string, GridSectionDefinitionView>(); var policies = new AcceptancePolicyValidator();
            policies.Validate(container.Policy, categories, items, errors);
            foreach (var section in container.Sections)
            {
                if (section == null) { errors.Add("Missing section."); continue; }
                if (string.IsNullOrWhiteSpace(section.Id) || !sections.TryAdd(section.Id, section)) errors.Add($"{container.Id}: missing/duplicate section ID.");
                if (section.Width < 1 || section.Height < 1) errors.Add($"{section.Id}: invalid section size.");
                policies.Validate(section.Policy, categories, items, errors);
            }
            new ContainerLayoutValidator().Validate(container, sections, errors);
        }
    }
}
