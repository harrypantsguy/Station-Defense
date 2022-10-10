using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    public class ObjectSegment : MonoBehaviour
    {
        [SerializeField] private List<ObjectSegment> _children = new List<ObjectSegment>();

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnDestroy()
        {
            foreach (ObjectSegment child in _children)
            {
                child._rb.bodyType = RigidbodyType2D.Dynamic;
                child._rb.AddForce((child._rb.position - (Vector2)transform.position) * .5f, ForceMode2D.Impulse);
                child._rb.AddTorque(Random.Range(-.1f, .1f), ForceMode2D.Impulse);
            }
        }
    }
}