using System;
namespace Pktony.GridInventory.Domain
{
    public readonly struct GridSectionId : IEquatable<GridSectionId>
    {
        public string Value { get; }
        public GridSectionId(string value) { Value = value ?? string.Empty; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);
        public bool Equals(GridSectionId other) => string.Equals(Value ?? string.Empty, other.Value ?? string.Empty, StringComparison.Ordinal);
        public override bool Equals(object value) => value is GridSectionId other && Equals(other);
        public override int GetHashCode() => (Value ?? string.Empty).GetHashCode();
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(GridSectionId left, GridSectionId right) => left.Equals(right);
        public static bool operator !=(GridSectionId left, GridSectionId right) => !left.Equals(right);
    }
}
