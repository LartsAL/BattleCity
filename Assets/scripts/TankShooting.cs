// TankShooting.cs
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;    // ������ �������
    public Transform firePoint;        // ����� �������� (�������� ������)
    public float cooldown = 0.5f;      // ����� ����������� � ��������

    private float lastShotTime = -Mathf.Infinity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastShotTime + cooldown)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void Shoot()
    {
        // ������������ ������ � ������� � � �������� firePoint
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
