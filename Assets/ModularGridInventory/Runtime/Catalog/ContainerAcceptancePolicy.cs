using System;
using System.Linq;
using UnityEngine;
namespace Pktony.GridInventory
{
    [Serializable]
    public sealed class ContainerAcceptancePolicy
    {
        [SerializeField] private AcceptanceMode mode;
        [SerializeField] private ItemCategoryDefinition[] allowedCategories = Array.Empty<ItemCategoryDefinition>();
        [SerializeField] private ItemCategoryDefinition[] deniedCategories = Array.Empty<ItemCategoryDefinition>();
        [SerializeField] private string[] allowedItemIds = Array.Empty<string>();
        [SerializeField] private string[] deniedItemIds = Array.Empty<string>();
        internal AcceptancePolicyView Freeze() => new(mode,
            allowedCategories.Select(c => c != null ? c.Id : null), deniedCategories.Select(c => c != null ? c.Id : null),
            allowedItemIds.Select(s => new DefinitionId(s)), deniedItemIds.Select(s => new DefinitionId(s)));
    }
}
