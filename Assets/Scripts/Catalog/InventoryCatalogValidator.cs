using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem
{
    public sealed class InventoryCatalogValidator
    {
        public IReadOnlyList<string> Validate(IReadOnlyList<CategoryDefinitionView> categories, IReadOnlyList<ItemDefinitionView> items)
        {
            var errors = new List<string>(new ItemCatalogValidator().Validate(items));
            var map = new Dictionary<string, CategoryDefinitionView>();
            foreach (var c in categories)
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Id) || string.IsNullOrWhiteSpace(c.Name))
                { errors.Add("Category ID/name is missing."); continue; }
                if (!map.TryAdd(c.Id, c)) errors.Add($"Duplicate category '{c.Id}'.");
            }
            foreach (var c in map.Values)
            {
                var visited = new HashSet<string>(); var current = c.Id;
                while (!string.IsNullOrEmpty(current))
                {
                    if (!visited.Add(current)) { errors.Add($"Category cycle at '{c.Id}'."); break; }
                    if (!map.TryGetValue(current, out var node)) { errors.Add($"Missing category '{current}'."); break; }
                    current = node.ParentId;
                }
            }
            var itemIds = new HashSet<DefinitionId>(items.Where(i => i != null).Select(i => i.Id));
            var containers = new Dictionary<string, ContainerDefinitionView>();
            foreach (var item in items.Where(i => i != null))
            {
                if (string.IsNullOrWhiteSpace(item.CategoryId) || !map.ContainsKey(item.CategoryId))
                    errors.Add($"{item.Identifier}: missing category.");
                if (item.Container == null) continue;
                if (item.MaxStack != 1) errors.Add($"{item.Identifier}: containers cannot stack.");
                var container = item.Container;
                if (string.IsNullOrWhiteSpace(container.Id)) errors.Add("Container ID is missing.");
                else if (containers.TryGetValue(container.Id, out var previous) && !ReferenceEquals(previous, container))
                    errors.Add($"Duplicate container '{container.Id}'.");
                else containers[container.Id] = container;
                ValidateContainer(container, map, itemIds, errors);
            }
            return errors.AsReadOnly();
        }
        private static void ValidateContainer(ContainerDefinitionView container, Dictionary<string, CategoryDefinitionView> categories,
            HashSet<DefinitionId> itemIds, List<string> errors)
        {
            if (container.Sections.Count == 0) errors.Add($"{container.Id}: sections are empty.");
            var ids = new HashSet<string>();
            ValidatePolicy(container.Policy, categories, itemIds, errors);
            foreach (var s in container.Sections)
            {
                if (s == null) { errors.Add("Missing section."); continue; }
                if (string.IsNullOrWhiteSpace(s.Id) || !ids.Add(s.Id)) errors.Add($"{container.Id}: missing/duplicate section ID.");
                if (s.Width < 1 || s.Height < 1) errors.Add($"{s.Id}: invalid section size.");
                ValidatePolicy(s.Policy, categories, itemIds, errors);
            }
            var layoutIds = new HashSet<string>();
            foreach (var l in container.Layout.Sections)
            {
                if (l == null || !ids.Contains(l.SectionId) || !layoutIds.Add(l.SectionId)) errors.Add("Invalid layout section reference.");
                else if (float.IsNaN(l.X) || float.IsNaN(l.Y) || float.IsInfinity(l.X) || float.IsInfinity(l.Y) || l.X < 0 || l.Y < 0)
                    errors.Add("Invalid layout position.");
            }
            if (!ids.SetEquals(layoutIds)) errors.Add("Layout must cover every section exactly once.");
        }
        private static void ValidatePolicy(AcceptancePolicyView policy, Dictionary<string, CategoryDefinitionView> categories,
            HashSet<DefinitionId> items, List<string> errors)
        {
            if (!Enum.IsDefined(typeof(AcceptanceMode), policy.Mode)) errors.Add("Invalid policy mode.");
            foreach (var c in policy.AllowedCategories.Concat(policy.DeniedCategories))
                if (c == null || !categories.ContainsKey(c)) errors.Add($"Policy category '{c}' is missing.");
            foreach (var id in policy.AllowedItems.Concat(policy.DeniedItems))
                if (!items.Contains(id)) errors.Add($"Policy item '{id}' is missing.");
        }
    }
}
