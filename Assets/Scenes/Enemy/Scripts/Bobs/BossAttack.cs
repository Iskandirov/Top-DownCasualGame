using FSMC.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    //public BossBullet bulletPrefab;

    //public float delay = 2f;
    //public float interval = 2f;
    //public float damage = 20;

    //public List<bool> attackBools;
    //public int index = 0;
    //public int poolSize = 60;
    //public List<BossBullet> bulletPool;
    //[Header("Attack settings")]
    //public List<GroupCollector> attackGroup;
    //public List<string> attackAnimBoolName;
    //public Animator anim;
    //public void Start()
    //{
    //    StartCoroutine(TimerCoroutineTypesAttack());
    //}
    //private IEnumerator TimerCoroutineTypesAttack()
    //{
    //    while (true)
    //    {

    //        yield return new WaitForSeconds(interval);
    //        AttackTypes();
    //    }
    //}
    //public void StartRecover()
    //{
    //    foreach (var group in attackGroup)
    //    {
    //        group.gameObject.SetActive(true);
    //    }
    //}
    //public void EndRecover()
    //{
    //    anim.SetBool("AttackEnd", false);
    //    anim.SetBool("IsMoveToPlayer", true);
    //}
    //private void AttackTypes()
    //{
    //    if (index < attackAnimBoolName.Count && !anim.GetBool("AttackEnd"))
    //    {
    //        anim.SetBool(attackAnimBoolName[index], true);
    //        index++;
    //    }
    //    else
    //    {
    //        index = 0;
    //    }
    //}
    //public void StartAttack()
    //{
    //    foreach (var bullet in attackGroup[index - 1].group)
    //    {
    //        bullet.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
    //    }
    //}
    //public void DeactivateAttack(string BoolName)
    //{
    //    foreach(var bullet in attackGroup[index - 1].group)
    //    {
    //        bullet.gameObject.layer = LayerMask.NameToLayer("Enemy");
    //    }
    //    anim.SetBool(BoolName, false);
    //    attackGroup[index - 1].gameObject.SetActive(false);
    //    if (index == attackAnimBoolName.Count)
    //    {
    //        anim.SetBool("AttackEnd", true);
    //    }
    //}
    //public void MoveToPlayer()
    //{
    //    transform.position = GetRandomPosition(PlayerManager.instance.objTransform.position,25f);
    //    anim.SetBool("IsMoveToPlayer", false);
    //}
    //public Vector3 GetRandomPosition(Vector3 pos,  float radius)
    //{
    //    float randomAngle = Random.Range(0f, 2f * Mathf.PI);
    //    Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
    //    Vector3 spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);

    //    return spawnPosition;
    //}



    public BossBullet projectilePrefab;
    public Transform firePoint;
    public Transform player;
    public float projectileSpeed = 5f;
    [Header("Projectile Intervals")]
    public float circularAttackInterval = 2f;
    public float waveAttackInterval = 5f;
    public float chaosAttackInterval = 3f;
    public float radialWaveInterval = 4f;
    [Header("Projectile Counts")]
    public int circularProjectiles = 12;
    public int waveProjectiles = 5;
    public int radialProjectiles = 5;
    public int chaosProjectiles = 5;
    [Header("Settings")]

    public float waveSpreadAngle = 45f;
    public float phaseTwoHealthThreshold = 50f;

    private bool isPhaseTwo = false;
    private float spiralAngle = 0f;
    private bool alternateCircularAttack = false;
    private FSMC_Executer bossHealth;
    public bool isTransitioning = false;
    private bool isDefeated = false;
    public Animator anim;
    private Dictionary<AttackType, Action> attackMethods;
    public enum AttackType
    {
        Circular,
        Wave,
        Chaos,
        RadialWave,
        Shrink
    }
    [Header("Visual Settings")]
    public List<GroupCollector> attackGroup;
    private int currentGroupIndex = 0;
    private int currentObjectIndex = 0;
    public void DeactivateNextObject()
    {
        if (currentGroupIndex >= attackGroup.Count)
            return;

        var currentGroup = attackGroup[currentGroupIndex].group;

        if (currentObjectIndex < currentGroup.Count)
        {
            currentGroup[currentObjectIndex].gameObject.SetActive(false);
            currentObjectIndex++;
        }

        if (currentObjectIndex >= currentGroup.Count)
        {
            currentObjectIndex = 0;
            currentGroupIndex++;
        }
    }
    public bool HasActiveObject()
    {
        foreach (var group in attackGroup)
        {
            foreach (var obj in group.group)
            {
                if (obj.activeSelf)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void StartRecover()
    {
        foreach (var group in attackGroup)
        {
            foreach (var obj in group.group)
            {
                obj.gameObject.SetActive(true);
            }
        }
        currentGroupIndex = 0;
        currentObjectIndex = 0;
    }
    void Start()
    {
        player = PlayerManager.instance.objTransform;
        bossHealth = GetComponent<FSMC_Executer>();
        phaseTwoHealthThreshold = bossHealth.healthMax / 2f;

        attackMethods = new Dictionary<AttackType, Action>
        {
            { AttackType.Circular, FireCircularPattern },
            { AttackType.Wave, FireWavePattern },
            { AttackType.Chaos, FireChaosPattern },
            { AttackType.RadialWave, FireRadialWave },
            { AttackType.Shrink, Shrink }
        };

        StartCoroutine(AttackRoutine(circularAttackInterval, AttackType.Circular));
        StartCoroutine(AttackRoutine(waveAttackInterval, AttackType.Wave));
        StartCoroutine(AttackRoutine(15, AttackType.Shrink));
    }

    void Update()
    {

        if (!isPhaseTwo && bossHealth.health <= phaseTwoHealthThreshold && !isTransitioning)
        {
            StartCoroutine(TransitionToPhaseTwo());
        }
        if (!isDefeated && bossHealth.health <= 0)
        {
            StartCoroutine(DefeatBoss());
        }
    }
    IEnumerator TransitionToPhaseTwo()
    {
        anim.SetBool("AttacEnd", true);
        isTransitioning = true;
        // Відтворення анімації або затримка перед другою фазою
        yield return new WaitForSeconds(5f);
        StopCoroutine(TransitionToPhaseTwo());

        isPhaseTwo = true;
        isTransitioning = false;

        StartPhaseTwoAttacks(); // Запускаємо атаки другої фази
    }
    public void EndRecover()
    {
        isTransitioning = false;
        anim.SetBool("AttackEnd", false);
        anim.SetBool("IsMoveToPlayer", true);
    }
    void Shrink()
    {
        isTransitioning = true;
        anim.SetBool("IsMoveToPlayer", true);
    }
    public void MoveToPlayer()
    {
        transform.position = GetRandomPosition(PlayerManager.instance.objTransform.position, 25f);
        anim.SetBool("IsMoveToPlayer", false);
        isTransitioning = false;
    }
    public Vector3 GetRandomPosition(Vector3 pos, float radius)
    {
        float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
        Vector3 spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);

        return spawnPosition;
    }
    void StartPhaseTwoAttacks()
    {
        StartCoroutine(AttackRoutine(radialWaveInterval, AttackType.RadialWave));
        StartCoroutine(AttackRoutine(chaosAttackInterval, AttackType.Chaos));
    }
    IEnumerator DefeatBoss()
    {
        isDefeated = true;
        isTransitioning = true;
        Debug.Log("Boss defeated!" + isDefeated);
        // Відтворення анімації смерті
        yield return new WaitForSeconds(5f);

        //Destroy(gameObject); // Видаляємо босса після перемоги
    }


    IEnumerator AttackRoutine(float interval, AttackType attackType)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (!isDefeated && !isTransitioning && attackMethods.ContainsKey(attackType))
            {
                DeactivateNextObject();
                attackMethods[attackType]();
                if (!HasActiveObject())
                {
                    isTransitioning = true;
                    anim.SetBool("AttackEnd", true);
                }
            }
        }
    }

    void FireCircularPattern()
    {
        float angleStep = 360f / circularProjectiles;
        float offsetAngle = alternateCircularAttack ? angleStep / 2f : 0f; // Зміщення на половину кроку

        for (int i = 0; i < circularProjectiles; i++)
        {
            float angle = i * angleStep + offsetAngle;
            FireProjectile(angle, projectileSpeed);
        }

        alternateCircularAttack = !alternateCircularAttack; // Змінюємо стан для наступної атаки
    }

    void FireWavePattern()
    {
        if (player == null) return;

        // Передбачаємо позицію гравця на момент прибуття снаряда
        float predictionTime = 1f; // Час передбачення, можна налаштувати
        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
        Vector2 predictedPosition = (Vector2)player.position + playerVelocity * predictionTime;

        // Обчислюємо напрямок до передбачуваної позиції
        Vector2 directionToPrediction = (predictedPosition - (Vector2)firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(directionToPrediction.y, directionToPrediction.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - waveSpreadAngle / 2;
        float angleStep = waveSpreadAngle / (waveProjectiles - 1);

        for (int i = 0; i < waveProjectiles; i++)
        {
            float angle = startAngle + (angleStep * i);
            FireProjectile(angle, projectileSpeed * 2);
        }
    }

    void FireChaosPattern()
    {
        float minSpeed = projectileSpeed * 0.5f; // Мінімальна швидкість
        float maxSpeed = projectileSpeed * 1.5f; // Максимальна швидкість

        for (int i = 0; i < chaosProjectiles; i++)
        {
            float randomAngle = UnityEngine.Random.Range(0f, 360f); // Випадковий напрямок
            float randomSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed); // Випадкова швидкість
            FireProjectile(randomAngle, randomSpeed);
        }
    }

    void FireRadialWave()
    {
        float angleStep = 360f / radialProjectiles;
        for (int i = 0; i < radialProjectiles; i++)
        {
            float angle = i * angleStep;
            BossBullet projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 initialDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rb.velocity = initialDirection * projectileSpeed;

            // Додаємо корекцію напрямку через певний час
            StartCoroutine(ChangeProjectileDirection(rb));
        }
    }
    IEnumerator ChangeProjectileDirection(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(0.5f); // Затримка перед зміною напрямку

        if (rb != null)
        {
            Vector2 newDirection = Quaternion.Euler(0, 0, -1000f) * rb.velocity.normalized;
            rb.velocity = newDirection * projectileSpeed;
        }
    }
    void FireProjectile(float angle, float speed)
    {
        BossBullet projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rb.velocity = direction * speed;
        if (FindAnyObjectByType<Tutor>())
        {
            projectile.damage = 0;
        }
        StartCoroutine(IncreaseProjectileDistance(rb));
    }

    IEnumerator IncreaseProjectileDistance(Rigidbody2D rb)
    {
        float acceleration = 0.5f;
        while (rb != null)
        {
            rb.velocity += rb.velocity.normalized * acceleration * Time.deltaTime;
            yield return null;
        }
    }
}