using Brains;
using Interfaces;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyTankController : MonoBehaviour, IMovable, IRotatable, IShooter
{
    private static LayerMask TanksLayer;
    
    private IBrain _brain;
    
    [SerializeField] private Rigidbody2D _rb;
    
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackCooldown = 1.25f;
    private float _currentAttackCooldown;
    
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 90.0f;

    private Vector2 _targetPosition;
    private Vector2 _facingDirection;

    public float minGap = 1.5f;
    
    private void Start()
    {
        TanksLayer = LayerMask.GetMask("Tanks");
        _brain = new SimplePatrolBrain(gameObject);
        _currentAttackCooldown = attackCooldown;
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
        direction = VectorUtils.RoundToCardinal(direction);

        if (_facingDirection != direction)
        {
            //_rb.linearVelocity = Vector2.zero;
            RotateTowards(direction, rotateSpeed);
            return;
        }
        
        if (Physics2D.Raycast(transform.position, _facingDirection, minGap, TanksLayer))
        {
            Debug.Log("Way blocked");
            return;
        }
        
        //_rb.linearVelocity = direction * moveSpeed;
        
        _rb.MovePosition(Vector2.MoveTowards(_rb.position, position, moveSpeed * Time.fixedDeltaTime));
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
            _rb.position = _targetPosition;
        }
    }
}