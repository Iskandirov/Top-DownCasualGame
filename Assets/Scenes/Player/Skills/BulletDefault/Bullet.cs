using UnityEngine;

public class Bullet : SkillBaseMono
{
    public float forceAmount = 30f; // Сила відштовхування
    public bool isPiers;
    public bool isRickoshet;
    public bool isLifeSteal;
    public bool isBulletSlow;
    public float lifeStealPercent;
    public float slowPercent;
    PlayerManager player;
    public GameObject obj;
    private void Start()
    {
        player = PlayerManager.instance;
        if (obj == null)
        {
            obj = player.gameObject;

        }
        if (basa.stats[0].isTrigger)
        {
            basa.damage = basa.stats[0].value;
        }
        if (basa.stats[1].isTrigger)
        {
            player.secondBulletCount = (int)basa.stats[1].value;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.stepMax -= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            player.secondBulletCount = (int)basa.stats[3].value;
        }
        if (basa.stats[4].isTrigger)
        {
            isPiers = true;
        }
       
        isRickoshet = player.isRicoshet;
        isLifeSteal = player.isLifeSteal;
        isBulletSlow = player.isBulletSlow;
        lifeStealPercent = player.lifeStealPercent;
        slowPercent = player.slowPercent;

        player.ShootBullet(obj.transform.position, this);

        CoroutineToDestroy(gameObject, 1f);
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            collision.GetComponent<HealthPoint>().TakeDamage(basa.damage);
            if (isLifeSteal && player.playerHealthPoint < player.playerHealthPointMax)
            {
                if (player.playerHealthPoint + basa.damage * lifeStealPercent < player.playerHealthPointMax)
                {
                    player.playerHealthPoint += basa.damage * lifeStealPercent;
                    player.fullFillImage.fillAmount += (basa.damage * lifeStealPercent) / player.playerHealthPointMax;
                }
                else
                {
                    player.playerHealthPoint = player.playerHealthPointMax;
                    player.fullFillImage.fillAmount = 1;
                }
            }
            Forward enemyMove = collision.GetComponentInParent<Forward>();
            if (isBulletSlow && enemyMove)
            {
                enemyMove.path.maxSpeed = enemyMove.speedMax * slowPercent;
                enemyMove.moveSlowTime = slowPercent;
            }
            GameManager.Instance.FindStatName("bulletDamage", basa.damage);

            if (enemyMove != null)
            {
                enemyMove.Body.AddForce(-(transform.position - collision.transform.position) * forceAmount, ForceMode2D.Impulse);
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

            GameObject projectile = Instantiate(gameObject, contactPoint, transform.rotation);

            float angle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            // Визначення напрямку до найближчого об'єкта
            //Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;

            // Надання снаряду швидкості в напрямку до найближчого об'єкта
            projectile.GetComponent<Rigidbody2D>().AddForce((randomDirection * 15) * GetComponent<Rigidbody2D>().velocity.magnitude * 2);
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        DestroyImmediate(gameObject);
    }

}
