using System.Linq;
using InventorySystem.Domain;
using TMPro;
namespace InventorySystem.Presentation
{
    public sealed class InventoryInspectorPresenter
    {
        private readonly TextMeshProUGUI label;
        public InventoryInspectorPresenter(TextMeshProUGUI label) { this.label = label; }
        public void Show(InventorySnapshot snapshot, ItemInstanceId id)
        {
            if (!snapshot.Items.TryGetValue(id, out var item)) { label.text = "Select an item to inspect. Double-click a bag to open it."; return; }
            var d = item.Definition;
            string details = $"{d.DisplayName}   |   {d.CategoryId}   |   {d.Width} x {d.Height}   |   {item.Quantity}/{d.MaxStack}";
            if (d.Container != null) details += $"   |   {d.Container.Sections.Sum(s => s.Width * s.Height)} internal cells / {snapshot.Containers[item.ChildContainerId].Entries.Count} items   |   {Policy(d.Container)}";
            label.text = details;
        }
        public static string Policy(ContainerDefinitionView container)
        {
            var p = container.Policy;
            string allowed = p.Mode == AcceptanceMode.AllowAll ? "All item types" : string.Join(", ", p.AllowedCategories.Concat(p.AllowedItems.Select(id => id.Value)));
            if (allowed.Length == 0) allowed = "No items allowed";
            return "Accepts: " + allowed + (p.DeniedCategories.Count + p.DeniedItems.Count > 0 ? " (exclusions apply)" : "");
        }
    }
}
