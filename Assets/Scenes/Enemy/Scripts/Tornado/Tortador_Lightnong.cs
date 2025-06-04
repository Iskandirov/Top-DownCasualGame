using UnityEngine;

public class Tortador_Lightnong : MonoBehaviour
{
    float damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") /*&& !collision.isTrigger*/)
        {
            collision.GetComponent<PlayerManager>().TakeDamage(damage);
        }
    }
}
