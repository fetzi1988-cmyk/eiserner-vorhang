using System;
using EisernerVorhang.Core.Commands;

namespace EisernerVorhang.Core.Simulation
{
    /// <summary>
    /// Herz der Spiellogik. Kennt keine Grafik und kein Unity, nur Takte und Befehle.
    /// In den nächsten Phasen kommen hier Einheiten, Kampf und Wirtschaft dazu.
    /// </summary>
    public sealed class GameSimulation
    {
        private readonly CommandQueue _commands = new CommandQueue();

        public long CurrentTick { get; private set; }

        /// <summary>Wird für jeden ausgeführten Befehl ausgelöst.</summary>
        public event Action<GameCommand> CommandExecuted;

        /// <summary>Wird nach jedem abgeschlossenen Takt ausgelöst.</summary>
        public event Action<long> TickCompleted;

        /// <summary>Plant einen Befehl ein. Ohne Taktangabe wird er im nächsten Takt ausgeführt.</summary>
        public void Submit(GameCommand command) => _commands.Enqueue(command);

        public long NextTick => CurrentTick + 1;

        public void Step()
        {
            CurrentTick++;

            foreach (var command in _commands.DequeueUpTo(CurrentTick))
                CommandExecuted?.Invoke(command);

            TickCompleted?.Invoke(CurrentTick);
        }
    }
}
