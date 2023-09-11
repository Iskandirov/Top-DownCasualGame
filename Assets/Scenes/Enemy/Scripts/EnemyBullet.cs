using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public Rigidbody2D rb;
    public float searchRadius = 10f;


    private GameObject target;
    Collider2D[] colliders;
    void Start()
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                target = collider.gameObject;
                Vector3 targetPos = target.transform.position;
                targetPos.z = transform.position.z;
                transform.up = targetPos - transform.position;

                break;
            }
        }
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 currentPosition = transform.position;
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)currentPosition).normalized;
            rb.velocity = new Vector3(direction.x * speed, direction.y * speed, currentPosition.z);
        }
        Invoke("DestroyBullet", 3f);
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            collision.GetComponent<Shield>().healthShield -= damage;
        }
        else if (collision.CompareTag("Player"))
        {
            foreach (Collider2D collider in colliders)
            {
                if (collision == collider)
                {
                    if (!collider.isTrigger)
                    {
                        collider.GetComponent<Health>().playerHealthPoint -= damage;
                        collider.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.GetComponent<Health>().playerHealthPointMax;
                        collider.GetComponent<Animator>().SetBool("IsHit", true);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
