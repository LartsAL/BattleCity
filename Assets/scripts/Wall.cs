using Interfaces;
using UnityEngine;
using TileType = Interfaces.IMapGenerator.TileType;

public class Wall : MonoBehaviour, IDamageable
{
    [SerializeField] private WallConfig config;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = config.maxHealth;
    }
    
    public void TakeDamage(int damage)
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

