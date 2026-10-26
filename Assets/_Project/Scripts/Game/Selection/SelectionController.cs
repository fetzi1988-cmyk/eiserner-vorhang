using System.Collections.Generic;
using System.Linq;
using EisernerVorhang.Core.Commands;
using EisernerVorhang.Game.Units;
using UnityEngine;

namespace EisernerVorhang.Game.Selection
{
    /// <summary>
    /// Auswahl und Befehle mit der Maus:
    /// Linksklick wählt eine Einheit, Ziehen mit links zieht einen Auswahlrahmen,
    /// Shift fügt hinzu, Rechtsklick auf den Boden schickt die Auswahl dorthin,
    /// X stoppt. Strg+1..9 speichert eine Gruppe, 1..9 ruft sie ab.
    /// </summary>
    public sealed class SelectionController : MonoBehaviour
    {
        private const float DragThreshold = 6f;
        private const float ClickRadius = 30f;

        private GameBootstrap _game;
        private Camera _camera;
        private int _playerId;

        private readonly List<UnitView> _selection = new List<UnitView>();
        private readonly Dictionary<int, List<int>> _groups = new Dictionary<int, List<int>>();

        private bool _mouseDown;
        private Vector2 _dragStart;
        private Texture2D _pixel;

        public IReadOnlyList<UnitView> Selection => _selection;

        public void Init(GameBootstrap game, Camera cam, int playerId)
        {
            _game = game;
            _camera = cam;
            _playerId = playerId;
        }

        private void Update()
        {
            if (_game == null)
                return;

            HandleSelectionInput();
            HandleOrders();
            HandleGroups();
        }

        private void HandleSelectionInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDown = true;
                _dragStart = Input.mousePosition;
            }

            if (!Input.GetMouseButtonUp(0) || !_mouseDown)
                return;

            _mouseDown = false;
            bool additive = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (!additive)
                ClearSelection();

            Vector2 end = Input.mousePosition;
            if (Vector2.Distance(_dragStart, end) < DragThreshold)
                SelectClosestTo(end);
            else
                SelectInRect(ScreenRect(_dragStart, end));
        }

        private void HandleOrders()
        {
            if (_selection.Count == 0)
                return;

            if (Input.GetMouseButtonDown(1) && TryGetGroundPoint(Input.mousePosition, out var point))
            {
                var target = UnitView.ToSimulation(point);
                _game.Simulation.Submit(new GameCommand(
                    _playerId, _game.Simulation.NextTick, CommandType.Move, SelectedIds(),
                    target.X, target.Z));
                _game.ShowMoveMarker(point);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _game.Simulation.Submit(new GameCommand(
                    _playerId, _game.Simulation.NextTick, CommandType.Stop, SelectedIds()));
            }
        }

        private void HandleGroups()
        {
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            for (int number = 1; number <= 9; number++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha0 + number))
                    continue;

                if (ctrl)
                {
                    _groups[number] = SelectedIds().ToList();
                }
                else if (_groups.TryGetValue(number, out var ids))
                {
                    ClearSelection();
                    foreach (var view in _game.UnitViews)
                        if (view != null && ids.Contains(view.Unit.Id))
                            Select(view);
                }
            }
        }

        private void SelectClosestTo(Vector2 screenPoint)
        {
            UnitView best = null;
            float bestDistance = ClickRadius;
            foreach (var view in OwnViews())
            {
                var screen = _camera.WorldToScreenPoint(view.transform.position);
                if (screen.z < 0f)
                    continue;
                float distance = Vector2.Distance(screenPoint, screen);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = view;
                }
            }

            if (best != null)
                Select(best);
        }

        private void SelectInRect(Rect rect)
        {
            foreach (var view in OwnViews())
            {
                var screen = _camera.WorldToScreenPoint(view.transform.position);
                if (screen.z > 0f && rect.Contains(screen))
                    Select(view);
            }
        }

        private IEnumerable<UnitView> OwnViews() =>
            _game.UnitViews.Where(v => v != null && v.Unit.OwnerId == _playerId);

        private void Select(UnitView view)
        {
            if (_selection.Contains(view))
                return;
            _selection.Add(view);
            view.SetSelected(true);
        }

        private void ClearSelection()
        {
            foreach (var view in _selection)
                if (view != null)
                    view.SetSelected(false);
            _selection.Clear();
        }

        private int[] SelectedIds() => _selection.Where(v => v != null).Select(v => v.Unit.Id).ToArray();

        private bool TryGetGroundPoint(Vector2 screenPoint, out Vector3 point)
        {
            var ray = _camera.ScreenPointToRay(screenPoint);
            var ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out float distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            point = default;
            return false;
        }

        private static Rect ScreenRect(Vector2 a, Vector2 b)
        {
            var min = Vector2.Min(a, b);
            var max = Vector2.Max(a, b);
            return new Rect(min, max - min);
        }

        private void OnGUI()
        {
            if (!_mouseDown)
                return;

            Vector2 current = Input.mousePosition;
            if (Vector2.Distance(_dragStart, current) < DragThreshold)
                return;

            if (_pixel == null)
            {
                _pixel = new Texture2D(1, 1);
                _pixel.SetPixel(0, 0, Color.white);
                _pixel.Apply();
            }

            // OnGUI zählt Y von oben, Input.mousePosition von unten.
            var rect = ScreenRect(_dragStart, current);
            rect.y = Screen.height - rect.y - rect.height;

            var previous = GUI.color;
            GUI.color = new Color(0.4f, 1f, 0.4f, 0.15f);
            GUI.DrawTexture(rect, _pixel);
            GUI.color = new Color(0.4f, 1f, 0.4f, 0.9f);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1), _pixel);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - 1, rect.width, 1), _pixel);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1, rect.height), _pixel);
            GUI.DrawTexture(new Rect(rect.xMax - 1, rect.yMin, 1, rect.height), _pixel);
            GUI.color = previous;
        }

        private void OnDestroy()
        {
            if (_pixel != null)
                Destroy(_pixel);
        }
    }
}
