using Common;
using Interfaces;
using Managers;
using UnityEngine;
using TileType = Common.TileType;

namespace Controllers
{
    public class WallController : MonoBehaviour, IDamageable
    {
        [SerializeField] private WallConfig config;
        private float _currentHealth;

        private void Start()
        {
            _currentHealth = config.maxHealth;
        }
    
        public void TakeDamage(float damage)
        {
            if (config.isIndestructible)
            {
                return;
            }

            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }
        
            if (config.destroyEffect != null)
            {
                Instantiate(config.destroyEffect, transform.position, Quaternion.identity);
            }

            MapManager mapManager = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
            Vector2Int wallCell = gameObject.GetComponent<TileInfo>().GridPosition;
            mapManager.ReplaceTile(wallCell.x, wallCell.y, TileType.Floor);
        }
    }
}

