using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Utils;

namespace Goals
{
    public class RandomMoveGoal : IGoal
    {
        private readonly System.Random _random;
        
        private readonly MapManager _mapManager;

        private readonly GameObject _relatedObject;
        private readonly IMovable _movable;
        
        private Vector2Int _destinationCell;
        private Vector2[] _path;
        private Vector2Int _currentCell;
        private int _nextPointIdx;

        public RandomMoveGoal(GameObject relatedObject)
        {
            if (!relatedObject.TryGetComponent(out IMovable movable))
            {
                throw new System.Exception("Object must have IMovable interface");
            }
            
            _relatedObject = relatedObject;
            _random = new System.Random();
            _movable = movable;
            _mapManager = GameObject.FindWithTag("MapManager")?.GetComponent<MapManager>();
        }
        
        public bool IsAvailable()
        {
            // No special conditions required, always available
            return true;
        }

        public void Execute()
        {
            // _currentCell = TileFinder.GetNearestTileInfo(_relatedObject.transform.position, 1).GridPosition;
            _currentCell = ObjectsFinder.GetNearestOfType<TileInfo>(_relatedObject.transform.position, 1).GridPosition;
            if (_path == null)
            {
                _destinationCell = GetRandomFreeCell();
                Vector2Int[] cellsPath = Pathfinder.AStar(_mapManager.Maze, _currentCell, _destinationCell);
                cellsPath = Pathfinder.SimplifyPath(cellsPath);
                _path = ToGlobalCoordinates(cellsPath); // Make a method in MapManager? Like CoordinatesAdapter or smth..?
                _nextPointIdx = 0;
            }
            else
            {
                if (Vector2.Distance(_relatedObject.transform.position, _path[_nextPointIdx]) < 0.05f)
                {
                    _nextPointIdx += 1;
                    if (_nextPointIdx >= _path.Length)
                    {
                        _path = null;
                        _nextPointIdx = 0;
                    }
                }
                else
                {
                    _movable.MoveTowards(_path[_nextPointIdx]);
                }
            }
        }

        private Vector2Int GetRandomFreeCell()
        {
            bool[,] maze = _mapManager.Maze;
            List<(int, int)> freeCells = Enumerable.Range(0, maze.GetLength(0))
                .SelectMany(x => Enumerable.Range(0, maze.GetLength(1))
                .Where(y => maze[x, y] == false)
                .Select(y => (x, y)))
                .ToList();

            if (freeCells.Count > 0)
            {
                var cell = freeCells[_random.Next(freeCells.Count)];
                return new Vector2Int(cell.Item1, cell.Item2);
            }

            return _currentCell;
        }

        private Vector2[] ToGlobalCoordinates(Vector2Int[] path)
        {
            Vector2[] globalPath = new Vector2[path.Length];
            for (int i = 0; i < path.Length; ++i)
            {
                globalPath[i] = _mapManager.Floor[path[i].x, path[i].y].transform.position;
            }
            return globalPath;
        }
    }
}