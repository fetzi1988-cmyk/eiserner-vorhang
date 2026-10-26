using System;

namespace EisernerVorhang.Core.Simulation
{
    /// <summary>
    /// Übersetzt die echte Bildrate in feste Simulationstakte.
    /// Die Spiellogik läuft immer mit <see cref="TicksPerSecond"/> Takten pro Sekunde,
    /// egal ob das Spiel mit 30 oder 144 Bildern pro Sekunde läuft.
    /// </summary>
    public sealed class SimulationClock
    {
        public const int TicksPerSecond = 15;
        public const double SecondsPerTick = 1.0 / TicksPerSecond;

        /// <summary>Obergrenze pro Bild, damit ein Ruckler das Spiel nicht einfriert.</summary>
        public int MaxTicksPerFrame { get; }

        private double _accumulator;

        public SimulationClock(int maxTicksPerFrame = 5)
        {
            if (maxTicksPerFrame < 1)
                throw new ArgumentOutOfRangeException(nameof(maxTicksPerFrame));
            MaxTicksPerFrame = maxTicksPerFrame;
        }

        /// <summary>Anteil bis zum nächsten Takt (0 bis 1), für flüssige Darstellung zwischen Takten.</summary>
        public float Interpolation => (float)(_accumulator / SecondsPerTick);

        /// <summary>Gibt zurück, wie viele Takte in diesem Bild simuliert werden sollen.</summary>
        public int Advance(double deltaSeconds)
        {
            if (deltaSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

            _accumulator += deltaSeconds;
            int ticks = (int)Math.Floor(_accumulator / SecondsPerTick);

            if (ticks > MaxTicksPerFrame)
            {
                ticks = MaxTicksPerFrame;
                _accumulator = 0;
            }
            else
            {
                _accumulator -= ticks * SecondsPerTick;
            }

            return ticks;
        }
    }
}
