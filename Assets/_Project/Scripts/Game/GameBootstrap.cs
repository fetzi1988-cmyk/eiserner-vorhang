using System.Collections.Generic;
using EisernerVorhang.Core.Units;
using EisernerVorhang.Core.Simulation;
using EisernerVorhang.Game.CameraControl;
using EisernerVorhang.Game.Selection;
using EisernerVorhang.Game.Units;
using UnityEngine;

namespace EisernerVorhang.Game
{
    /// <summary>
    /// Startpunkt des Spiels. Auf ein leeres GameObject in einer Szene ziehen und Play drücken:
    /// Es baut ein Testgelände mit Kamera und Einheiten auf und verbindet Unity mit der Simulation.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        public const int LocalPlayerId = 1;
        public const int EnemyPlayerId = 2;

        [Header("Testgelände")]
        [SerializeField] private float mapSize = 200f;
        [SerializeField] private int ownUnits = 12;
        [SerializeField] private int enemyUnits = 6;
        [Tooltip("Geschwindigkeit in Metern pro Sekunde.")]
        [SerializeField] private float unitSpeed = 9f;

        [Header("Farben")]
        [SerializeField] private Color groundColor = new Color(0.36f, 0.42f, 0.28f);
        [SerializeField] private Color ownColor = new Color(0.25f, 0.42f, 0.7f);
        [SerializeField] private Color enemyColor = new Color(0.68f, 0.24f, 0.2f);

        [Tooltip("Schreibt alle 10 Sekunden den aktuellen Takt in die Konsole.")]
        [SerializeField] private bool logTicks = true;

        private readonly SimulationClock _clock = new SimulationClock();
        private readonly List<UnitView> _views = new List<UnitView>();
        private GameSimulation _simulation;
        private Transform _moveMarker;
        private float _moveMarkerHideAt;

        public GameSimulation Simulation => _simulation;
        public IReadOnlyList<UnitView> UnitViews => _views;

        private void Awake()
        {
            _simulation = new GameSimulation();
            _simulation.TickCompleted += OnTickCompleted;

            var cam = EnsureCamera();
            EnsureLight();
            CreateGround();
            SpawnUnits(LocalPlayerId, ownUnits, new Vector3(-40f, 0f, -40f), ownColor);
            SpawnUnits(EnemyPlayerId, enemyUnits, new Vector3(40f, 0f, 40f), enemyColor);
            CreateMoveMarker();

            var rig = new GameObject("Kamera-Drehpunkt").AddComponent<RtsCameraController>();
            rig.transform.position = new Vector3(-40f, 0f, -40f);
            float half = mapSize / 2f;
            rig.SetBounds(new Vector2(-half, -half), new Vector2(half, half));
            rig.Attach(cam);

            gameObject.AddComponent<SelectionController>().Init(this, cam, LocalPlayerId);
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

            float t = _clock.Interpolation;
            foreach (var view in _views)
                view.Render(t);

            if (_moveMarker.gameObject.activeSelf && Time.time >= _moveMarkerHideAt)
                _moveMarker.gameObject.SetActive(false);
        }

        public void ShowMoveMarker(Vector3 point)
        {
            _moveMarker.position = new Vector3(point.x, 0.05f, point.z);
            _moveMarker.gameObject.SetActive(true);
            _moveMarkerHideAt = Time.time + 0.6f;
        }

        private void OnTickCompleted(long tick)
        {
            foreach (var view in _views)
                view.OnSimulationTick();

            if (logTicks && tick % (SimulationClock.TicksPerSecond * 10) == 0)
                Debug.Log($"[Simulation] Takt {tick} ({tick / SimulationClock.TicksPerSecond} s)");
        }

        private void SpawnUnits(int ownerId, int count, Vector3 center, Color color)
        {
            int speedPerTick = Mathf.RoundToInt(unitSpeed * 100f / SimulationClock.TicksPerSecond);
            var slots = Formation.Grid(UnitView.ToSimulation(center), count, 800);

            foreach (var slot in slots)
            {
                var unit = _simulation.World.Spawn(ownerId, slot, speedPerTick);

                // Platzhalter-Panzer: Wanne und Turm aus Würfeln, etwa 3,5 x 7 Meter.
                var root = new GameObject($"Einheit {unit.Id} (Spieler {ownerId})");
                var hull = CreatePrimitive(PrimitiveType.Cube, root.transform, color);
                hull.localScale = new Vector3(3.5f, 1.4f, 7f);
                hull.localPosition = new Vector3(0f, 0.7f, 0f);
                var turret = CreatePrimitive(PrimitiveType.Cube, root.transform, color * 0.8f);
                turret.localScale = new Vector3(2.2f, 0.9f, 3f);
                turret.localPosition = new Vector3(0f, 1.85f, -0.3f);
                var barrel = CreatePrimitive(PrimitiveType.Cube, root.transform, color * 0.6f);
                barrel.localScale = new Vector3(0.3f, 0.3f, 4f);
                barrel.localPosition = new Vector3(0f, 1.85f, 2.8f);

                var marker = CreatePrimitive(PrimitiveType.Cylinder, root.transform, UnitView.MarkerColor);
                marker.localScale = new Vector3(8.5f, 0.02f, 8.5f);
                marker.localPosition = new Vector3(0f, 0.03f, 0f);

                var view = root.AddComponent<UnitView>();
                view.Bind(unit, marker.gameObject);
                _views.Add(view);
            }
        }

        private void CreateGround()
        {
            var ground = CreatePrimitive(PrimitiveType.Plane, null, groundColor);
            ground.name = "Boden";
            ground.localScale = new Vector3(mapSize / 10f, 1f, mapSize / 10f);
        }

        private void CreateMoveMarker()
        {
            _moveMarker = CreatePrimitive(PrimitiveType.Cylinder, null, new Color(1f, 0.85f, 0.3f));
            _moveMarker.name = "Zielmarkierung";
            _moveMarker.localScale = new Vector3(2f, 0.02f, 2f);
            _moveMarker.gameObject.SetActive(false);
        }

        private static Transform CreatePrimitive(PrimitiveType type, Transform parent, Color color)
        {
            var go = GameObject.CreatePrimitive(type);
            Destroy(go.GetComponent<Collider>());
            go.GetComponent<Renderer>().material.color = color;
            if (parent != null)
                go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static Camera EnsureCamera()
        {
            var cam = Camera.main;
            if (cam != null)
                return cam;

            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.AddComponent<AudioListener>();
            return go.AddComponent<Camera>();
        }

        private static void EnsureLight()
        {
            if (FindAnyObjectByType<Light>() != null)
                return;

            var light = new GameObject("Sonne").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
