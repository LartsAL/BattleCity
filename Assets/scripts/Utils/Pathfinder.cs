using System;
using System.Collections.Generic;
using UnityEngine;
using TileType = Common.TileType;

namespace Utils
{
    public static class Pathfinder
    {
        private class Node
        {
            public readonly Vector2Int Position;
            public Node Parent;
            public int GCost;
            public int HCost;
            public int FCost => GCost + HCost;

            public Node(Vector2Int position)
            {
                Position = position;
            }
        }
        
        public static Vector2Int[] AStar(TileType[,] map, Vector2Int startPoint, Vector2Int endPoint)
        {
            // Check if start or end is out of bounds or blocked
            if (!IsPositionValid(map, startPoint) || !IsPositionValid(map, endPoint))
                return Array.Empty<Vector2Int>();

            if (map[startPoint.x, startPoint.y] != TileType.Floor || map[endPoint.x, endPoint.y] != TileType.Floor)
                return Array.Empty<Vector2Int>();

            // Initialize open and closed sets
            var openSet = new List<Node>();
            var closedSet = new HashSet<Vector2Int>();
            var nodeGrid = new Node[map.GetLength(0), map.GetLength(1)];

            // Create start node
            var startNode = new Node(startPoint)
            {
                GCost = 0,
                HCost = CalculateHeuristic(startPoint, endPoint)
            };
            openSet.Add(startNode);
            nodeGrid[startPoint.x, startPoint.y] = startNode;

            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            };

            while (openSet.Count > 0)
            {
                // Get node with the lowest F cost
                var currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || 
                        (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                if (currentNode.Position == endPoint)
                {
                    return ReconstructPath(currentNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                foreach (var direction in directions)
                {
                    Vector2Int neighborPos = currentNode.Position + direction;

                    // Skip if not valid or wall or already closed
                    if (!IsPositionValid(map, neighborPos) || 
                        map[neighborPos.x, neighborPos.y] != TileType.Floor || 
                        closedSet.Contains(neighborPos))
                    {
                        continue;
                    }

                    int newGCost = currentNode.GCost + 1;

                    Node neighborNode = nodeGrid[neighborPos.x, neighborPos.y];
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighborPos);
                        nodeGrid[neighborPos.x, neighborPos.y] = neighborNode;
                        openSet.Add(neighborNode);
                    }
                    else if (newGCost >= neighborNode.GCost)
                    {
                        continue;
                    }

                    neighborNode.Parent = currentNode;
                    neighborNode.GCost = newGCost;
                    neighborNode.HCost = CalculateHeuristic(neighborPos, endPoint);
                }
            }

            // No path found
            return Array.Empty<Vector2Int>();
        }
        
        public static Vector2Int[] SimplifyPath(Vector2Int[] path)
        {
            if (path == null || path.Length < 2)
            {
                return path;
            }

            var simplifiedPath = new List<Vector2Int> { path[0] };

            Vector2Int previousDirection = Vector2Int.zero;

            for (int i = 1; i < path.Length; i++)
            {
                Vector2Int currentDir = path[i] - path[i - 1];

                if (currentDir != previousDirection && i > 1)
                {
                    simplifiedPath.Add(path[i - 1]);
                }

                previousDirection = currentDir;
            }

            simplifiedPath.Add(path[^1]);

            return simplifiedPath.ToArray();
        }
        
        private static bool IsPositionValid(TileType[,] map, Vector2Int position)
        {
            return position.x >= 0 && position.x < map.GetLength(0) && 
                   position.y >= 0 && position.y < map.GetLength(1);
        }

        private static int CalculateHeuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static Vector2Int[] ReconstructPath(Node endNode)
        {
            var path = new List<Vector2Int>();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path.ToArray();
        }
    }
}