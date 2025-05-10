using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 180f;   

    private Rigidbody2D _rb;
    
    private float _horizontal;
    private float _vertical;
    
    private Vector2 _currentFacingDirection = Vector2.right;
    private Vector2 _movementDirection = Vector2.right;
    private Vector2 _desiredDirection = Vector2.right;
    private Vector2 _targetMoveDirection;
    private Quaternion _targetRotation;
    private bool _isRotating = false;

    // TODO: No need to assign the same values in Start() if we've done it above
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _currentFacingDirection = Vector2.right;
        _movementDirection = _currentFacingDirection;
        _targetRotation = CalculateRotation(_currentFacingDirection);
        transform.rotation = _targetRotation;
    }

    private void Update()
    {
        HandleInput();
        RotateTank();
        MoveTank();
    }

    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(_horizontal) > 0.0f) _desiredDirection = new Vector2(_horizontal, 0.0f).normalized;
        if (Mathf.Abs(_vertical) > 0.0f) _desiredDirection = new Vector2(0.0f, _vertical).normalized;
        
        if (_desiredDirection != _currentFacingDirection && !_isRotating)
        {
            // _desiredDirection can be changed during rotation, so we need to remember its value
            _targetMoveDirection = _desiredDirection;
            _targetRotation = CalculateRotation(_desiredDirection);
            _isRotating = true;
        }
    }

    private Quaternion CalculateRotation(Vector2 dir)
    {
        float angle = 0f;
        if (dir == Vector2.up) angle = 90f;
        else if (dir == Vector2.down) angle = 270f;
        else if (dir == Vector2.left) angle = 180f;
        else if (dir == Vector2.right) angle = 0f;

        return Quaternion.Euler(0f, 0f, angle);
    }

    private void RotateTank()
    {
        if (!_isRotating) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotateSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f)
        {
            transform.rotation = _targetRotation;
            _currentFacingDirection = _targetMoveDirection;
            _movementDirection = _currentFacingDirection;
            _isRotating = false;
        }
    }

    private void MoveTank()
    {
        if (_isRotating) return;

        bool moveKeyPressed = Mathf.Abs(_horizontal) > 0.0f || Mathf.Abs(_vertical) > 0.0f;

        if (moveKeyPressed)
        {
            // TODO: Rewrite with Rigidbody2D (_rb.linearVelocity = ... ?)
            transform.position += (Vector3)(_movementDirection * (moveSpeed * Time.deltaTime));
        }
    }
}