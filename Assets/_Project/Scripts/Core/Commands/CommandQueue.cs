using System;
using System.Collections.Generic;

namespace EisernerVorhang.Core.Commands
{
    /// <summary>
    /// Sammelt Befehle bis zu ihrem Ausführungstakt. Die Reihenfolge innerhalb
    /// eines Takts ist fest (Spieler-ID, dann Eingangsreihenfolge), damit alle
    /// Rechner dieselben Befehle in derselben Reihenfolge ausführen.
    /// </summary>
    public sealed class CommandQueue
    {
        private readonly SortedDictionary<long, List<Entry>> _byTick = new SortedDictionary<long, List<Entry>>();
        private long _sequence;

        public int Count { get; private set; }

        public void Enqueue(GameCommand command)
        {
            if (!_byTick.TryGetValue(command.ExecuteOnTick, out var list))
            {
                list = new List<Entry>();
                _byTick.Add(command.ExecuteOnTick, list);
            }

            list.Add(new Entry(command, _sequence++));
            Count++;
        }

        /// <summary>
        /// Entnimmt alle Befehle, die bis einschließlich <paramref name="tick"/> fällig sind.
        /// Verspätete Befehle aus früheren Takten werden mit ausgeführt.
        /// </summary>
        public List<GameCommand> DequeueUpTo(long tick)
        {
            var result = new List<GameCommand>();
            var dueTicks = new List<long>();

            foreach (var pair in _byTick)
            {
                if (pair.Key > tick)
                    break;

                var entries = pair.Value;
                entries.Sort(CompareEntries);
                foreach (var entry in entries)
                    result.Add(entry.Command);

                dueTicks.Add(pair.Key);
            }

            foreach (var dueTick in dueTicks)
                _byTick.Remove(dueTick);

            Count -= result.Count;
            return result;
        }

        private static int CompareEntries(Entry a, Entry b)
        {
            int byPlayer = a.Command.PlayerId.CompareTo(b.Command.PlayerId);
            return byPlayer != 0 ? byPlayer : a.Sequence.CompareTo(b.Sequence);
        }

        private readonly struct Entry
        {
            public readonly GameCommand Command;
            public readonly long Sequence;

            public Entry(GameCommand command, long sequence)
            {
                Command = command;
                Sequence = sequence;
            }
        }
    }
}
