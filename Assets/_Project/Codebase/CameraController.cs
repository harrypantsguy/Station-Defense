using FishingGame.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class CameraController : MonoSingleton<CameraController>
    {
        public Vector2 targetPos;
        public Camera camera;

        private const float MIN_SIZE = 2f;
        private const float MAX_SIZE = 15f;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + Input.mouseScrollDelta.y * -4f, 
                MIN_SIZE, MAX_SIZE);
            
            targetPos += GameControls.DirectionalInput * (Time.deltaTime * 8f);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 
                Time.deltaTime * 10f);
            transform.position = transform.position.SetZ(-10f);
        }
    }
}