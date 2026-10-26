using System;

namespace EisernerVorhang.Core.Geometry
{
    /// <summary>
    /// Position auf der Karte in Zentimetern (X nach Osten, Z nach Norden).
    /// Ganze Zahlen statt float, damit alle Rechner exakt dasselbe Ergebnis bekommen.
    /// </summary>
    public readonly struct IntVector2 : IEquatable<IntVector2>
    {
        public readonly int X;
        public readonly int Z;

        public IntVector2(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static readonly IntVector2 Zero = new IntVector2(0, 0);

        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.X + b.X, a.Z + b.Z);
        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.X - b.X, a.Z - b.Z);
        public static bool operator ==(IntVector2 a, IntVector2 b) => a.Equals(b);
        public static bool operator !=(IntVector2 a, IntVector2 b) => !a.Equals(b);

        public long SqrMagnitude => (long)X * X + (long)Z * Z;

        /// <summary>Länge in Zentimetern, abgerundet.</summary>
        public int Magnitude => (int)IntMath.Sqrt(SqrMagnitude);

        public bool Equals(IntVector2 other) => X == other.X && Z == other.Z;
        public override bool Equals(object obj) => obj is IntVector2 other && Equals(other);
        public override int GetHashCode() => (X * 397) ^ Z;
        public override string ToString() => $"({X}, {Z})";
    }
}
