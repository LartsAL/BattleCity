using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "WallConfig", menuName = "Walls/WallConfig")]
    public class WallConfig : ScriptableObject
    {
        [Header("Health Settings")]
        public float maxHealth;
        public bool isIndestructible;

        [Header("Visuals")]
        public GameObject destroyEffect;
    }
}
