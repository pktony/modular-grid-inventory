using System.Collections.Generic;
namespace Pktony.GridInventory
{
    public sealed class ItemDefinitionValidator
    {
        public void Validate(ItemDefinitionView definition, string path, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(definition.Identifier)) errors.Add($"{path}: item ID is required.");
            if (string.IsNullOrWhiteSpace(definition.DisplayName)) errors.Add($"{path}: display name is required.");
            if (definition.Width < 1 || definition.Height < 1) errors.Add($"{path}: width and height must be positive.");
            if (definition.MaxStack < 1) errors.Add($"{path}: maximum stack must be positive.");
            if (definition.Icon == null) errors.Add($"{path}: icon is required.");
        }
    }
}
