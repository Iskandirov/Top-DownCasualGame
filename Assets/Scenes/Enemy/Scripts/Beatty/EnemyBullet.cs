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
    void Start()
    {
        objTransform = transform;
        target = PlayerManager.instance.gameObject;
        targetTransform = target.transform;
        Invoke("DestroyBullet", 3f);
    }
    private void FixedUpdate()
    {
        // Розрахувати цільову позицію
        Vector3 targetPosition = targetTransform.position + (targetTransform.forward * offset);

        // Перемістити кулю з постійною швидкістю
        objTransform.position = Vector3.MoveTowards(objTransform.position, targetPosition, speed * Time.deltaTime);

        //currentPosition = objTransform.position;
        //direction = (targetTransform.position - currentPosition).normalized;
        //rb.velocity = new Vector3(direction.x * speed, direction.y * speed, currentPosition.z);
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
