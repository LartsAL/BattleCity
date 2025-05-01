using UnityEngine;

[RequireComponent(typeof(Wall))]
public class BrickWall : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Wall>().maxHP = 6;
    }
}
