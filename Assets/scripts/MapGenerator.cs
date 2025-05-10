using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Assign all wall prefabs for interior walls (e.g. BrickWall, SteelWall)")]
    public GameObject[] wallPrefabs;
    [Tooltip("Prefab used for map boundary")]
    public GameObject boundaryPrefab;

    [Header("Grid Settings (odd numbers recommended)")]
    public int width = 25;
    public int height = 25;
    public float cellSize = 1f;

    [Header("Tag Settings")]
    public string wallTag = "Wall";

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (width % 2 == 0 || height % 2 == 0)
            Debug.LogWarning("Width and Height should be odd for proper maze generation.");

        ClearExistingWalls();
        GenerateBoundary();
        bool[,] maze = GenerateMaze();
        InstantiateInteriorWalls(maze);

        Debug.Log($"Maze generated: {width}×{height}, passages carved.");
    }

    void GenerateBoundary()
    {
        // perimeter walls
        var localScale = boundaryPrefab.transform.localScale;
        float sizeX = localScale.x;
        float sizeY = localScale.y;
        for (int x = 0; x < width; x++)
            PlaceAt(x * sizeX, 0, boundaryPrefab);
        for (int x = 0; x < width; x++)
            PlaceAt(x * sizeX, (height - 1) * sizeY, boundaryPrefab);
        for (int y = 1; y < height - 1; y++)
            PlaceAt(0, y * sizeY, boundaryPrefab);
        for (int y = 1; y < height - 1; y++)
            PlaceAt((width - 1) * sizeX, y * sizeY, boundaryPrefab);
    }

    bool[,] GenerateMaze()
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
                //var tmp = dirs[i]; dirs[i] = dirs[r]; dirs[r] = tmp;
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

    void InstantiateInteriorWalls(bool[,] maze)
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y])
                {
                    // choose random prefab
                    int idx = Random.Range(0, wallPrefabs.Length);
                    float sizeX = wallPrefabs[idx].transform.localScale.x;
                    float sizeY = wallPrefabs[idx].transform.localScale.y;
                    PlaceAt(x * sizeX, y * sizeY, wallPrefabs[idx]);
                }
            }
        }
    }

    void PlaceAt(float x, float y, GameObject prefab)
    {
        if (prefab == null) return;
        Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0f);
        GameObject w = Instantiate(prefab, pos, Quaternion.identity, transform);
        w.tag = wallTag;
        w.name = prefab.name + "_(" + x + "," + y + ")";
    }

    void ClearExistingWalls()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    private void Start() => GenerateMap();
}
