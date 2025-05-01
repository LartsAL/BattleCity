using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Wall HP")]
    public int maxHP = 3;
    private int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// Damage to wall
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.damage);
            Destroy(other.gameObject);
        }
    }
}

