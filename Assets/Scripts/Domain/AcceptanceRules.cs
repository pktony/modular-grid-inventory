using System.Linq;
namespace InventorySystem.Domain
{
    public sealed class AcceptanceRules
    {
        private readonly InventoryCatalog catalog;
        public AcceptanceRules(InventoryCatalog catalog) { this.catalog = catalog; }
        public bool Allows(AcceptancePolicyView policy, ItemDefinitionView item)
        {
            if (policy.DeniedItems.Contains(item.Id) || policy.DeniedCategories.Any(c => catalog.IsCategory(item.CategoryId, c))) return false;
            return policy.Mode == AcceptanceMode.AllowAll || policy.AllowedItems.Contains(item.Id)
                || policy.AllowedCategories.Any(c => catalog.IsCategory(item.CategoryId, c));
        }
    }
}
