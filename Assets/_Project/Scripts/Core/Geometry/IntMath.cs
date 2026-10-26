using System;

namespace EisernerVorhang.Core.Geometry
{
    /// <summary>Deterministische Ganzzahl-Mathematik ohne Gleitkomma.</summary>
    public static class IntMath
    {
        /// <summary>Ganzzahlige Quadratwurzel, abgerundet.</summary>
        public static long Sqrt(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (value < 2)
                return value;

            long x = value;
            long y = (x + 1) / 2;
            while (y < x)
            {
                x = y;
                y = (x + value / x) / 2;
            }
            return x;
        }
    }
}
