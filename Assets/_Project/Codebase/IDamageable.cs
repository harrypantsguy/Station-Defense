namespace _Project.Codebase
{
    public interface IDamageable
    {
        public int Health { get; set; }
        
        public DamageReport TakeDamage(DamageReport damage);
        public void Die();
    }
}