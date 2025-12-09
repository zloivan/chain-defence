using System;
using UnityEngine;

namespace ChainDefense.GridSystem.core
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public GridPosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X { get; }
        public int Z { get; }

        public override bool Equals(object obj)
        {
            return obj is GridPosition position &&
                   X == position.X &&
                   Z == position.Z;
        }

        public override int GetHashCode() => HashCode.Combine(X, Z);
        public static bool operator ==(GridPosition a, GridPosition b) => a.X == b.X && a.Z == b.Z;
        public static bool operator !=(GridPosition a, GridPosition b) => !(a == b);
        public bool Equals(GridPosition other) => this == other;
        public static GridPosition operator +(GridPosition a, GridPosition b) => new(a.X + b.X, a.Z + b.Z);
        public static GridPosition operator -(GridPosition a, GridPosition b) => new(a.X - b.X, a.Z - b.Z);
        public static GridPosition operator *(GridPosition a, GridPosition b) => new(a.X * b.X, a.Z * b.Z);

        public static GridPosition operator *(GridPosition a, float b) =>
            new(Mathf.RoundToInt(a.X * b), Mathf.RoundToInt(a.Z * b));

        public static GridPosition operator *(GridPosition a, int b) => new(a.X * b, a.Z * b);
        public static GridPosition operator /(GridPosition a, GridPosition b) => new(a.X / b.X, a.Z / b.Z);
        public override string ToString() => $"X: {X}; Z: {Z};";
    }
}