using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 3f;       // �������� ��������
    public float rotateSpeed = 180f;   // �������� �������� (�������� � �������)

    private Vector2 currentFacingDirection = Vector2.right; // �����������, � ������� ���� ������ �������/��������
    private Vector2 movementDirection = Vector2.right;      // �����������, � ������� ���� �������� (����������� ����� ���������� ��������)
    private Vector2 desiredDirection = Vector2.right;       // �������� ����������� (�� �����)
    private Quaternion targetRotation;                      // ������� �������
    private bool isRotating = false;                        // ����, �����������, ���� �� ������ �������

    void Start()
    {
        // ������������� ��������� ������� � �����������
        currentFacingDirection = Vector2.right; // ��� ������ ��������� �����������
        movementDirection = currentFacingDirection;
        targetRotation = CalculateRotation(currentFacingDirection);
        transform.rotation = targetRotation; // ��������� ��������� ������� ���������
    }

    void Update()
    {
        HandleInput();
        RotateTank();
        MoveTank();
    }

    void HandleInput()
    {
        // ���������� �������� ����������� �� ������ �����
        // ���������� GetKeyDown ��� ������������ ������ ��� �������,
        // �� ����� �������� GetKey, ���� ����� ������ ����������� ��� ���������
        if (Input.GetKeyDown(KeyCode.W)) desiredDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) desiredDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) desiredDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) desiredDirection = Vector2.right;

        // ���� �������� ����������� ���������� �� �������� *����������� �������*
        // � ���� ������ �� ��������������, �������� �������
        if (desiredDirection != currentFacingDirection && !isRotating)
        {
            targetRotation = CalculateRotation(desiredDirection);
            isRotating = true;
        }
    }

    Quaternion CalculateRotation(Vector2 dir)
    {
        // ������������ ������� ���� � ��������� ����������� (������ ��� Z ��� 2D)
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
            // ��������� direction ������ ����� ���������� ��������
            movementDirection = currentFacingDirection;
            isRotating = false;
        }
    }

    void MoveTank()
    {
        if (isRotating) return;

        // ��������� ����� ������� ������ �����������
        bool moveKeyPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
                              Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (moveKeyPressed)
        {
            transform.position += (Vector3)(movementDirection * moveSpeed * Time.deltaTime);
        }
    }
}