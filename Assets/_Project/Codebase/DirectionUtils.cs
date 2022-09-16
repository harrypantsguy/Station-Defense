using System;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public static class DirectionUtils
    {
        public static Vector2Int TransformVectorInDirection(this Direction dir, Vector2Int original) =>
            TransformVectorInDirection(dir, new Vector2(original.x, original.y)).ToInt();

        public static float DirectionToAngle(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return 0f;
                case Direction.Right:
                    return -90f;
                case Direction.Down:
                    return -180f;
                case Direction.Left:
                    return -270f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
        
        public static Direction Rotate(this Direction dir, int shift)
        {
            int shiftedValue = ((int) dir + shift) % 4;
            if (shiftedValue < 0)
                shiftedValue += 4;
            return (Direction)shiftedValue;
        }
        public static Vector2 TransformVectorInDirection(this Direction dir, Vector2 original)
        {
            switch (dir)
            {
                case Direction.Up:
                    return original;
                case Direction.Right:
                    return -Vector2.Perpendicular(original);
                case Direction.Down:
                    return original.Flip();
                case Direction.Left:
                    return Vector2.Perpendicular(original);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}