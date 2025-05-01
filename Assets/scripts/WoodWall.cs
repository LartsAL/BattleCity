using UnityEngine;

[RequireComponent(typeof(Wall))]
public class WoodWall : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Wall>().maxHP = 3;
    }
}
