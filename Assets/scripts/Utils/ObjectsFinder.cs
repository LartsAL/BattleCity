using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class ObjectsFinder
    {
        public static GameObject GetNearestWithTag(Vector2 center, float radius, string tag)
        {
            return Physics2D.OverlapCircleAll(center, radius)
                .Select(collider => collider.gameObject)
                .Where(gameObject => gameObject.CompareTag(tag))
                .OrderBy(gameObject => Vector2.Distance(center, gameObject.transform.position))
                .FirstOrDefault();
        }
        
        public static T GetNearestOfType<T>(Vector2 center, float radius)
        {
            return Physics2D.OverlapCircleAll(center, radius)
                .Select(collider => 
                {
                    if (collider.TryGetComponent(out T component))
                    {
                        float distance = Vector2.Distance(center, collider.transform.position);
                        return (component, distance);
                    }
                    return default;
                })
                .Where(x => x.component != null)
                .OrderBy(x => x.distance)
                .Select(x => x.component)
                .FirstOrDefault();
        }

        public static List<T> GetAllNearestOfType<T>(Vector2 center, float radius)
        {
            return Physics2D.OverlapCircleAll(center, radius)
                .Select(collider => 
                {
                    collider.TryGetComponent(out T component);
                    return component;
                })
                .Where(component => component != null)
                .ToList();
        }
    }
}