namespace _Project.Codebase
{
    public interface IDestroyable
    {
        public float Health { get; set; }
        
        public void TakeDamage();
        public void Die();
    }
}