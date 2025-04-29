using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour
{
    public float moveSpeed = 3f;       // —корость движени€
    public float rotateSpeed = 180f;   // —корость поворота (градусов в секунду)

    private Rigidbody2D _rb;
    
    private float _horizontal;
    private float _vertical;
    
    private Vector2 _currentFacingDirection = Vector2.right; // Ќаправление, в котором танк сейчас смотрит/повернут
    private Vector2 _movementDirection = Vector2.right; // Ќаправление, в котором танк движетс€ (обновл€етс€ после завершени€ поворота)
    private Vector2 _desiredDirection = Vector2.right; // ∆елаемое направление (из ввода)
    private Vector2 _targetMoveDirection;
    private Quaternion _targetRotation; // ÷елевой поворот
    private bool _isRotating = false; // ‘лаг, указывающий, идет ли сейчас поворот

    // TODO: No need to assign the same values in Start() if we've done it above
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // ”станавливаем начальный поворот и направление
        _currentFacingDirection = Vector2.right; // »ли другое начальное направление
        _movementDirection = _currentFacingDirection;
        _targetRotation = CalculateRotation(_currentFacingDirection);
        transform.rotation = _targetRotation; // ѕримен€ем начальный поворот мгновенно
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

        // ќпредел€ем желаемое направление на основе ввода
        if (Mathf.Abs(_horizontal) > 0.0f) _desiredDirection = new Vector2(_horizontal, 0.0f).normalized;
        if (Mathf.Abs(_vertical) > 0.0f) _desiredDirection = new Vector2(0.0f, _vertical).normalized;
        
        // ≈сли желаемое направление отличаетс€ от текущего *направлени€ взгл€да*
        // и танк сейчас не поворачиваетс€, начинаем поворот
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
        // –ассчитываем целевой угол в Ёйлеровых координатах (вокруг оси Z дл€ 2D)
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
            // ќбновл€ем direction только после завершени€ поворота
            _movementDirection = _currentFacingDirection;
            _isRotating = false;
        }
    }

    private void MoveTank()
    {
        if (_isRotating) return;

        // ѕровер€ем любое нажатие клавиш направлени€
        bool moveKeyPressed = Mathf.Abs(_horizontal) > 0.0f || Mathf.Abs(_vertical) > 0.0f;

        if (moveKeyPressed)
        {
            // TODO: Rewrite with Rigidbody2D (_rb.linearVelocity = ... ?)
            transform.position += (Vector3)(_movementDirection * (moveSpeed * Time.deltaTime));
        }
    }
}