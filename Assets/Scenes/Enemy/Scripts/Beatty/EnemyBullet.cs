using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public Rigidbody2D rb;
    public float searchRadius = 10f;


    private GameObject target;
    Transform objTransform;
    Vector3 currentPosition;
    Vector2 direction;
    void Start()
    {
        objTransform = transform;
        target = PlayerManager.instance.gameObject;
        Invoke("DestroyBullet", 3f);
    }
    private void FixedUpdate()
    {
        currentPosition = objTransform.position;
        direction = (target.transform.position - currentPosition).normalized;
        rb.velocity = new Vector3(direction.x * speed, direction.y * speed, currentPosition.z);
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
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            PlayerManager.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
