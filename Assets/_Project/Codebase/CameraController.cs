using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class CameraController : MonoSingleton<CameraController>
    {
        public Vector2 targetPos;
        public Camera camera;

        private float _targetZoom;

        private const float MIN_SIZE = 2f;
        private const float MAX_SIZE = 22.5f;

        private const float TARGET_MOVE_SPEED = 8f; 
        private const float TARGET_FAST_MOVE_SPEED = 14f; 
        private const float CAMERA_MOVE_SPEED = 15f; 
        private const float CAMERA_ZOOM_SPEED = 3f;
        private const float CAMERA_ZOOM_LERP_SPEED = 5f;

        private void Start()
        {
            camera = GetComponent<Camera>();
            _targetZoom = camera.orthographicSize;
        }

        private void Update()
        {
            _targetZoom = Mathf.Clamp(_targetZoom + Input.mouseScrollDelta.y * -CAMERA_ZOOM_SPEED, 
                MIN_SIZE, MAX_SIZE);
            camera.orthographicSize =
                Mathf.Lerp(camera.orthographicSize, _targetZoom, CAMERA_ZOOM_LERP_SPEED * Time.deltaTime);

            float targetMoveSpeed = GameControls.FastMoveCamera.IsHeld ? TARGET_FAST_MOVE_SPEED : TARGET_MOVE_SPEED;
            
            targetPos += GameControls.DirectionalInput * (Time.deltaTime * targetMoveSpeed);
            
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 
                Time.deltaTime * CAMERA_MOVE_SPEED);
            transform.position = transform.position.SetZ(-10f);
        }
    }
}