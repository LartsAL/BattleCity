using System;
using Brains;
using Interfaces;
using UnityEngine;
using Utils;

namespace Managers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyTankController : MonoBehaviour, IDamageable, IMovable, IRotatable, IShooter
    {
        private static LayerMask _tanksLayer;
        private static LayerMask _wallsLayer;
    
        private IBrain _brain;
    
        [SerializeField] private Rigidbody2D rb;
    
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float attackCooldown = 1.25f;
        private float _currentAttackCooldown;
    
        public float moveSpeed = 2.0f;
        public float rotateSpeed = 90.0f;

        private Vector2 _targetPosition;
        private Vector2 _facingDirection;

        public float minGap = 2.25f;
        public Transform[] raycastPoints;
        
        public float maxHealth = 2.0f;
        private float _currentHealth;

        public GameObject destroyEffect;

        public event Action OnEnemyDeath;
    
        private void Start()
        {
            _tanksLayer = LayerMask.GetMask("Tanks");
            _wallsLayer = LayerMask.GetMask("Walls");
            rb = GetComponent<Rigidbody2D>();
            _brain = new SimplePatrolBrain(gameObject);
            _currentAttackCooldown = attackCooldown;
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            AlignToGrid();
            _brain.Think();
        }

        public void MoveTowards(Vector2 position)
        {
            _facingDirection = transform.right;
            _targetPosition = position;
        
            Vector2 currentPosition = transform.position;
            Vector2 direction = (position - currentPosition).normalized;
            direction = CommonUtils.RoundToCardinalVector(direction);

            if (_facingDirection != direction)
            {
                RotateTowards(direction, rotateSpeed);
                return;
            }

            foreach (var raycastPoint in raycastPoints)
            {
                RaycastHit2D enemyHit = Physics2D.Raycast(raycastPoint.position, _facingDirection, minGap, _tanksLayer);
                if (enemyHit && enemyHit.collider.CompareTag("Enemy"))
                {
                    // Prevents "wall hack" vision
                    RaycastHit2D wallHit = Physics2D.Raycast(raycastPoint.position, _facingDirection, enemyHit.distance, _wallsLayer);
                    if (!wallHit)
                    {
                        return;
                    }
                }
            }
            
            rb.MovePosition(Vector2.MoveTowards(rb.position, position, moveSpeed * Time.fixedDeltaTime));
        }

        public void RotateTowards(Vector2 direction, float speed)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
            }
        }
    
        public void Shoot()
        {
            if (_currentAttackCooldown > 0)
            {
                _currentAttackCooldown -= Time.deltaTime;
                return;
            }

            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _currentAttackCooldown = attackCooldown;
        }

        private void AlignToGrid()
        {
            if (Vector2.Distance(transform.position, _targetPosition) < 0.075f)
            {
                rb.position = _targetPosition;
            }
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
            if (!gameObject.scene.isLoaded)
            {
                return;
            }
        
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }
            
            OnEnemyDeath?.Invoke();
        }
    }
}