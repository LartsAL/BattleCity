using System;
using System.Collections.Generic;
using Common;
using Generators.MapGenerators;
using Generators.SpawnPointsGenerators;
using Interfaces;
using UnityEngine;
using Random = System.Random;
using TileType = Common.TileType;

namespace Managers
{
    public class MapManager : MonoBehaviour
    {
        private readonly Random _random = new Random();
    
        [Header("Prefabs")]
        [Tooltip("Prefab used for empty tiles")]
        public GameObject emptyPrefab;
    
        [Tooltip("Prefab used for floor tiles")]
        public GameObject floorPrefab;
    
        [Tooltip("Prefab used for water tiles")]
        public GameObject waterPrefab;
    
        [Tooltip("Prefabs for random wall tiles (e.g. BrickWall, SteelWall)")]
        public GameObject[] wallPrefabs;
    
        [Tooltip("Prefab used for map border tiles")]
        public GameObject borderPrefab;

        [Tooltip("Prefab used for enemy spawn points")]
        public GameObject enemySpawnPointPrefab;
        
        [Tooltip("Prefab used for player spawn points")]
        public GameObject playerSpawnPointPrefab;

        [Header("Grid Settings (odd numbers recommended)")]
        [Min(1)]
        public int width = 25;
        [Min(1)]
        public int height = 25;
        [Min(0.05f)]
        public float cellSize = 1f;

        [Header("Spawn Points Setting")]
        public int playerSpawnPointsCount = 1;
        public int enemySpawnPointsCount = 5;
        
        public List<GameObject> EnemySpawnPoints { get; private set; }
        public List<GameObject> PlayerSpawnPoints { get; private set; }

        private readonly IMapGenerator _mapGenerator = new MapGeneratorDFSMaze();
        private readonly ISpawnPointsGenerator _spawnPointsGenerator = new SpawnPointsGeneratorAllVsOne();
        private bool _generatingMap = false;

        public event Action OnMapChanged;
        public TileType[,] Map { get; private set; }
        public GameObject[,] TilesToObjectsMap { get; private set; }

        private void Start() => GenerateMap();

        [ContextMenu("Generate Map")]
        private void GenerateMap()
        {
            _generatingMap = true;
        
            Map = _mapGenerator.GenerateMap(width, height);
            TilesToObjectsMap = new GameObject[width, height];
            ClearExistingMap();
            InstantiateMap();
        
            Debug.Log($"Generated map: {width}x{height}");

            List<SpawnPointInfo> spawnPointsInfo = _spawnPointsGenerator.GenerateSpawnPoints(Map, enemySpawnPointsCount, playerSpawnPointsCount);
            EnemySpawnPoints = new();
            PlayerSpawnPoints = new();
            InstantiateSpawnPoints(spawnPointsInfo);

            _generatingMap = false;
        }

        public void ReplaceTile(int x, int y, TileType type)
        {
            if (_generatingMap) // Tiles cannot be replaced during map generation (foolproof?)
            {
                return;
            }
        
            if (TilesToObjectsMap[x, y] != null)
            {
                Destroy(TilesToObjectsMap[x, y]);
            }
        
            Map[x, y] = type;
        
            GameObject prefab = GetTilePrefab(type);
            TilesToObjectsMap[x, y] = PlaceAt(x, y, prefab);
        
            OnMapChanged?.Invoke();
        }

        private void InstantiateMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject prefab = GetTilePrefab(Map[i, j]);
                    TilesToObjectsMap[i, j] = PlaceAt(i, j, prefab);
                    TilesToObjectsMap[i, j].AddComponent<TileInfo>().Initialize(new Vector2Int(i, j));
                }
            }
        }
    
        private GameObject GetTilePrefab(TileType type)
        {
            return type switch
            {
                TileType.Empty => emptyPrefab,
                TileType.Floor => floorPrefab,
                TileType.Water => waterPrefab,
                TileType.Wall => wallPrefabs[_random.Next(0, wallPrefabs.Length)],
                TileType.MapBorder => borderPrefab,
                _ => throw new ArgumentException($"Prefab cannot be requested for this type of tile: {type}")
            };
        }
        
        private void InstantiateSpawnPoints(List<SpawnPointInfo> spawnPointsInfo)
        {
            foreach (var spawnPointInfo in spawnPointsInfo)
            {
                int x = spawnPointInfo.GridPosition.x;
                int y = spawnPointInfo.GridPosition.y;
                SpawnPointType type = spawnPointInfo.Type;
                
                GameObject prefab = GetSpawnPointPrefab(type);
                GameObject spawnPoint = PlaceAt(x, y, prefab);

                switch (type)
                {
                    case SpawnPointType.Enemy:
                        EnemySpawnPoints.Add(spawnPoint);
                        break;
                    case SpawnPointType.Player:
                        PlayerSpawnPoints.Add(spawnPoint);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private GameObject GetSpawnPointPrefab(SpawnPointType type)
        {
            return type switch
            {
                SpawnPointType.Enemy => enemySpawnPointPrefab,
                SpawnPointType.Player => playerSpawnPointPrefab,
                _ => throw new ArgumentException($"Prefab cannot be requested for this type of spawn point: {type}")
            };
        }

        private GameObject PlaceAt(int x, int y, GameObject prefab)
        {
            if (prefab == null)
            {
                throw new NullReferenceException("prefab == null");
            }

            Vector3 localScale = prefab.transform.localScale;
            Vector3 position = new Vector3(x * localScale.x * cellSize, y * localScale.y * cellSize, 0f);
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        
            obj.name = prefab.name + "_(" + x + "," + y + ")";
            obj.SetActive(true);

            return obj;
        }

        private void ClearExistingMap()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }
    }
}
