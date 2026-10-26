using System;
using System.Collections.Generic;
using EisernerVorhang.Core.Geometry;

namespace EisernerVorhang.Core.Units
{
    /// <summary>
    /// Alle Einheiten der Partie. Wird immer in aufsteigender ID-Reihenfolge
    /// abgearbeitet, damit die Simulation deterministisch bleibt.
    /// </summary>
    public sealed class UnitWorld
    {
        private readonly List<Unit> _units = new List<Unit>();
        private readonly Dictionary<int, Unit> _byId = new Dictionary<int, Unit>();
        private int _nextId = 1;

        public IReadOnlyList<Unit> Units => _units;

        public Unit Spawn(int ownerId, IntVector2 position, int speedPerTick)
        {
            if (speedPerTick < 0)
                throw new ArgumentOutOfRangeException(nameof(speedPerTick));

            var unit = new Unit(_nextId++, ownerId, position, speedPerTick);
            _units.Add(unit);
            _byId.Add(unit.Id, unit);
            return unit;
        }

        public bool TryGet(int id, out Unit unit) => _byId.TryGetValue(id, out unit);

        public void OrderMove(Unit unit, IntVector2 target)
        {
            unit.MoveTarget = target;
            unit.IsMoving = unit.Position != target;
        }

        public void Stop(Unit unit)
        {
            unit.MoveTarget = unit.Position;
            unit.IsMoving = false;
        }

        /// <summary>Bewegt alle Einheiten einen Takt weiter, geradlinig zum Ziel.</summary>
        public void Tick()
        {
            foreach (var unit in _units)
            {
                if (!unit.IsMoving)
                    continue;

                var delta = unit.MoveTarget - unit.Position;
                int distance = delta.Magnitude;

                if (distance <= unit.SpeedPerTick)
                {
                    unit.Position = unit.MoveTarget;
                    unit.IsMoving = false;
                    continue;
                }

                int stepX = (int)((long)delta.X * unit.SpeedPerTick / distance);
                int stepZ = (int)((long)delta.Z * unit.SpeedPerTick / distance);
                unit.Position = new IntVector2(unit.Position.X + stepX, unit.Position.Z + stepZ);
            }
        }
    }
}
