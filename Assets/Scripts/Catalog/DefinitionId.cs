using System;
namespace InventorySystem
{
    public readonly struct DefinitionId : IEquatable<DefinitionId>
    {
        public string Value { get; }
        public DefinitionId(string value) => Value = value ?? string.Empty;
        public bool Equals(DefinitionId other) => StringComparer.Ordinal.Equals(Value ?? string.Empty, other.Value ?? string.Empty);
        public override bool Equals(object obj) => obj is DefinitionId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(DefinitionId left, DefinitionId right) => left.Equals(right);
        public static bool operator !=(DefinitionId left, DefinitionId right) => !left.Equals(right);
    }
}
