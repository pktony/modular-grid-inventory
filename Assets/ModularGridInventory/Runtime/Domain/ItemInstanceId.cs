using System;
namespace Pktony.GridInventory.Domain
{
    public readonly struct ItemInstanceId : IEquatable<ItemInstanceId>
    {
        public string Value { get; }
        public ItemInstanceId(string value) { Value = value ?? string.Empty; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);
        public bool Equals(ItemInstanceId other) => string.Equals(Value ?? string.Empty, other.Value ?? string.Empty, StringComparison.Ordinal);
        public override bool Equals(object value) => value is ItemInstanceId other && Equals(other);
        public override int GetHashCode() => (Value ?? string.Empty).GetHashCode();
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(ItemInstanceId left, ItemInstanceId right) => left.Equals(right);
        public static bool operator !=(ItemInstanceId left, ItemInstanceId right) => !left.Equals(right);
    }
}
