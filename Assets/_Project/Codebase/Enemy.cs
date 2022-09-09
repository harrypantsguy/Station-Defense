using UnityEngine;

namespace _Project.Codebase
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private GameObject _projectilePrefab;

        private float _lastFireTime;

        private void Start()
        {
            _lastFireTime = Time.time;
        }

        private void Update()
        {
            if (Time.time > _lastFireTime + 1.5f)
            {
                _lastFireTime = Time.time;
                
                GameObject newProjectile = Instantiate(_projectilePrefab);
                newProjectile.transform.position = transform.position;
                newProjectile.transform.right = transform.up;
            }
            
            float timeSample = Time.time * Mathf.PI / 5f;
            float radius = 20f;
            transform.position =
                new Vector2(Mathf.Cos(timeSample) * radius, Mathf.Sin(timeSample) * radius);
            transform.up = -transform.position;
        }
    }
}