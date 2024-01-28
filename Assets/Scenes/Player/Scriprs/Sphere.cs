using UnityEngine;

public class Sphere : MonoBehaviour
{
    public float circleRadius;
    public float speed;
    public float angle;
    public float damage;
    Transform objTransform;
    PlayerManager player;
    EnemyController enemy;
    private void Start()
    {
        objTransform = transform;
        player = PlayerManager.instance;
        enemy = EnemyController.instance;
    }
    private void FixedUpdate()
    {
        objTransform.position = player.objTransform.position + new Vector3(Mathf.Cos(Time.time * speed + angle) * circleRadius, Mathf.Sin(Time.time * speed + angle) * circleRadius, 0f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemy.TakeDamage(collision.GetComponent<EnemyState>(), damage);
            //collision.GetComponent<HealthPoint>().TakeDamage(damage);
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);

        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);
        }
    }
}
