using Interfaces;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerTankController : MonoBehaviour, IDamageable, IMovable, IRotatable, IShooter
    {
        public float moveSpeed = 2.0f;
        public float rotateSpeed = 180.0f;   

        [SerializeField] private Rigidbody2D rb;
    
        private float _horizontal;
        private float _vertical;
    
        private Vector2 _currentFacingDirection;
        private Vector2 _targetRotateDirection;
        private bool _isRotating = false;

        public GameObject bulletPrefab;
        public Transform firePoint;
        [SerializeField] private float shootCooldown = 0.5f;
        private float _currentShootCooldown = 0.0f;

        public float ShootCooldown => _currentShootCooldown;
        public float MaxShootCooldown => shootCooldown;

        [SerializeField] private float maxHealth = 4.0f;
        private float _currentHealth;

        public float Health => _currentHealth;
        public float MaxHealth => maxHealth;

        public GameObject destroyEffect;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            _currentShootCooldown -= Time.deltaTime;
            _currentFacingDirection = transform.right;
        
            ProcessInput();

            if (_isRotating)
            {
                RotateTowards(_targetRotateDirection, rotateSpeed);
            }
        }

        private void ProcessInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector2 moveDirection = Vector2.zero;

            if (Mathf.Abs(horizontal) > 0.0f)
            {
                moveDirection = new Vector2(horizontal, 0.0f).normalized;
            }
            if (Mathf.Abs(vertical) > 0.0f)
            {
                moveDirection = new Vector2(0.0f, vertical).normalized;
            }

            if (_currentFacingDirection != moveDirection && moveDirection != Vector2.zero)
            {
                _targetRotateDirection = moveDirection;
                _isRotating = true;
            }
        
            MoveTowards((Vector2) transform.position + moveDirection);
        }
    
        public void MoveTowards(Vector2 position)
        {
            if (_isRotating)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 moveDirection = position - (Vector2) transform.position;
            rb.linearVelocity = moveDirection * moveSpeed;
        }

        public void RotateTowards(Vector2 direction, float speed)
        {
            if (!_isRotating)
            {
                return;
            }
        
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                _isRotating = false;
            }
        }

        public void Shoot()
        {
            if (_currentShootCooldown > 0)
            {
                return;
            }
        
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _currentShootCooldown = shootCooldown;
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

            var gameManager = GameObject.FindWithTag("GameManager").GetComponent<SoloGameManager>();
            gameManager.ToGameOverScene();
        }
    }
}