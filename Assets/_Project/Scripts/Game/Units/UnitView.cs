using EisernerVorhang.Core.Geometry;
using EisernerVorhang.Core.Units;
using UnityEngine;

namespace EisernerVorhang.Game.Units
{
    /// <summary>
    /// Zeigt eine Einheit der Simulation an. Zwischen zwei Takten wird die Position
    /// weich überblendet, damit die Bewegung trotz 15 Takten pro Sekunde flüssig aussieht.
    /// </summary>
    public sealed class UnitView : MonoBehaviour
    {
        private static readonly Color SelectionColor = new Color(0.4f, 1f, 0.4f);

        public Unit Unit { get; private set; }
        public bool IsSelected { get; private set; }

        private Vector3 _previous;
        private Vector3 _current;
        private GameObject _selectionMarker;

        public void Bind(Unit unit, GameObject selectionMarker)
        {
            Unit = unit;
            _selectionMarker = selectionMarker;
            _selectionMarker.SetActive(false);
            _previous = _current = ToWorld(unit.Position);
            transform.position = _current;
        }

        /// <summary>Nach jedem Simulationstakt aufrufen.</summary>
        public void OnSimulationTick()
        {
            _previous = _current;
            _current = ToWorld(Unit.Position);

            var direction = _current - _previous;
            if (direction.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        /// <summary>Jedes Bild aufrufen, <paramref name="t"/> ist der Anteil bis zum nächsten Takt.</summary>
        public void Render(float t)
        {
            var position = Vector3.Lerp(_previous, _current, t);
            position.y = transform.position.y;
            transform.position = position;
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (_selectionMarker != null)
                _selectionMarker.SetActive(selected);
        }

        public static Color MarkerColor => SelectionColor;

        /// <summary>Simulation rechnet in Zentimetern, Unity in Metern.</summary>
        public static Vector3 ToWorld(IntVector2 position) =>
            new Vector3(position.X / 100f, 0f, position.Z / 100f);

        public static IntVector2 ToSimulation(Vector3 world) =>
            new IntVector2(Mathf.RoundToInt(world.x * 100f), Mathf.RoundToInt(world.z * 100f));
    }
}
