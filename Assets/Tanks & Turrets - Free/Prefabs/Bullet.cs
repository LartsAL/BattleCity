// Bullet.cs

using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 2f;

    [Header("Damage")]
    public int damage = 1;

    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        // move bullet ahead to Ox
        // TODO: Also replace moving via direct transform.position changing
        transform.position += transform.right * speed * Time.deltaTime;

        // ttl
        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
