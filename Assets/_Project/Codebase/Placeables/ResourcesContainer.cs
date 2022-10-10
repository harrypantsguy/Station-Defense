using System;

namespace _Project.Codebase
{
    [Serializable]
    public struct ResourcesContainer
    {
        public int credits;
        
        public ResourcesContainer(int credits)
        {
            this.credits = credits;
        }

        public override string ToString() => $"{{credits: {credits}}}";

        public bool EqualsZero() => credits == 0;

        public static ResourcesContainer operator +(ResourcesContainer a, ResourcesContainer b) => 
            new ResourcesContainer(a.credits + b.credits);
        public static ResourcesContainer operator -(ResourcesContainer a, ResourcesContainer b) => 
            new ResourcesContainer(a.credits - b.credits);
        public static ResourcesContainer operator *(ResourcesContainer a, ResourcesContainer b) => 
            new ResourcesContainer(a.credits * b.credits);
        public static ResourcesContainer operator *(ResourcesContainer a, int n) => 
            new ResourcesContainer(a.credits * n);

        public static bool operator >(ResourcesContainer a, ResourcesContainer b) => a.credits > b.credits;
        public static bool operator >=(ResourcesContainer a, ResourcesContainer b) => a.credits >= b.credits;
        public static bool operator <(ResourcesContainer a, ResourcesContainer b) => a.credits < b.credits;
        public static bool operator <=(ResourcesContainer a, ResourcesContainer b) => a.credits <= b.credits;
        public static bool operator ==(ResourcesContainer a, ResourcesContainer b) => a.credits == b.credits;
        public static bool operator !=(ResourcesContainer a, ResourcesContainer b) => !(a == b);
    }
}