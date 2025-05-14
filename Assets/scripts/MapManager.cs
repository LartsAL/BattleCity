using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Assign all wall prefabs for interior walls (e.g. BrickWall, SteelWall)")]
    public GameObject[] wallPrefabs;
    [Tooltip("Prefab used for map floor")]
    public GameObject floorPrefab;
    [Tooltip("Prefab used for map boundary")]
    public GameObject boundaryPrefab;

    [Header("Grid Settings (odd numbers recommended)")]
    public int width = 25;
    public int height = 25;
    public float cellSize = 1f;
    
    public bool[,] Maze { get; private set; }
    public GameObject[,] Floor { get; private set; }

    [Header("Tag Settings")]
    public string wallTag = "Wall";
    public string floorTag = "Floor";

    private void Start() => GenerateMap();
    
    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (width % 2 == 0 || height % 2 == 0)
            Debug.LogWarning("Width and Height should be odd for proper maze generation.");

        ClearExistingWalls();
        GenerateBoundary();
        Maze = GenerateMaze();
        InstantiateInteriorWalls(Maze);
        Floor = new GameObject[width, height];
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Floor[i, j] = PlaceAt(i, j, floorPrefab, floorTag);
            }
        }

        Debug.Log($"Maze generated: {width}×{height}, passages carved.");
    }

    public void FreeCell(int x, int y)
    {
        Maze[x, y] = false;
    }

    private void GenerateBoundary()
    {
        // perimeter walls
        for (int x = 0; x < width; x++)
        {
            PlaceAt(x, 0, boundaryPrefab, wallTag);
            PlaceAt(x, height - 1, boundaryPrefab, wallTag);
        }
        for (int y = 1; y < height - 1; y++)
        {
            PlaceAt(0, y, boundaryPrefab, wallTag);
            PlaceAt(width - 1, y, boundaryPrefab, wallTag);
        }
    }

    private bool[,] GenerateMaze()
    {
        bool[,] maze = new bool[width, height];
        // initialize all as walls
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = true;
            
        // start DFS from (1,1)
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = false;
        stack.Push(start);

        System.Random rnd = new System.Random();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (stack.Count > 0)
        {
            Vector2Int cell = stack.Pop();
            // shuffle directions
            for (int i = 0; i < dirs.Length; i++)
            {
                int r = rnd.Next(i, dirs.Length);
                (dirs[i], dirs[r]) = (dirs[r], dirs[i]);
            }
            foreach (var dir in dirs)
            {
                Vector2Int next = cell + dir * 2;
                if (next.x > 0 && next.x < width - 1 && next.y > 0 && next.y < height - 1 && maze[next.x, next.y])
                {
                    // carve passage
                    maze[next.x, next.y] = false;
                    maze[cell.x + dir.x, cell.y + dir.y] = false;
                    stack.Push(next);
                }
            }
        }
        return maze;
    }

    private void InstantiateInteriorWalls(bool[,] maze)
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y])
                {
                    // choose random prefab
                    int idx = Random.Range(0, wallPrefabs.Length);
                    PlaceAt(x, y, wallPrefabs[idx], wallTag);
                }
            }
        }
    }

    private GameObject PlaceAt(int x, int y, GameObject prefab, string tag)
    {
        if (prefab == null) return null;

        Vector3 localScale = prefab.transform.localScale;
        Vector3 pos = new Vector3(x * localScale.x * cellSize, y * localScale.y * cellSize, 0f);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);
        
        obj.tag = tag;
        obj.name = prefab.name + "_(" + x + "," + y + ")";
        obj.AddComponent<TileInfo>().Initialize(new Vector2Int(x, y));

        return obj;
    }

    private void ClearExistingWalls()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
