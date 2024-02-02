using UnityEngine;

public class Tortador_Lightnong : MonoBehaviour
{
    PlayerManager player;
    float damage;
    // Start is called before the first frame update
    void Awake()
    {
        player = PlayerManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(player.tag) && !collision.isTrigger)
        {
            player.TakeDamage(damage);
        }
    }
}
