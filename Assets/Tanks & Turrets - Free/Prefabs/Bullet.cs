using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime); // уничтожить через 2 сек
    }

    void Update()
    {
        // движение по направлению "вперЄд" Ч по локальной оси X (transform.right)
        transform.position += transform.right * speed * Time.deltaTime;
    }
}
