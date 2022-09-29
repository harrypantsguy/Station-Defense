using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class CameraController : MonoSingleton<CameraController>
    {
        public Vector2 targetPos;
        [field: SerializeField] public Canvas ScreenSpaceCanvas { get; private set; }
        public Camera Camera { get; private set; }
        public float OrthographicSizeRatio => _startOrthographicSize / Camera.orthographicSize;

        private float _targetZoom;
        private float _startOrthographicSize;

        private const float MIN_SIZE = 2f;
        private const float MAX_SIZE = 22.5f;

        private const float TARGET_MOVE_SPEED = 8f; 
        private const float TARGET_FAST_MOVE_SPEED = 14f; 
        private const float CAMERA_MOVE_SPEED = 15f; 
        private const float CAMERA_ZOOM_SPEED = 3f;
        private const float CAMERA_ZOOM_LERP_SPEED = 5f;

        protected override void Awake()
        {
            base.Awake();
            
            Camera = GetComponent<Camera>();
            _targetZoom = Camera.orthographicSize;
            _startOrthographicSize = Camera.orthographicSize;
        }

        private void Update()
        {
            _targetZoom = Mathf.Clamp(_targetZoom + Input.mouseScrollDelta.y * -CAMERA_ZOOM_SPEED, 
                MIN_SIZE, MAX_SIZE);
            Camera.orthographicSize =
                Mathf.Lerp(Camera.orthographicSize, _targetZoom, CAMERA_ZOOM_LERP_SPEED * Time.deltaTime);

            float targetMoveSpeed = GameControls.FastMoveCamera.IsHeld ? TARGET_FAST_MOVE_SPEED : TARGET_MOVE_SPEED;
            
            targetPos += GameControls.DirectionalInput * (Time.deltaTime * targetMoveSpeed);
            
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 
                Time.deltaTime * CAMERA_MOVE_SPEED);
            transform.position = transform.position.SetZ(-10f);
        }
    }
}