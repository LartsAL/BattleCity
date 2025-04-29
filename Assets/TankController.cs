using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 3f;       // —корость движени€
    public float rotateSpeed = 180f;   // —корость поворота (градусов в секунду)

    private Vector2 currentFacingDirection = Vector2.right; // Ќаправление, в котором танк сейчас смотрит/повернут
    private Vector2 movementDirection = Vector2.right;      // Ќаправление, в котором танк движетс€ (обновл€етс€ после завершени€ поворота)
    private Vector2 desiredDirection = Vector2.right;       // ∆елаемое направление (из ввода)
    private Quaternion targetRotation;                      // ÷елевой поворот
    private bool isRotating = false;                        // ‘лаг, указывающий, идет ли сейчас поворот

    void Start()
    {
        // ”станавливаем начальный поворот и направление
        currentFacingDirection = Vector2.right; // »ли другое начальное направление
        movementDirection = currentFacingDirection;
        targetRotation = CalculateRotation(currentFacingDirection);
        transform.rotation = targetRotation; // ѕримен€ем начальный поворот мгновенно
    }

    void Update()
    {
        HandleInput();
        RotateTank();
        MoveTank();
    }

    void HandleInput()
    {
        // ќпредел€ем желаемое направление на основе ввода
        // »спользуем GetKeyDown дл€ срабатывани€ только при нажатии,
        // но можно оставить GetKey, если нужно мен€ть направление при удержании
        if (Input.GetKeyDown(KeyCode.W)) desiredDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) desiredDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) desiredDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) desiredDirection = Vector2.right;

        // ≈сли желаемое направление отличаетс€ от текущего *направлени€ взгл€да*
        // и танк сейчас не поворачиваетс€, начинаем поворот
        if (desiredDirection != currentFacingDirection && !isRotating)
        {
            targetRotation = CalculateRotation(desiredDirection);
            isRotating = true;
        }
    }

    Quaternion CalculateRotation(Vector2 dir)
    {
        // –ассчитываем целевой угол в Ёйлеровых координатах (вокруг оси Z дл€ 2D)
        float angle = 0f;
        if (dir == Vector2.up) angle = 90f;
        else if (dir == Vector2.down) angle = 270f;
        else if (dir == Vector2.left) angle = 180f;
        else if (dir == Vector2.right) angle = 0f;

        return Quaternion.Euler(0f, 0f, angle);
    }

    void RotateTank()
    {
        if (!isRotating) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation;
            currentFacingDirection = desiredDirection;
            // ќбновл€ем direction только после завершени€ поворота
            movementDirection = currentFacingDirection;
            isRotating = false;
        }
    }

    void MoveTank()
    {
        if (isRotating) return;

        // ѕровер€ем любое нажатие клавиш направлени€
        bool moveKeyPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
                              Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (moveKeyPressed)
        {
            transform.position += (Vector3)(movementDirection * moveSpeed * Time.deltaTime);
        }
    }
}