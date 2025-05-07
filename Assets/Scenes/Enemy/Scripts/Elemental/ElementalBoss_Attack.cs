using FSMC.Runtime;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
[System.Serializable]
public class BossAttacks
{
    public string triggerName; // Назва тригера для Animator
    public float cooldown;     // Затримка між атаками
    public GameObject[] vfxObjects; // Ефекти, які активуються до анімації
    public AttackType type;
    public float damage;
    public string clipName;
}
public enum AttackType
{
    Fist,
    Jump,
    Meteor,
    Sandstorm,
    StonePrison,
    // Додавай інші типи тут
}
public class ElementalBoss_Attack : MonoBehaviour
{
    [Header("Basic Settings")]
    public List<Collider2D> fistHitZone;
    public List<Collider2D> bodyParts;

    public float initialForce = 30f;
    public float duration = 0.5f;
    public float reductionFactor = 1f;

    private bool playerInZone;
    private bool enemyInZone;

    Rigidbody2D playerRB;
    Collider2D playerCollider;
    PlayerManager player;
    Animator objAnim;
    Shield objShield;

    Transform objTransform;
    Collider2D objCollider;
    public List<Rigidbody2D> enemy = new List<Rigidbody2D>();

    public List<BossAttacks> bossAttacks = new List<BossAttacks>();
    private bool isAttacking = false;

    [Header("Meteor Settings")]
    public GameObject meteorMarkerPrefab;
    public GameObject meteorPrefab;
    public GameObject projectilePrefab;

    public int meteorCount = 5;
    public float spawnRadius = 10f;
    public float meteorFallDelay = 1.5f;
    public float meteorLifetime = 5f;
    public float projectileSpeed = 3f;
    public LayerMask obstacleLayer;
    [Header("SandStorm Settings")]
    [SerializeField] private GameObject sandstormPrefab;
    [SerializeField] private float sandstormDuration = 8f;
    [SerializeField] private float sandstormDamagePerSecond = 5f;
    [SerializeField] private float slowMultiplier = 0.5f;
    GameObject storm;
    SandStorm sand;
    private bool previousZoneState = false;
    private Coroutine distortionRoutine;
    public Material sandStormMat;
    [Header("StonePrison Settings")]
    [SerializeField] private GameObject prisonMarkPrefab;
    [SerializeField] private GameObject stonePrisonPrefabHorizontal;
    [SerializeField] private GameObject stonePrisonPrefabVertical;
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private float delayBeforeSpawn = 1f;
    public float barrierLifetime = 5f;
    public float stoneSpawnRadius = 2f;
    public int numberOfBarriers = 4;

    void Start()
    {
        player = PlayerManager.instance;
        playerRB = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();
        objAnim = GetComponent<Animator>();
        objTransform = transform;
        objShield = FindObjectOfType<Shield>();
        objCollider = GetComponent<Collider2D>();

        foreach (var attack in bossAttacks)
        {
            StartCoroutine(AttackRoutine(attack));
        }
    }
    IEnumerator AttackRoutine(BossAttacks attack)
    {
        while (true)
        {
            yield return new WaitForSeconds(attack.cooldown);
            if (!isAttacking)
            {
                isAttacking = true;
                //if (playerInZone || attack.type == AttackType.Jump) // або будь-яка інша умова
                foreach (var vfx in attack.vfxObjects)
                    vfx?.SetActive(true);

                //isAttacking = true;
                objAnim.SetTrigger(attack.triggerName);
                // Подальші дії тригеряться через Animation Event
            }
        }
    }
    public void ExecuteAttack(string typeName)
    {
        //if (isAttacking) return;
        if (!System.Enum.TryParse(typeName, out AttackType type)) return;

        var attack = GetAttackByType(type);

        switch (type)
        {
            case AttackType.Fist:
                if (playerInZone && objCollider.IsTouching(playerCollider))
                {
                    ApplyForceTo(playerRB, GetAttackByType(type).damage);
                }

                ToggleVFX(GetAttackByType(type), false);
                CineMachineCameraShake.instance.Shake(10, .1f);
                break;
            case AttackType.Jump:
                if (playerInZone)
                {
                    ApplyForceTo(playerRB, GetAttackByType(type).damage);
                }
                if (enemyInZone)
                {
                    foreach (var enemyRB in enemy)
                    {
                        StartCoroutine(ReducePushForce(enemyRB, initialForce));
                    }
                }

                ToggleVFX(GetAttackByType(type), false);
                CineMachineCameraShake.instance.Shake(30, .1f);
                break;
            case AttackType.Meteor:
                StartCoroutine(MeteorAttackRoutine());
                break;
            case AttackType.Sandstorm:
                StartCoroutine(SandstormRoutine());
                break;
            case AttackType.StonePrison:
                StartCoroutine(StonePrisonRoutine());
                break;
        }
        StartCoroutine(ResetAttackFlagAfterDelay(GetAnimationClipLength(attack.clipName)));
        //isAttacking = false;
        objAnim.SetTrigger(typeName);
        GetComponent<FSMC_Executer>().SetTrigger("Attack");
    }
    float GetAnimationClipLength(string clipName)
    {
        var clips = objAnim.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.LogWarning($"Анімація '{clipName}' не знайдена!");
        return 1f; // fallback
    }
    IEnumerator ResetAttackFlagAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }
    BossAttacks GetAttackByType(AttackType type)
    {
        return bossAttacks.Find(a => a.type == type);
    }

    void ToggleVFX(BossAttacks attack, bool state)
    {
        foreach (var vfx in attack.vfxObjects)
            if (vfx != null) vfx.SetActive(state);
    }
    public void TransportToPlayer()
    {
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 10);
    }
    void ApplyForceTo(Rigidbody2D rb, float damage)
    {
        Shield shield = objShield;
        if (shield != null && shield.CompareTag("Shield"))
        {
            shield.healthShield -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
        }
        else if (!player.isInvincible)
        {
            player.TakeDamage(damage);
        }

        StartCoroutine(ReducePushForce(rb, initialForce));
    }
    //Метеоритний дощ
    IEnumerator ReducePushForce(Rigidbody2D rb, float force)
    {
        float elapsedTime = 0f;
        float currentForce = force;

        while (elapsedTime < duration)
        {
            Vector2 direction = (rb.transform.position - transform.position).normalized;
            rb.velocity = direction * currentForce;

            currentForce -= reductionFactor * force * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }
    public IEnumerator MeteorAttackRoutine()
    {
        List<Vector2> spawnPositions = GenerateUniquePositions(transform.position, spawnRadius, meteorCount, 1.5f);

        foreach (var pos in spawnPositions)
        {
            Instantiate(meteorMarkerPrefab, pos, Quaternion.identity);
        }

        yield return new WaitForSeconds(meteorFallDelay);

        foreach (var pos in spawnPositions)
        {
            GameObject meteor = Instantiate(meteorPrefab, pos + new Vector2(1f, 1f) * 100f, Quaternion.identity);
            StartCoroutine(HandleMeteorLanding(meteor, pos));
        }
    }
    List<Vector2> GenerateUniquePositions(Vector2 center, float radius, int count, float minDistance)
    {
        List<Vector2> positions = new List<Vector2>();

        int attempts = 0;
        while (positions.Count < count && attempts < 500)
        {
            Vector2 randomPos = center + Random.insideUnitCircle * radius;
            if (!Physics2D.OverlapCircle(randomPos, 0.5f, obstacleLayer) &&
    positions.All(p => Vector2.Distance(p, randomPos) >= minDistance))
            {
                positions.Add(randomPos);
            }
            attempts++;
        }

        return positions;
    }

    IEnumerator HandleMeteorLanding(GameObject meteor, Vector2 targetPos)
    {
        float fallSpeed = 100f;
        bool hasLanded = false;

        while (!hasLanded)
        {
            meteor.transform.position = Vector2.MoveTowards(meteor.transform.position, targetPos, fallSpeed * Time.deltaTime);
            if (Vector2.Distance(meteor.transform.position, targetPos) < 0.1f)
            {
                hasLanded = true;
            }
            yield return null;
        }

        meteor.transform.position = targetPos;
        Collider2D col = meteor.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = false;
        }

        // Перевірка влучання по гравцю
        if (playerInZone && Vector2.Distance(player.transform.position, targetPos) < 1f)
        {
            ApplyForceTo(playerRB, 10f); // або окрема шкода для метеориту
        }

        yield return new WaitForSeconds(meteorLifetime);

        Vector2 direction = (GetNearestEnemyPosition(targetPos) - targetPos).normalized;
        GameObject projectile = Instantiate(projectilePrefab, targetPos, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        Destroy(meteor);
    }
    Vector2 GetNearestEnemyPosition(Vector2 from)
    {
        float closestDist = float.MaxValue;
        Vector2 closest = from;

        foreach (var enemyRB in enemy)
        {
            float dist = Vector2.Distance(from, enemyRB.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemyRB.transform.position;
            }
        }

        return closest;
    }
    //Піщана буря
   

    IEnumerator SandstormRoutine()
    {
        storm = Instantiate(sandstormPrefab, transform.position, Quaternion.identity);
        sand = storm.GetComponent<SandStorm>();
        SandStorm zone = storm.GetComponent<SandStorm>();
        zone.Setup(sandstormDuration, sandstormDamagePerSecond, slowMultiplier);
        yield return new WaitForSeconds(sandstormDuration);
        distortionRoutine = StartCoroutine(DistortionManager(0f, 2f));
        sand = null;
        Destroy(storm);
    }
    private void FixedUpdate()
    {
        if (sand == null) return;

        if (sand.inZone != previousZoneState)
        {
            previousZoneState = sand.inZone;

            if (distortionRoutine != null)
                StopCoroutine(distortionRoutine);

            float target = sand.inZone ? 10f : 0f;
            distortionRoutine = StartCoroutine(DistortionManager(target, 2f));
        }
    }
    public IEnumerator DistortionManager(float targetValue, float duration)
    {

        if (sandStormMat == null) yield break;

        float startValue = sandStormMat.GetFloat("_VignetteIntensity");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newValue = Mathf.Lerp(startValue, targetValue, t);
            sandStormMat.SetFloat("_VignetteIntensity", newValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        sandStormMat.SetFloat("_VignetteIntensity", targetValue); // точне завершення
    }
    //Кам'яна в'язниця
    private IEnumerator StonePrisonRoutine()
    {
        GameObject mark = Instantiate(prisonMarkPrefab);
        Transform markTransform = mark.transform;

        float elapsed = 0f;
        while (elapsed < followDuration)
        {
            markTransform.position = player.transform.position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 finalPosition = markTransform.position;

        

        yield return new WaitForSeconds(delayBeforeSpawn);

        // Спавнимо бар'єри по колу навколо гравця
        for (int i = 0; i < numberOfBarriers; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfBarriers;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 spawnPos = finalPosition + direction.normalized * stoneSpawnRadius;

            // Визначаємо, чи напрямок ближчий до вертикалі
            bool isVertical = Mathf.Abs(direction.y) > Mathf.Abs(direction.x);
            GameObject rotation = isVertical ? stonePrisonPrefabHorizontal : stonePrisonPrefabVertical;

            Instantiate(rotation, spawnPos, Quaternion.identity);
        }
    }
    // Детект зони
    private void OnTriggerEnter2D(Collider2D other) => UpdateZoneState(other, true);
    private void OnTriggerStay2D(Collider2D other) => UpdateZoneState(other, true);
    private void OnTriggerExit2D(Collider2D other) => UpdateZoneState(other, false);

    void UpdateZoneState(Collider2D other, bool state)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = state;
        }
        else if (other.CompareTag("Enemy") && other.GetComponent<Rigidbody2D>())
        {
            if (state && !enemy.Contains(other.GetComponent<Rigidbody2D>()))
            {
                enemy.Add(other.GetComponent<Rigidbody2D>());
            }
            else if (!state)
            {
                enemy.Remove(other.GetComponent<Rigidbody2D>());
            }

            enemyInZone = enemy.Count > 0;
        }
    }
}
