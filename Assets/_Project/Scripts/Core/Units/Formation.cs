using System.Collections.Generic;
using EisernerVorhang.Core.Geometry;

namespace EisernerVorhang.Core.Units
{
    /// <summary>
    /// Verteilt eine Gruppe in einem Raster um den Zielpunkt, damit nicht alle
    /// Einheiten auf denselben Fleck fahren. Echte Formationen folgen in Phase 3.
    /// </summary>
    public static class Formation
    {
        public const int DefaultSpacing = 500; // 5 Meter

        public static List<IntVector2> Grid(IntVector2 center, int count, int spacing = DefaultSpacing)
        {
            var result = new List<IntVector2>(count);
            if (count <= 0)
                return result;

            int columns = (int)IntMath.Sqrt(count - 1) + 1;
            int rows = (count + columns - 1) / columns;
            int offsetX = (columns - 1) * spacing / 2;
            int offsetZ = (rows - 1) * spacing / 2;

            for (int i = 0; i < count; i++)
            {
                int column = i % columns;
                int row = i / columns;
                result.Add(new IntVector2(
                    center.X + column * spacing - offsetX,
                    center.Z - row * spacing + offsetZ));
            }
            return result;
        }
    }
}
