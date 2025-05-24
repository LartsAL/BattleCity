using Interfaces;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BulletController : MonoBehaviour, IDamageable
    {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        public float moveSpeed = 10.0f;
        public float maxLifeTime = 2.0f;
        
        private float _lifeTime;

        [Header("Damage")]
        public float bulletDamage = 1.0f;
        
        public float maxHealth = 0.5f;
        private float _currentHealth;
        
        [Header("Effects")]
        public GameObject explosionPrefab;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        
            _lifeTime = maxLifeTime;
            _currentHealth = maxHealth;
            
            rb.linearVelocity = transform.right * moveSpeed;
        }

        private void Update()
        {
            _lifeTime -= Time.deltaTime;
            
            if (_lifeTime <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                return;
            }
            
            damageable.TakeDamage(bulletDamage);
            
            Destroy(gameObject);
        }
        
        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
