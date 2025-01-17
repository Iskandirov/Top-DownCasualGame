using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Shield : SkillBaseMono
{
    public Rigidbody2D rb;
    public float minDistance = 2f;
    public float maxDistance = 5f;
    public float attractionForce = 5f;
    public float repulsionForce = 3f;
    Vector3 directionToPlayer;
    public float orbitForce = 10f;
    public float orbitForceStart = 10f;
    public float maxSpeedStart = 5f;
    public float maxSpeed = 5f;
    public float enemyApproachDistance = 4f;
    float distanceToEnemy;
    float distanceToPlayer;
    public float levitationRange = 0.1f;
    public List<GameObject> enemiesInZone = new List<GameObject>();
    public GameObject enemy;
    List<Shield> otherScript;
    public Animator anim;
    public GameObject clash;
    public float healthShield;
    public float healthShieldMissed;
    public SlowArea slowObj;
    public float rockDamage;
    public GameObject rockObj;
    public float dirtElement;
    Transform objTransform;
    public float attackReloadTime;
    public float currentReloadTime;
    public bool isPotions;
    public bool isHitInTheEnd;
    // Start is called before the first frame update
    void Start()
    {
        currentReloadTime = attackReloadTime;
        player = PlayerManager.instance;
        otherScript = FindObjectsOfType<Shield>().ToList();
        otherScript.Remove(this);

        rb.drag = 3f;
        orbitForce = orbitForceStart;
        maxSpeed = maxSpeedStart;
        StartCoroutine(DeactivateInvincible());
        if (!isPotions)
        {
            player = PlayerManager.instance;
            objTransform = transform;
            if (basa.stats[1].isTrigger)
            {
                basa.lifeTime += basa.stats[1].value;
                basa.stats[1].isTrigger = false;
            }
            if (basa.stats[2].isTrigger)
            {
                basa.stepMax -= basa.stats[2].value;
                basa.stats[2].isTrigger = false;
            }
           
            healthShield = basa.damage;
            dirtElement = player.Dirt;
            if (basa.stats[3].isTrigger)
            {
                SlowArea a = Instantiate(slowObj, objTransform.position, Quaternion.identity, objTransform);
                a.dirtElement = dirtElement;
            }
            if (basa.stats[4].isTrigger)
            {
                basa.lifeTime += basa.stats[4].value;
                basa.stats[4].isTrigger = false;
            }
            healthShield = basa.damage;
            dirtElement = player.Dirt;
           
           
            player.isInvincible = true;
            CoroutineToDestroy(gameObject, basa.lifeTime + 2f);
        }
    }
    public IEnumerator DeactivateInvincible()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        player.isInvincible = false;
        anim.SetTrigger("Death");
    }
    public void DestroySword()
    {
        Destroy(gameObject);
    }
    public void DeactivateSword()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        orbitForce = 0f;
        orbitForceStart = 0f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemiesInZone != null)
            FindClosestEnemy(enemiesInZone);

        if (enemy != null)
            distanceToEnemy = Vector2.Distance(enemy.transform.position, player.objTransform.position);

        if (distanceToEnemy < enemyApproachDistance && enemy != null && Vector2.Distance(enemy.transform.position, player.objTransform.position) < 40f && !CheckIfHasEnemy())
        {
            MoveBetweenPlayerAndEnemy();
        }
        else
        {
            anim.SetBool("On", false);
            directionToPlayer = (player.objTransform.position - transform.position).normalized;
            distanceToPlayer = Vector2.Distance(player.objTransform.position, transform.position);

            if (distanceToPlayer > maxDistance)
            {
                rb.AddForce(directionToPlayer * attractionForce);
            }
            else if (distanceToPlayer < minDistance)
            {
                rb.AddForce(-directionToPlayer * repulsionForce);
            }

            Vector2 orbitDirection = Vector2.Perpendicular(directionToPlayer);
            rb.AddForce(orbitDirection * orbitForce);
        }
        currentReloadTime -= Time.fixedDeltaTime;
    }
    
    private bool CheckIfHasEnemy()
    {
        foreach (var obj in otherScript)
        {
            if (obj.enemy == enemy)
            {
                enemy = null;
                return true;
            }
        }
        return false;
    }

    public void FindClosestEnemy(List<GameObject> enemies)
    {
        float nearestDistSqr = Mathf.Infinity;
        foreach (var enemyIndex in enemies)
        {
            if (enemyIndex.activeSelf)
            {
                Vector3 enemyPos = enemyIndex.transform.position;
                float distSqr = (enemyPos - player.objTransform.position).sqrMagnitude;

                if (distSqr < nearestDistSqr && otherScript.FirstOrDefault(ClosestEnemy => ClosestEnemy.enemy == enemyIndex) == null)
                {
                    nearestDistSqr = distSqr;
                    enemy = enemyIndex;
                }
            }
        }
    }
    void MoveBetweenPlayerAndEnemy()
    {
        anim.SetBool("On", true);
        Vector2 middlePoint = (player.objTransform.position + enemy.transform.position) / 2;

        float levitationOffsetX = Random.Range(-levitationRange, levitationRange);
        float levitationOffsetY = Random.Range(-levitationRange, levitationRange);
        Vector2 levitationPosition = middlePoint + new Vector2(levitationOffsetX, levitationOffsetY);

        transform.position = Vector2.Lerp(transform.position, levitationPosition, 0.1f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !enemiesInZone.Contains(other.gameObject))
        {
            enemiesInZone.Add(other.gameObject);
            if (basa.stats[2].isTrigger)
            {
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("Bullet"))
        {
            clash.SetActive(true);
            if (basa.stats[2].isTrigger)
            {
                Destroy(other.gameObject);
            }
            Invoke("Disactivate", .5f);
        }
    }
    void Disactivate()
    {
        clash.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && enemiesInZone.Contains(other.gameObject))
        {
            enemiesInZone.Remove(other.gameObject);
        }
    }
}
