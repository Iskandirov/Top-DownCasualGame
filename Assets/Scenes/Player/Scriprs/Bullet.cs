using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    public float forceAmount = 30f; // Сила відштовхування
    public bool isPiers;
    public bool isRickoshet;
    private void FixedUpdate()
    {
        Invoke("DestroyBullet", 1f);

    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {

            collision.GetComponent<HealthPoint>().healthPoint -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("bulletDamage", damage);
            collision.GetComponent<HealthPoint>().ChangeToKick();

            if (collision.GetComponentInParent<Forward>() != null)
            {
                collision.GetComponentInParent<Forward>().isShooted = true;
                collision.GetComponentInParent<Forward>().Body.AddForce(-(transform.position - collision.transform.position) * forceAmount, ForceMode2D.Impulse);
            }
            if (!isPiers)
            {
                if (isRickoshet)
                {
                    Ricoshet(collision);
                }
                Destroy(gameObject);
            }

        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.gameObject.GetComponent<ObjectHealth>().health -= 1;
            if (!isPiers)
            {
                Destroy(gameObject);
            }
        }
        else if (!collision.isTrigger && collision.CompareTag("TutorEnemy"))
        {
            collision.GetComponent<EnemyHealthTutorial>().health -= 1;
            //Destroy(collision.gameObject);
            if (isRickoshet)
            {
                Ricoshet(collision);
            }
            Destroy(gameObject);
        }
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
    public void Ricoshet(Collider2D collision)
    {
        // Пошук найближчого об'єкта з тегом Enemy
        Collider2D nearestEnemy = Physics2D.OverlapCircle(gameObject.transform.position, 100f, 10);

        // Якщо найближчий об'єкт існує
        if (nearestEnemy != null)
        {
            Vector3 contactPoint = collision.ClosestPoint(nearestEnemy.transform.position);

            // Створення снаряда
            GameObject projectile = Instantiate(gameObject, contactPoint, transform.rotation);

            float angle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            // Визначення напрямку до найближчого об'єкта
            //Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;

            // Надання снаряду швидкості в напрямку до найближчого об'єкта
            projectile.GetComponent<Rigidbody2D>().AddForce((randomDirection * 15) * GetComponent<Rigidbody2D>().velocity.magnitude * 2);
        }
    }
}
