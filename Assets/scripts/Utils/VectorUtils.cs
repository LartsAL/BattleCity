using UnityEngine;

namespace Utils
{
    public static class VectorUtils
    {
        public static Vector2 RoundToCardinal(Vector2 vector)
        {
            vector = vector.normalized;

            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };

            float maxDot = -Mathf.Infinity;
            Vector2 closestDirection = Vector2.zero;

            foreach (Vector2 direction in directions)
            {
                float dot = Vector2.Dot(vector, direction);

                if (dot > maxDot)
                {
                    maxDot = dot;
                    closestDirection = direction;
                }
            }

            return closestDirection;
        }
    }
}