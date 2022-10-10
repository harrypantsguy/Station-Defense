using UnityEngine;

namespace _Project.Codebase
{
    public static class Layers
    {
        public static readonly LayerMask WorldMask = 1 << 6;
        public static readonly LayerMask EnemyMask = 1 << 8;
    }
}