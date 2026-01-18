using DG.Tweening.Core.Easing;
using FSMC.Runtime;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Bullet : SkillBaseMono
{
    public float forceAmount = 30f;
    public bool isPierce;
    public bool isRickoshet;
    public bool isLifeSteal;
    public bool isBulletSlow;
    public float lifeStealPercent;
    public float slowPercent;
    public float launchForce;
    public Transform spawnPoint;

    Rigidbody2D rb;
    PlayerManager cachedPlayer;

    // Новые поля для мультишота
    [HideInInspector] public int projectilesPerShot = 1;
    [HideInInspector] public float projectileInterval = 0.2f;
    [HideInInspector] public int sequenceIndex = 0; // 0 = мастер, >0 = клон

    // Храним направление, чтобы клоны могли запуститься тем же вектором
    private Vector2 lastLaunchDirection = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        cachedPlayer = PlayerManager.instance;
        player = cachedPlayer;

        if (basa != null)
            ApplyStats();

        isLifeSteal = cachedPlayer != null ? cachedPlayer.isLifeSteal : isLifeSteal;
        isBulletSlow = cachedPlayer != null ? cachedPlayer.isBulletSlow : isBulletSlow;
        lifeStealPercent = cachedPlayer != null ? cachedPlayer.lifeStealPercent : lifeStealPercent;
        slowPercent = cachedPlayer != null ? cachedPlayer.slowPercent : slowPercent;

        Init(cachedPlayer != null && cachedPlayer.isAuto, spawnPoint);
        isRickoshet = cachedPlayer != null && cachedPlayer.isRicoshet;
    }

    private void ApplyStats()
    {
        if (basa.stats != null && basa.stats.Count > 0 && basa.stats[0].isTrigger)
            basa.damage = basa.stats[0].value;

        if (basa.stats != null && basa.stats.Count > 1 && basa.stats[1].isTrigger)
            basa.countObjects = (int)basa.stats[1].value;

        if (basa.stats != null && basa.stats.Count > 2 && basa.stats[2].isTrigger)
        {
            basa.stepMax -= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }

        if (basa.stats != null && basa.stats.Count > 3 && basa.stats[3].isTrigger)
            basa.countObjects = (int)basa.stats[3].value;

        if (basa.stats != null && basa.stats.Count > 4 && basa.stats[4].isTrigger)
            isPierce = true;
    }

    public void Init(bool isAuto, Transform trans)
    {
        if (!isRickoshet)
        {
            if (isAuto && GameManager.Instance != null && GameManager.Instance.enemies != null && GameManager.Instance.enemies.children.Count > 0)
            {
                if (trans != null)
                    AutoShoot(trans.position);
                else
                    Destroy(gameObject);
            }
            else
            {
                if (trans != null)
                    ShootBullet(trans.position);
                else
                    Destroy(gameObject);
            }
        }

        isRickoshet = cachedPlayer != null && cachedPlayer.isRicoshet;
        CoroutineToDestroy(gameObject, basa != null && basa.lifeTime > 0 ? basa.lifeTime : 5f);
    }

    public void ShootBullet(Vector3 position)
    {
        transform.position = position;
        Vector2 directionBullet = cachedPlayer != null ? cachedPlayer.GetMousDirection(position) : Vector2.zero;
        if (directionBullet != Vector2.zero)
            Launch(directionBullet);
        else
            Destroy(gameObject);
    }

    public void AutoShoot(Vector3 position)
    {
        transform.position = position;
        Vector2 direction = GetDirectionToNearestEnemy(position);
        if (direction != Vector2.zero)
            Launch(direction);
        else
            Destroy(gameObject);
    }

    private Vector2 GetDirectionToNearestEnemy(Vector3 fromPosition, float maxDistance = 50f)
    {
        Vector2 nearestDirection = Vector2.zero;
        float nearestDistSqr = Mathf.Infinity;

        var enemies = GameManager.Instance?.enemies?.children;
        if (enemies == null) return Vector2.zero;

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeSelf) continue;

            var sr = enemy.GetComponentInChildren<SpriteRenderer>();
            if (sr == null || sr.color.a == 0) continue;

            Vector3 enemyPos = enemy.objTransform.position;
            float distSqr = (enemyPos - fromPosition).sqrMagnitude;

            if (distSqr < nearestDistSqr && distSqr <= maxDistance * maxDistance)
            {
                nearestDistSqr = distSqr;
                nearestDirection = (enemyPos - fromPosition).normalized;
            }
        }

        return nearestDirection;
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        lastLaunchDirection = direction.normalized; // запоминаем направление для клонов

        rb.velocity = Vector2.zero;
        rb.AddForce(lastLaunchDirection * launchForce, ForceMode2D.Force);

        float angleShot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);

        // Только мастер запускает корутину создания дополнительных снарядов
        if (sequenceIndex == 0 && projectilesPerShot > 1)
        {
            StartCoroutine(SpawnAdditionalShots());
        }
    }

    private IEnumerator SpawnAdditionalShots()
    {
        // создаём projectilesPerShot-1 клонов, с интервалом projectileInterval
        for (int i = 1; i < projectilesPerShot; i++)
        {
            yield return new WaitForSeconds(projectileInterval);

            // Если объект уничтожен — прекратить спавн
            if (this == null || !gameObject.activeInHierarchy) yield break;

            Bullet proj = Instantiate(this, transform.position, transform.rotation);
            if (proj == null) continue;

            proj.sequenceIndex = i;
            proj.isRickoshet = this.isRickoshet;
            // копируем кэш игрока (allowed — доступ к private полю другого экземпляра того же типа разрешён)
            proj.cachedPlayer = this.cachedPlayer;

            // инициализируем rb у клона
            proj.rb = proj.GetComponent<Rigidbody2D>();

            // корректно запускаем
            proj.Launch(lastLaunchDirection);
            // установим время жизни для клона
            proj.CoroutineToDestroy(proj.gameObject, proj.basa != null && proj.basa.lifeTime > 0 ? proj.basa.lifeTime : 5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.TryGetComponent<ElementActiveDebuff>(out var debuff))
                debuff.ApplyEffect(status.Fire, 5);

            if (!collision.TryGetComponent<FSMC_Executer>(out var enemyMove)) return;

            enemyMove.TakeDamage(basa != null ? basa.damage : 0f, damageMultiplier);

            if (isLifeSteal && cachedPlayer != null && cachedPlayer.playerHealthPoint < cachedPlayer.playerHealthPointMax)
            {
                float healAmount = (basa != null ? basa.damage : 0f) * lifeStealPercent;
                float toAdd = Mathf.Min(healAmount, cachedPlayer.playerHealthPointMax - cachedPlayer.playerHealthPoint);
                if (toAdd > 0)
                {
                    if (DailyQuests.instance != null && DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive) != null)
                        DailyQuests.instance.UpdateValue(1, toAdd, false, true);

                    cachedPlayer.playerHealthPoint += toAdd;
                    GameManager.Instance.fullFillImage.fillAmount += toAdd / cachedPlayer.playerHealthPointMax;
                }
            }

            if (isBulletSlow)
            {
                enemyMove.StateMachine.SetFloat("SlowTime", 1f);
                enemyMove.StateMachine.SetFloat("SlowPercent", slowPercent);
                enemyMove.StateMachine.SetCurrentState("Slowed", enemyMove);
            }

            GameManager.Instance.FindStatName("bulletDamage", basa != null ? basa.damage : 0f);
            if (DailyQuests.instance != null)
                DailyQuests.instance.UpdateValue(3, basa != null ? basa.damage : 0f, false, true);

            var enemyRb = enemyMove.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
                enemyRb.AddForce(-(transform.position - collision.transform.position) * forceAmount, ForceMode2D.Impulse);

            if (!isPierce)
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
            if (collision.TryGetComponent<ObjectHealth>(out var oh))
                oh.TakeDamage();
            if (!isPierce) Destroy(gameObject);
        }
        else if (!collision.isTrigger && collision.CompareTag("TutorEnemy"))
        {
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
        Bullet projectile = Instantiate(this, transform.position, transform.rotation);
        projectile.StartCoroutine(LayerChange(projectile));
        projectile.isRickoshet = true;

        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        if (rb != null)
            projectile.GetComponent<Rigidbody2D>().AddForce((randomDirection * 15f) * rb.velocity.magnitude * 3f);
        else
            projectile.GetComponent<Rigidbody2D>().AddForce(randomDirection * 15f);
    }

    private void OnLevelWasLoaded(int level)
    {
        DestroyImmediate(gameObject);
    }
}
