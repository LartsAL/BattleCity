using System;
using Interfaces;
using MapGenerators;
using UnityEngine;
using Random = System.Random;
using TileType = Interfaces.IMapGenerator.TileType;

[ExecuteInEditMode]
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

    [Header("Grid Settings (odd numbers recommended)")]
    [Min(1)]
    public int width = 25;
    [Min(1)]
    public int height = 25;
    [Min(0.05f)]
    public float cellSize = 1f;

    private readonly IMapGenerator _mapGenerator = new MapGeneratorDFSMaze();
    private bool _generatingMap = false;
    
    public TileType[,] Map { get; private set; }
    public GameObject[,] TilesToObjectsMap { get; private set; }

    private void Start() => GenerateMap();

    private void GenerateMap()
    {
        _generatingMap = true;
        
        Map = _mapGenerator.GenerateMap(width, height);
        TilesToObjectsMap = new GameObject[width, height];
        ClearExistingMap();
        InstantiateMap();
        
        Debug.Log($"Generated map: {width}x{height}");

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
        
        GameObject prefab = GetPrefab(type);
        TilesToObjectsMap[x, y] = PlaceAt(x, y, prefab);
    }

    private void InstantiateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject prefab = GetPrefab(Map[i, j]);
                TilesToObjectsMap[i, j] = PlaceAt(i, j, prefab);
            }
        }
    }
    
    private GameObject GetPrefab(TileType type)
    {
        return type switch
        {
            TileType.Empty => emptyPrefab,
            TileType.Floor => floorPrefab,
            TileType.Water => waterPrefab,
            TileType.Wall => wallPrefabs[_random.Next(0, wallPrefabs.Length)],
            TileType.MapBorder => borderPrefab,
            _ => throw new ArgumentOutOfRangeException()
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
        obj.AddComponent<TileInfo>().Initialize(new Vector2Int(x, y));

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
