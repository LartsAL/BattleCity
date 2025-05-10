// TankShooting.cs
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;    // Префаб снаряда
    public Transform firePoint;        // Точка выстрела (дочерний объект)
    public float cooldown = 0.5f;      // Время перезарядки в секундах

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
        // Инстанцируем снаряд в позиции и с ротацией firePoint
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
