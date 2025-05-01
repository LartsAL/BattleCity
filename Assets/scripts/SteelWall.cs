using UnityEngine;

[RequireComponent(typeof(Wall))]
public class SteelWall : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Wall>().maxHP = 9;
    }
}
