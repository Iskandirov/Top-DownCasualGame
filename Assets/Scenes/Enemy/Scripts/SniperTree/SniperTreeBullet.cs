using UnityEngine;

public class SniperTreeBullet : MonoBehaviour
{
    public float damage;
    private void Start()
    {
        Invoke("Destroyobj", 5);
    }
    void Destroyobj()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !PlayerManager.instance.isInvincible)
        {
            PlayerManager.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Shield") && !collision.isTrigger)
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
    }
}
