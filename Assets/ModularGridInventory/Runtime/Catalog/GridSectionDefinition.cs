using System;
using UnityEngine;
namespace Pktony.GridInventory
{
    [Serializable]
    public sealed class GridSectionDefinition
    {
        [SerializeField] private string identifier;
        [SerializeField, Min(1)] private int width = 1;
        [SerializeField, Min(1)] private int height = 1;
        [SerializeField] private ContainerAcceptancePolicy policy = new();
        internal GridSectionDefinitionView Freeze() => new(identifier, width, height, policy.Freeze());
    }
}
