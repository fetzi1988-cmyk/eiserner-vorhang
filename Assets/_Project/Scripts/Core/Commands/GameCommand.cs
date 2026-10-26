using System;

namespace EisernerVorhang.Core.Commands
{
    /// <summary>
    /// Ein Befehl als reiner Datensatz. Spieler, KI und später das Netzwerk
    /// erzeugen alle diese Struktur, die Simulation führt sie im angegebenen Takt aus.
    /// Positionen sind ganze Zahlen in Zentimetern, damit die Simulation
    /// auf jedem Rechner exakt gleich rechnet (Voraussetzung für Lockstep).
    /// </summary>
    public readonly struct GameCommand
    {
        public readonly int PlayerId;
        public readonly long ExecuteOnTick;
        public readonly CommandType Type;
        public readonly int[] UnitIds;
        public readonly int TargetX;
        public readonly int TargetZ;
        public readonly int TargetEntityId;

        public GameCommand(int playerId, long executeOnTick, CommandType type, int[] unitIds,
            int targetX = 0, int targetZ = 0, int targetEntityId = -1)
        {
            PlayerId = playerId;
            ExecuteOnTick = executeOnTick;
            Type = type;
            UnitIds = unitIds ?? Array.Empty<int>();
            TargetX = targetX;
            TargetZ = targetZ;
            TargetEntityId = targetEntityId;
        }
    }
}
