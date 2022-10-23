using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class Spacecraft : MonoBehaviour
    {
        public Rigidbody2D Rb { get; private set; }

        [SerializeField] protected float maxMoveSpeed;
        [SerializeField] protected float maxRotationSpeed;
        [SerializeField] protected List<SpacecraftPart> parts = new List<SpacecraftPart>();
        [HideInInspector] public List<Thruster> thrusters = new List<Thruster>();
        
        protected float MaxMoveSpeed => maxMoveSpeed;

        private float _turnVelocity;
        private Vector2 _moveVelocity;
        private float _moveSpeed;
        
        public Vector2 targetPosition;
        
        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void OrientTorwardsTarget()
        {
            Vector2 dirToTarget = targetPosition - (Vector2)transform.position;
            float desiredAngleChange = -Vector2.SignedAngle(dirToTarget, transform.up);
            float absAngleChange = Mathf.Abs(desiredAngleChange);
            
            float turnSlowDownRange = 15f;
            float angleDifferenceTurnSpeedMultiplier = (absAngleChange > turnSlowDownRange
                ? 1f
                : absAngleChange / turnSlowDownRange) * Mathf.Sign(desiredAngleChange);
            
            Debug.Log($"desired angle change: {desiredAngleChange}, multiplier: {angleDifferenceTurnSpeedMultiplier}");

            _turnVelocity = Mathf.Lerp(_turnVelocity, angleDifferenceTurnSpeedMultiplier * maxRotationSpeed, 
                10f * Time.deltaTime);
            Rb.angularVelocity = _turnVelocity;

            float moveAngleDifferenceRequirement = Utils.Remap(Mathf.Min(dirToTarget.magnitude, 6f), 
                0f, 6f, 5f, 60f);
            float slowDownDist = Rb.velocity.magnitude * 2f;
            float stopDist = .15f;
            
            float proximitySlowDownMultiplier = 1f;
            if (dirToTarget.magnitude < slowDownDist)
                proximitySlowDownMultiplier = dirToTarget.magnitude / slowDownDist;
            else if (dirToTarget.magnitude < stopDist)
                proximitySlowDownMultiplier = 0f;

            float desiredSpeed = (absAngleChange < moveAngleDifferenceRequirement ? MaxMoveSpeed : 0f) *
                                 proximitySlowDownMultiplier;
            
            _moveSpeed = Mathf.Lerp(_moveSpeed, desiredSpeed, 10f * Time.deltaTime);

            _moveVelocity = transform.up * _moveSpeed;
            Rb.velocity = _moveVelocity;
        }

        protected virtual void Update()
        {
            OrientTorwardsTarget();
        }
    }
}