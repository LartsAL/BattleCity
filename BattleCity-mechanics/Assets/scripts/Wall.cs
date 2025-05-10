using Interfaces;
using UnityEngine;

public class Wall : MonoBehaviour, IDamageable
{
    [SerializeField] private WallConfig config;
    private int _currentHealth;
    public int Health => _currentHealth;

    protected virtual void Start()
    {
        _currentHealth = config.maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (config.isIndestructible) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            if (config.destroyEffect != null)
                Instantiate(config.destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

