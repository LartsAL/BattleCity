using System.Collections.Generic;
using Common;

namespace Interfaces
{
    public interface ISpawnPointsGenerator
    {
        public List<SpawnPointInfo> GenerateSpawnPoints(in TileType[,] map, int enemySpawnPointCount, int playerSpawnPointCount, float minDistance);
    }
}