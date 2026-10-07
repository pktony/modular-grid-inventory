using System;
using System.Linq;
using UnityEngine;
namespace Pktony.GridInventory
{
    [CreateAssetMenu(menuName = "Modular Grid Inventory/Container")]
    public sealed class ContainerDefinition : ScriptableObject
    {
        [SerializeField] private string identifier;
        [SerializeField] private GridSectionDefinition[] sections = Array.Empty<GridSectionDefinition>();
        [SerializeField] private ContainerAcceptancePolicy policy = new();
        [SerializeField] private ContainerLayoutDefinition layout;
        internal ContainerDefinitionView Freeze() => new(identifier, sections.Select(s => s != null ? s.Freeze() : null),
            policy.Freeze(), layout != null ? layout.Freeze() : null);
    }
}
