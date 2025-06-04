using UnityEngine;

public class SniperTreeBullet : MonoBehaviour
{
    public float damage;
    public float launchForce = 10f;  // Сила запуску об'єкта
    public float destroyDelay;
    private void Start()
    {
        Vector2 direction = PlayerManager.instance.ShootPoint.transform.position - transform.position;

        direction.Normalize();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction.normalized * launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
        Invoke("Destroyobj", destroyDelay);
    }
    void Destroyobj()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") /*&& !collision.isTrigger */&& !PlayerManager.instance.isInvincible)
        {
            collision.GetComponent<PlayerManager>().TakeDamage(damage);
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
