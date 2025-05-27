using UnityEngine;

namespace Common
{
    public class SpawnPointInfo
    {
        public Vector2Int GridPosition { get; private set; }
        public SpawnPointType Type { get; private set; }

        public SpawnPointInfo(Vector2Int gridPosition, SpawnPointType type)
        {
            GridPosition = gridPosition;
            Type = type;
        }
    }
}