using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeteorRain : MonoBehaviour
{
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

    private bool playerInZone;
    private PlayerManager player;
    private Rigidbody2D playerRB;
    public List<Rigidbody2D> enemy = new List<Rigidbody2D>();

    [SerializeField] private float initialForce = 10f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float reductionFactor = 0.5f;
    [SerializeField] private Shield objShield;
    void Start()
    {
        objShield = FindObjectOfType<Shield>();
        player = PlayerManager.instance;
        playerRB = player.GetComponent<Rigidbody2D>();
    }
    public void ExecuteAttack()
    {
        StartCoroutine(MeteorAttackRoutine());
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
}