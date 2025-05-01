using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MapWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var bullet = other.GetComponent<Bullet>();
        if (bullet is not null)
        {
            Destroy(other.gameObject);
        }
    }
}
