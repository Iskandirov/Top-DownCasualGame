using System.Collections;
using System.Linq;
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
    public GameObject obj;
    public EnemyController enemy;
    private void OnEnable()
    {
        enemy = EnemyController.instance;
    }
    private void Start()
    {
        player = PlayerManager.instance;
        if (obj == null)
        {
            obj = player.gameObject;

        }
        if (basa != null)
        {
            if (basa.stats[0].isTrigger)
            {
                basa.damage = basa.stats[0].value;
            }
            if (basa.stats[1].isTrigger)
            {
                basa.countObjects = (int)basa.stats[1].value;
            }
            if (basa.stats[2].isTrigger)
            {
                basa.stepMax -= basa.stats[2].value;
                basa.stats[2].isTrigger = false;
            }
            if (basa.stats[3].isTrigger)
            {
                basa.countObjects = (int)basa.stats[3].value;
            }
            if (basa.stats[4].isTrigger)
            {
                isPiers = true;
            }
        }
        isLifeSteal = player.isLifeSteal;
        isBulletSlow = player.isBulletSlow;
        lifeStealPercent = player.lifeStealPercent;
        slowPercent = player.slowPercent;
        if (!player.isTutor && player.isAuto && EnemyController.instance.children.Count > 0 && !isRickoshet)
        {
            player.AutoShoot(obj.transform.position,this);
        }
        else if(!isRickoshet)
        {
            player.ShootBullet(obj.transform.position, this);
        }
        isRickoshet = player.isRicoshet;

        CoroutineToDestroy(gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            debuff.StartCoroutine(debuff.EffectTime(Elements.status.Fire, 5));
            if (enemy == null)
            {
                Debug.Log(enemy);
            }
            enemy.TakeDamage(collision.GetComponent<EnemyState>(), basa.damage);
            

            if (isLifeSteal && player.playerHealthPoint < player.playerHealthPointMax)
            {
                if (player.playerHealthPoint + basa.damage * lifeStealPercent < player.playerHealthPointMax)
                {
                    if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                    {
                        DailyQuests.instance.UpdateValue(1, basa.damage * lifeStealPercent, false);
                    }
                    player.playerHealthPoint += basa.damage * lifeStealPercent;
                    player.fullFillImage.fillAmount += (basa.damage * lifeStealPercent) / player.playerHealthPointMax;
                }
                else
                {
                    player.playerHealthPoint = player.playerHealthPointMax;
                    player.fullFillImage.fillAmount = 1;
                }
            }
            EnemyState enemyMove = collision.GetComponent<EnemyState>();
            if (isBulletSlow)
            {
                enemy.SlowEnemy(enemyMove, 1f, slowPercent);
                //enemyControll.moveSlowTime = slowPercent;
            }
            GameManager.Instance.FindStatName("bulletDamage", basa.damage);
            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 3 && s.isActive == true) != null)
            {
                DailyQuests.instance.UpdateValue(3, basa.damage, false);
            }
            //enemyMove.GetComponent<Rigidbody2D>().AddForce(-(transform.position - collision.transform.position) * forceAmount, ForceMode2D.Impulse);

            if (!isPiers)
            {
                if (isRickoshet)
                {
                    Ricoshet(collision);
                }
                Destroy(gameObject);
            }

        }
        else if (collision.CompareTag("Barrel") && !collision.isTrigger)
        {
            collision.gameObject.GetComponent<ObjectHealth>().TakeDamage();
            if (!isPiers)
            {
                Destroy(gameObject);
            }
        }
        else if (!collision.isTrigger && collision.CompareTag("TutorEnemy"))
        {
            EnemyState enemyHealth = collision.GetComponent<EnemyState>();
            enemyHealth.HealthDamage(enemyHealth.health - 1);
            if (collision.GetComponent<EnemyHealthTutorial>().mob.type == "boss")
            {
                Boss mob = collision.GetComponent<EnemyHealthTutorial>().mob;
                mob.fillAmountImage.fillAmount = (enemyHealth.health - 1) / mob.healthMax;
            }

            Destroy(gameObject);
        }
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator LayerChange(Bullet bullet)
    {
        bullet.gameObject.layer = 0;
        yield return new WaitForSeconds(0.2f);
        bullet.gameObject.layer = 7;
    }
    public void Ricoshet(Collider2D collision)
    {
        // Пошук найближчого об'єкта з тегом Enemy
        Collider2D nearestEnemy = Physics2D.OverlapCircle(transform.position, 100f, 10);

        if (nearestEnemy != null)
        {
            Vector3 contactPoint = collision.ClosestPoint(nearestEnemy.transform.position);

            Bullet projectile = Instantiate(this, contactPoint, transform.rotation);
            projectile.StartCoroutine(LayerChange(projectile));
            projectile.isRickoshet = true;
            float angle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            projectile.GetComponent<Rigidbody2D>().AddForce(randomDirection * 15 * GetComponent<Rigidbody2D>().velocity.magnitude * 3);
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        DestroyImmediate(gameObject);
    }

}
