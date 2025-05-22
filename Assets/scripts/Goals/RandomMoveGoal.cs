using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Utils;
using TileType = Interfaces.IMapGenerator.TileType;

namespace Goals
{
    public class RandomMoveGoal : IGoal
    {
        private readonly System.Random _random;
        
        private readonly MapManager _mapManager;

        private readonly GameObject _relatedObject;
        private readonly IMovable _movable;
        
        private Vector2Int _destinationTile;
        private Vector2[] _path;
        private Vector2Int _currentTile;
        private int _nextPointIdx;
        
        private Vector2Int _currentTileOld; // For stuck checking
        private float _stuckTimer = 0.0f;
        private const float MaxStuckTime = 3.0f;

        public RandomMoveGoal(GameObject relatedObject)
        {
            if (!relatedObject.TryGetComponent(out IMovable movable))
            {
                throw new System.Exception("Object must have IMovable interface");
            }
            
            _relatedObject = relatedObject;
            _random = new System.Random();
            _movable = movable;
            _mapManager = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();

            _mapManager.OnMapChanged += UpdatePath;
        }
        
        public bool IsAvailable()
        {
            // No special conditions required, always available
            return true;
        }

        public void Execute()
        {
            _currentTile = ObjectsFinder.GetNearestOfType<TileInfo>(_relatedObject.transform.position, 1).GridPosition;

            CheckStuck();
            
            if (_path == null)
            {
                _destinationTile = GetRandomFloorCell();
                UpdatePath();
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
            
            Debug.DrawLine(_destinationTile + new Vector2(-0.5f, -0.5f), _destinationTile + new Vector2(0.5f, 0.5f), Color.red, 0.05f);
            Debug.DrawLine(_destinationTile + new Vector2(-0.5f, 0.5f), _destinationTile + new Vector2(0.5f, -0.5f), Color.red, 0.05f);
        }

        private void CheckStuck()
        {
            if (_currentTile == _currentTileOld)
            {
                _stuckTimer += Time.deltaTime;
                if (_stuckTimer > MaxStuckTime)
                {
                    _destinationTile = GetRandomFloorCell();
                    UpdatePath();
                    _stuckTimer = 0.0f;
                }
            }
            else
            {
                _currentTileOld = _currentTile;
                _stuckTimer = 0.0f;
            }
        }

        private void UpdatePath()
        {
            Vector2Int[] cellsPath = Pathfinder.AStar(_mapManager.Map, _currentTile, _destinationTile);
            if (cellsPath.Length == 0)
            {
                _path = null;
                return;
            }
            cellsPath = Pathfinder.SimplifyPath(cellsPath);
            _path = ToGlobalCoordinatesPath(cellsPath);
            _nextPointIdx = 0;
        }

        private Vector2Int GetRandomFloorCell()
        {
            TileType[,] map = _mapManager.Map;
            List<(int, int)> freeCells = Enumerable.Range(0, map.GetLength(0))
                .SelectMany(x => Enumerable.Range(0, map.GetLength(1))
                .Where(y => map[x, y] == TileType.Floor)
                .Select(y => (x, y)))
                .ToList();

            if (freeCells.Count > 0)
            {
                var cell = freeCells[_random.Next(freeCells.Count)];
                return new Vector2Int(cell.Item1, cell.Item2);
            }

            return _currentTile;
        }

        private Vector2[] ToGlobalCoordinatesPath(Vector2Int[] path)
        {
            Vector2[] globalPath = new Vector2[path.Length];
            for (int i = 0; i < path.Length; ++i)
            {
                globalPath[i] = _mapManager.TilesToObjectsMap[path[i].x, path[i].y].transform.position;
            }
            return globalPath;
        }
    }
}