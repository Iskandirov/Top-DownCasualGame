using UnityEngine;
public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public float searchRadius = 10f;
    public float offset;

    private GameObject target;
    Transform objTransform;
    Transform targetTransform;
    Vector3 targetPosition;
    void Start()
    {
        objTransform = transform;
        target = PlayerManager.instance.ShootPoint;
        targetTransform = target.transform;
        Invoke("DestroyBullet", 4f);
    }
    private void FixedUpdate()
    {
        targetPosition = targetTransform.position + (targetTransform.forward * offset);

        objTransform.position = Vector3.MoveTowards(objTransform.position, targetPosition, speed * Time.deltaTime);

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
            collision.GetComponent<PlayerManager>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
