using System;
using UnityEngine;

namespace _Project.Codebase
{
    public class SpacecraftPart : MonoBehaviour, IDamageable
    {
        public int Health { get; set; }
        private Spacecraft _spacecraft;
        
        private void Start()
        {
            _spacecraft = GetComponentInParent<Spacecraft>();
        }

        public DamageReport TakeDamage(DamageReport damage)
        {
            Health = Math.Max(Health - damage.damage, 0);
            return damage;
        }

        public void Die()
        {
            
        }
    }
}