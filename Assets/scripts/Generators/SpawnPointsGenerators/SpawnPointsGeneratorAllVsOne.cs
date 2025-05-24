using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Interfaces;
using UnityEngine;
using Utils;

namespace Generators.SpawnPointsGenerators
{
    public class SpawnPointsGeneratorAllVsOne : ISpawnPointsGenerator
    {
        public List<SpawnPointInfo> GenerateSpawnPoints(in TileType[,] map, int enemySpawnPointCount, int playerSpawnPointCount)
        {
            if (playerSpawnPointCount < 1)
            {
                throw new ArgumentException("Should be at least 1 player spawn point");
            }
            
            int width = map.GetLength(0) - 1;
            int height = map.GetLength(1) - 1;
            
            List<SpawnPointInfo> spawnPoints = new ();
            
            PlaceCenterSpawnPoint(ref spawnPoints, map, width, height, SpawnPointType.Player);

            playerSpawnPointCount--;

            List<Vector2Int> freePerimeterPositions = GetFreePerimeterPositions(map, width, height);
            freePerimeterPositions.Shuffle();

            for (int i = 0; i < playerSpawnPointCount && freePerimeterPositions.Count > 0; i++)
            {
                Vector2Int pos = freePerimeterPositions[0];
                TryAddSpawnPoint(ref spawnPoints, map, pos.x, pos.y, SpawnPointType.Player);
                freePerimeterPositions.RemoveAt(0);
            }
            
            for (int i = 0; i < enemySpawnPointCount && freePerimeterPositions.Count > 0; i++)
            {
                Vector2Int pos = freePerimeterPositions[0];
                TryAddSpawnPoint(ref spawnPoints, map, pos.x, pos.y, SpawnPointType.Enemy);
                freePerimeterPositions.RemoveAt(0);
            }
            
            return spawnPoints;
        }

        private void PlaceCenterSpawnPoint(ref List<SpawnPointInfo> spawnPoints, in TileType[,] map, int width, int height, SpawnPointType type)
        {
            Vector2Int center = new Vector2Int(width / 2 + width % 2, height / 2 + height % 2);

            for (int radius = 0; radius < Mathf.Min(width / 2, height / 2); radius++)
            {
                for (int x = center.x - radius; x <= center.x + radius; x++)
                {
                    if (TryAddSpawnPoint(ref spawnPoints, map, x, center.y - radius, type)) return;
                    if (TryAddSpawnPoint(ref spawnPoints, map, x, center.y + radius, type)) return;
                }

                for (int y = center.y - radius; y <= center.y + radius; y++)
                {
                    if (TryAddSpawnPoint(ref spawnPoints, map, center.x - radius, y, type)) return;
                    if (TryAddSpawnPoint(ref spawnPoints, map, center.x + radius, y, type)) return;
                }
            }
        }
        
        private List<Vector2Int> GetFreePerimeterPositions(TileType[,] map, int width, int height)
        {
            List<Vector2Int> positions = new ();
            
            for (int x = 1; x < width - 1; x++)
            {
                positions.Add(new Vector2Int(x, 1));
                positions.Add(new Vector2Int(x, height - 1));
            }

            for (int y = 2; y < height - 2; y++)
            {
                positions.Add(new Vector2Int(1, y));
                positions.Add(new Vector2Int(width - 1, y));
            }

            return positions
                .Where(pos => map[pos.x, pos.y] == TileType.Floor)
                .ToList();
        }
        
        private bool TryAddSpawnPoint(ref List<SpawnPointInfo> spawnPoints, in TileType[,] map, int x, int y, SpawnPointType type)
        {
            if (map[x, y] == TileType.Floor)
            {
                spawnPoints.Add(new SpawnPointInfo(new Vector2Int(x, y), type));
                return true;
            }

            return false;
        }
    }
}