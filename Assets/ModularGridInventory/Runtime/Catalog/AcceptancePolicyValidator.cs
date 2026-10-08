using System;
using System.Collections.Generic;
using System.Linq;
namespace Pktony.GridInventory
{
    public sealed class AcceptancePolicyValidator
    {
        public void Validate(AcceptancePolicyView policy, ISet<string> categories, ISet<DefinitionId> items, ICollection<string> errors)
        {
            if (!Enum.IsDefined(typeof(AcceptanceMode), policy.Mode)) errors.Add("Invalid policy mode.");
            foreach (var c in policy.AllowedCategories.Concat(policy.DeniedCategories))
                if (c == null || !categories.Contains(c)) errors.Add($"Policy category '{c}' is missing.");
            foreach (var id in policy.AllowedItems.Concat(policy.DeniedItems))
                if (!items.Contains(id)) errors.Add($"Policy item '{id}' is missing.");
        }
    }
}
