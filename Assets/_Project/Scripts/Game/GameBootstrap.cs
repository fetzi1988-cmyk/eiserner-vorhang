using EisernerVorhang.Core.Simulation;
using UnityEngine;

namespace EisernerVorhang.Game
{
    /// <summary>
    /// Verbindet Unity mit der Simulation: liest die Bildzeit und führt die passenden Takte aus.
    /// Auf ein leeres GameObject in der Szene ziehen, dann Play drücken.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Tooltip("Schreibt jede Sekunde den aktuellen Takt in die Konsole.")]
        [SerializeField] private bool logTicks = true;

        private readonly SimulationClock _clock = new SimulationClock();
        private GameSimulation _simulation;

        public GameSimulation Simulation => _simulation;

        private void Awake()
        {
            _simulation = new GameSimulation();
            _simulation.TickCompleted += OnTickCompleted;
        }

        private void OnDestroy()
        {
            if (_simulation != null)
                _simulation.TickCompleted -= OnTickCompleted;
        }

        private void Update()
        {
            int ticks = _clock.Advance(Time.deltaTime);
            for (int i = 0; i < ticks; i++)
                _simulation.Step();
        }

        private void OnTickCompleted(long tick)
        {
            if (logTicks && tick % SimulationClock.TicksPerSecond == 0)
                Debug.Log($"[Simulation] Takt {tick} ({tick / SimulationClock.TicksPerSecond} s)");
        }
    }
}
