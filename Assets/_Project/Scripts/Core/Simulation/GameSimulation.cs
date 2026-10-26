using System;
using System.Collections.Generic;
using EisernerVorhang.Core.Commands;
using EisernerVorhang.Core.Geometry;
using EisernerVorhang.Core.Units;

namespace EisernerVorhang.Core.Simulation
{
    /// <summary>
    /// Herz der Spiellogik. Kennt keine Grafik und kein Unity, nur Takte, Befehle und Einheiten.
    /// </summary>
    public sealed class GameSimulation
    {
        private readonly CommandQueue _commands = new CommandQueue();

        public long CurrentTick { get; private set; }
        public UnitWorld World { get; } = new UnitWorld();

        /// <summary>Wird für jeden ausgeführten Befehl ausgelöst.</summary>
        public event Action<GameCommand> CommandExecuted;

        /// <summary>Wird nach jedem abgeschlossenen Takt ausgelöst.</summary>
        public event Action<long> TickCompleted;

        /// <summary>Plant einen Befehl ein. Er wird in seinem Ausführungstakt abgearbeitet.</summary>
        public void Submit(GameCommand command) => _commands.Enqueue(command);

        public long NextTick => CurrentTick + 1;

        public void Step()
        {
            CurrentTick++;

            foreach (var command in _commands.DequeueUpTo(CurrentTick))
            {
                Execute(command);
                CommandExecuted?.Invoke(command);
            }

            World.Tick();
            TickCompleted?.Invoke(CurrentTick);
        }

        private void Execute(GameCommand command)
        {
            var units = OwnedUnits(command);

            switch (command.Type)
            {
                case CommandType.Move:
                    var target = new IntVector2(command.TargetX, command.TargetZ);
                    var slots = Formation.Grid(target, units.Count);
                    for (int i = 0; i < units.Count; i++)
                        World.OrderMove(units[i], slots[i]);
                    break;

                case CommandType.Stop:
                    foreach (var unit in units)
                        World.Stop(unit);
                    break;
            }
        }

        /// <summary>Nur eigene Einheiten folgen einem Befehl, sortiert nach ID.</summary>
        private List<Unit> OwnedUnits(GameCommand command)
        {
            var result = new List<Unit>(command.UnitIds.Length);
            foreach (int id in command.UnitIds)
            {
                if (World.TryGet(id, out var unit) && unit.OwnerId == command.PlayerId && !result.Contains(unit))
                    result.Add(unit);
            }
            result.Sort((a, b) => a.Id.CompareTo(b.Id));
            return result;
        }
    }
}
