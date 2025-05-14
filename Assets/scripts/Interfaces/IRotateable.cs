using UnityEngine;

namespace Interfaces
{
    public interface IRotatable
    {
        public void RotateTowards(Vector2 direction, float speed);
    }
}