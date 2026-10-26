using UnityEngine;

namespace EisernerVorhang.Game.CameraControl
{
    /// <summary>
    /// Strategiekamera: WASD oder Pfeiltasten (oder Maus an den Bildschirmrand) zum Verschieben,
    /// Mausrad zum Zoomen, Q und E oder gedrückte mittlere Maustaste zum Drehen.
    /// </summary>
    public sealed class RtsCameraController : MonoBehaviour
    {
        [Header("Bewegung")]
        [SerializeField] private float panSpeed = 40f;
        [SerializeField] private bool edgeScrolling = true;
        [SerializeField] private int edgeSize = 12;

        [Header("Zoom")]
        [SerializeField] private float minHeight = 15f;
        [SerializeField] private float maxHeight = 120f;
        [SerializeField] private float zoomSpeed = 25f;

        [Header("Drehung")]
        [SerializeField] private float rotateSpeed = 90f;
        [SerializeField] private float mouseRotateSpeed = 0.3f;
        [SerializeField] private float pitch = 55f;

        [Header("Kartengrenzen (Meter)")]
        [SerializeField] private Vector2 boundsMin = new Vector2(-100f, -100f);
        [SerializeField] private Vector2 boundsMax = new Vector2(100f, 100f);

        private Transform _camera;
        private float _height = 50f;
        private float _yaw;

        /// <summary>Hängt die Kamera an diesen Drehpunkt.</summary>
        public void Attach(Camera cam)
        {
            _camera = cam.transform;
            _camera.SetParent(transform, false);
            ApplyCameraOffset();
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            boundsMin = min;
            boundsMax = max;
        }

        private void Update()
        {
            if (_camera == null)
                return;

            HandleRotation();
            HandlePan();
            HandleZoom();
            ApplyCameraOffset();
        }

        private void HandlePan()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (edgeScrolling && Application.isFocused && input == Vector2.zero)
            {
                var mouse = Input.mousePosition;
                bool insideWindow = mouse.x >= 0 && mouse.y >= 0 && mouse.x <= Screen.width && mouse.y <= Screen.height;
                if (insideWindow)
                {
                    if (mouse.x <= edgeSize) input.x = -1f;
                    else if (mouse.x >= Screen.width - edgeSize) input.x = 1f;
                    if (mouse.y <= edgeSize) input.y = -1f;
                    else if (mouse.y >= Screen.height - edgeSize) input.y = 1f;
                }
            }

            if (input == Vector2.zero)
                return;

            // Schneller verschieben, wenn weit herausgezoomt.
            float speed = panSpeed * (_height / 50f);
            var forward = Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;
            var right = Quaternion.Euler(0f, _yaw, 0f) * Vector3.right;
            var position = transform.position + (forward * input.y + right * input.x) * speed * Time.unscaledDeltaTime;

            position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
            position.z = Mathf.Clamp(position.z, boundsMin.y, boundsMax.y);
            transform.position = position;
        }

        private void HandleZoom()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Approximately(scroll, 0f))
                return;

            _height = Mathf.Clamp(_height - scroll * zoomSpeed * (_height / 50f), minHeight, maxHeight);
        }

        private void HandleRotation()
        {
            if (Input.GetKey(KeyCode.Q)) _yaw -= rotateSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.E)) _yaw += rotateSpeed * Time.unscaledDeltaTime;
            if (Input.GetMouseButton(2)) _yaw += Input.GetAxis("Mouse X") * mouseRotateSpeed * 60f;

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }

        private void ApplyCameraOffset()
        {
            if (_camera == null)
                return;

            // Kamera schaut im festen Winkel auf den Drehpunkt am Boden.
            float distance = _height / Mathf.Tan(pitch * Mathf.Deg2Rad);
            _camera.localPosition = new Vector3(0f, _height, -distance);
            _camera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}
