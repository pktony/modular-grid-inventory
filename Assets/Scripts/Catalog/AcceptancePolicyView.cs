using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem
{
    public enum AcceptanceMode { AllowAll, AllowListed }
    public sealed class AcceptancePolicyView
    {
        public AcceptanceMode Mode { get; }
        public IReadOnlyList<string> AllowedCategories { get; }
        public IReadOnlyList<string> DeniedCategories { get; }
        public IReadOnlyList<DefinitionId> AllowedItems { get; }
        public IReadOnlyList<DefinitionId> DeniedItems { get; }
        public static AcceptancePolicyView All { get; } = new(AcceptanceMode.AllowAll);
        public AcceptancePolicyView(AcceptanceMode mode, IEnumerable<string> allowedCategories = null,
            IEnumerable<string> deniedCategories = null, IEnumerable<DefinitionId> allowedItems = null,
            IEnumerable<DefinitionId> deniedItems = null)
        {
            Mode = mode;
            AllowedCategories = Array.AsReadOnly((allowedCategories ?? Array.Empty<string>()).ToArray());
            DeniedCategories = Array.AsReadOnly((deniedCategories ?? Array.Empty<string>()).ToArray());
            AllowedItems = Array.AsReadOnly((allowedItems ?? Array.Empty<DefinitionId>()).ToArray());
            DeniedItems = Array.AsReadOnly((deniedItems ?? Array.Empty<DefinitionId>()).ToArray());
        }
    }
}
