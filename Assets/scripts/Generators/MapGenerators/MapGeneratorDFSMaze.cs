using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using Utils;
using TileType = Common.TileType;
using Random = System.Random;

namespace Generators.MapGenerators
{
    public class MapGeneratorDFSMaze : IMapGenerator
    {
        private readonly Random _random = new ();
        
        public TileType[,] GenerateMap(int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                Debug.LogWarning("Width and height should be odd for proper maze generation");
            }
            
            TileType[,] map = new TileType[width, height];

            FillAllWalls(ref map, width, height);
            FillMapBorder(ref map, width, height);
            DigMaze(ref map, width, height);
            CarveAdditionalPassages(ref map, width, height);
            
            return map;
        }

        private void FillAllWalls(ref TileType[,] map, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = TileType.Wall;
                }
            }
        }
        
        private void FillMapBorder(ref TileType[,] map, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                map[i, 0] = TileType.MapBorder;
                map[i, height-1] = TileType.MapBorder;
            }
            for (int i = 0; i < height; i++)
            {
                map[0, i] = TileType.MapBorder;
                map[width-1, i] = TileType.MapBorder;
            }
        }

        private void DigMaze(ref TileType[,] map, int width, int height)
        {
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            Vector2Int start = new Vector2Int(1, 1);
            map[start.x, start.y] = TileType.Floor;
            stack.Push(start);
            
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            while (stack.Count > 0)
            {
                Vector2Int currentTile = stack.Pop();
                directions.Shuffle();
                foreach (var direction in directions)
                {
                    Vector2Int nextTile = currentTile + direction * 2;
                    if (nextTile.x > 0 && nextTile.x < width - 1 && nextTile.y > 0 && nextTile.y < height - 1 && map[nextTile.x, nextTile.y] == TileType.Wall)
                    {
                        // Carve a passage
                        map[nextTile.x, nextTile.y] = TileType.Floor;
                        map[currentTile.x + direction.x, currentTile.y + direction.y] = TileType.Floor;
                        stack.Push(nextTile);
                    }
                }
            }
        }
        
        private void CarveAdditionalPassages(ref TileType[,] map, int width, int height)
        {
            const float koef = 0.05f; // 5%
            const int maxAttempts = 1000;
            
            int additionalPassagesCount = (int)(koef * width * height);
            for (int i = 0; i < maxAttempts; i++)
            {
                if (additionalPassagesCount == 0)
                {
                    break;
                }
                
                int x = _random.Next(1, width-1);
                int y = _random.Next(1, height-1);

                if (map[x, y] == TileType.Wall)
                {
                    if (map[x - 1, y] == TileType.Floor && map[x + 1, y] == TileType.Floor ||
                        map[x, y - 1] == TileType.Floor && map[x, y + 1] == TileType.Floor)
                    {
                        map[x, y] = TileType.Placeholder;
                        additionalPassagesCount--;
                    }
                }
            }

            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    if (map[i, j] == TileType.Placeholder)
                    {
                        map[i, j] = TileType.Floor;
                    }
                }
            }
        }
    }
}