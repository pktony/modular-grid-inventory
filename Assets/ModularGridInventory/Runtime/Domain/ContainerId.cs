using System;
namespace Pktony.GridInventory.Domain
{
    public readonly struct ContainerId : IEquatable<ContainerId>
    {
        public string Value { get; }
        public ContainerId(string value) { Value = value ?? string.Empty; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);
        public bool Equals(ContainerId other) => string.Equals(Value ?? string.Empty, other.Value ?? string.Empty, StringComparison.Ordinal);
        public override bool Equals(object value) => value is ContainerId other && Equals(other);
        public override int GetHashCode() => (Value ?? string.Empty).GetHashCode();
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(ContainerId left, ContainerId right) => left.Equals(right);
        public static bool operator !=(ContainerId left, ContainerId right) => !left.Equals(right);
    }
}
