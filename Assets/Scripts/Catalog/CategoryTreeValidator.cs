using System.Collections.Generic;
namespace InventorySystem
{
    public sealed class CategoryTreeValidator
    {
        public Dictionary<string, CategoryDefinitionView> Validate(IReadOnlyList<CategoryDefinitionView> categories, ICollection<string> errors)
        {
            var map = new Dictionary<string, CategoryDefinitionView>();
            foreach (var c in categories)
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Id) || string.IsNullOrWhiteSpace(c.Name)) { errors.Add("Category ID/name is missing."); continue; }
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
            return map;
        }
    }
}
