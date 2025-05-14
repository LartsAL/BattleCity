using UnityEngine;

public class TileInfo : MonoBehaviour
{ 
    public Vector2Int GridPosition { get; private set; }

    public void Initialize(Vector2Int pos)
    {
        GridPosition = pos;
    }
}
