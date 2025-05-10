using UnityEngine;

[CreateAssetMenu(fileName = "WallConfig", menuName = "Walls/WallConfig")]
public class WallConfig : ScriptableObject
{
    [Header("Health Settings")]
    public int maxHealth;
    public bool isIndestructible;

    [Header("Visuals")]
    public GameObject destroyEffect;
}
