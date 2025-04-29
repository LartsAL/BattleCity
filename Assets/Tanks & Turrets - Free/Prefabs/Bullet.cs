using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime); // ���������� ����� 2 ���
    }

    void Update()
    {
        // �������� �� ����������� "�����" � �� ��������� ��� X (transform.right)
        transform.position += transform.right * speed * Time.deltaTime;
    }
}
