using UnityEngine;

public class Root_Bullet : MonoBehaviour
{
    public GameObject objectToLeave; 
    public Transform leavePoint; 
    public float delay; 
    float delayMax;
    public float damage;
    public float lifeTime;
    PlayerManager player;
    public float launchForce = 10.0f; // Сила запуску
    public void Start()
    {
        player = PlayerManager.instance;
        delayMax = delay;
        Invoke("DestroObj", lifeTime);
        // Отримуємо напрямок до гравця
        Vector2 directionToPlayer = player.ShootPoint.transform.position - transform.position;
        GetComponent<Rigidbody2D>().velocity = directionToPlayer.normalized * launchForce;
    }
    private void FixedUpdate()
    {
        delay -= Time.fixedDeltaTime;
        if (delay <= 0)
        {
            delay = delayMax;
            Instantiate(objectToLeave, leavePoint.position, Quaternion.identity);
        }
    }
    public void DestroObj()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") /*&& !collision.collider.isTrigger*/ && !player.isInvincible)
        {
            player.TakeDamage(damage);
            player.StartSlowPlayer(4f, 0.2f);
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Shield"))
        {
            collision.collider.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
