using System;
using System.Collections.Generic;
using System.Linq;
namespace Pktony.GridInventory
{
    public sealed class CatalogValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }
        public CatalogValidationException(IReadOnlyList<string> errors)
            : base("Invalid item catalog:\n" + string.Join("\n", errors)) => Errors = Array.AsReadOnly(errors.ToArray());
    }
}
