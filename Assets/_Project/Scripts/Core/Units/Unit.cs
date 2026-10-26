using EisernerVorhang.Core.Geometry;

namespace EisernerVorhang.Core.Units
{
    /// <summary>Zustand einer Einheit in der Simulation.</summary>
    public sealed class Unit
    {
        public int Id { get; }
        public int OwnerId { get; }

        /// <summary>Strecke pro Simulationstakt in Zentimetern.</summary>
        public int SpeedPerTick { get; }

        public IntVector2 Position { get; internal set; }
        public IntVector2 MoveTarget { get; internal set; }
        public bool IsMoving { get; internal set; }

        internal Unit(int id, int ownerId, IntVector2 position, int speedPerTick)
        {
            Id = id;
            OwnerId = ownerId;
            Position = position;
            SpeedPerTick = speedPerTick;
        }
    }
}
