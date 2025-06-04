using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornadic_Rush : MonoBehaviour
{
    public Tornadic_Move tornadic;
    public GameObject target;
    public float spawnRadius = 5.0f;
    public GameObject lightningObj;
    [SerializeField] Vector3 dir;
    [SerializeField] GameObject objArea;
    [SerializeField] float interval = 10;
    [SerializeField] float speedRush = 10;
    [SerializeField] bool state = false;
    [SerializeField] bool rush = false;
    [SerializeField] bool playerToched = false;
    Transform objTransform;
    Vector2 object1Pos;
    Vector2 object2Pos;
    [SerializeField] Animator anim;

    private Dictionary<AttackType, Action> attackMethods;
    public enum AttackType
    {
        Rush,
        Lightnings,
        Tornadics,
    }
    private bool isTransitioning = false;

    [Header("Projectile Intervals")]
    public float rushAttackInterval = 5f;
    public float lightningsAttackInterval = 5f;
    public float tornadicsAttackInterval = 10f;

    [Header("Projectile Counts")]
    public float tornadicCount = 10;
    public float lightningsCount = 10;
    [Header("Damages")]
    public float lightningDamage = 20;
    private void Start()
    {

        objTransform = transform;
        //StartCoroutine(TornadorRush());

        attackMethods = new Dictionary<AttackType, Action>
        {
            { AttackType.Rush, RushAnim },
            { AttackType.Lightnings, SpawnObjectsAndLightnings },
            { AttackType.Tornadics, TornadicsSpawn },
        };

        StartCoroutine(AttackRoutine(rushAttackInterval, AttackType.Rush));
        StartCoroutine(AttackRoutine(lightningsAttackInterval, AttackType.Lightnings));
        StartCoroutine(AttackRoutine(tornadicsAttackInterval, AttackType.Tornadics));
    }
    IEnumerator AttackRoutine(float interval, AttackType attackType)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (!isTransitioning && attackMethods.ContainsKey(attackType))
            {
                attackMethods[attackType]();
            }
        }
    }
    private void FixedUpdate()
    {
        if (rush)
        {
            Vector2 direction = object2Pos - object1Pos;
            direction.Normalize();
            direction.x *= objTransform.rotation.y < 0 ? -1 : 1;
            objTransform.Translate(direction * speedRush * Time.deltaTime);
            if (playerToched)
            {
                PlayerManager.instance.TakeDamage(15f);
            }
        }
    }
    //Rush
    public void RushAnim()
    {
        state = !state;
        anim.SetBool("Rush", state);
    }
    public void RushMove()
    {
        rush = !rush;
    }
    public void GetDirection()
    {
        object1Pos = transform.position;
        object2Pos = PlayerManager.instance.objTransform.transform.position;

        float angle = Mathf.Atan2(object2Pos.y - object1Pos.y, object2Pos.x - object1Pos.x);

        angle *= Mathf.Rad2Deg;
        angle += 180;
        objArea.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerToched = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerToched = false;
        }
    }
    //Rush End
    //Tornadics
    public void TornadicsSpawn()
    {
        for (int i = 0; i <= tornadicCount; i++)
        {
            Tornadic_Move a = Instantiate(tornadic, objTransform.position, Quaternion.identity);
            a.mainDirection = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
        }
    }
    //Tornadics End
    //Lightnings
    void SpawnObjectsAndLightnings()
    {
        StartCoroutine(SpawnObjectsAndLightningsCoroutine());
    }

    IEnumerator SpawnObjectsAndLightningsCoroutine()
    {
        // Спавнимо мітки
        for (int i = 0; i <= lightningsCount; i++)
        {
            Vector3 spawnPosition = objTransform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
            spawnPosition.z = 0f;
            Instantiate(target, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }

        // Чекаємо одну секунду перед спавном блискавок
        yield return new WaitForSeconds(5f);

        // Спавнимо блискавки
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Lightning");
        foreach (var obj in spawnedObjects)
        {
            bool playerInZone = Physics2D.OverlapCircle(obj.transform.position, 3, LayerMask.GetMask("Player"));
            Collider2D ShieldIsActive = Physics2D.OverlapCircle(obj.transform.position, 3, LayerMask.GetMask("PlayerIgnore"));
            if (playerInZone)
            {
                if (ShieldIsActive != null)
                {
                    if (ShieldIsActive.CompareTag("Shield"))
                    {
                        ShieldIsActive.GetComponent<Shield>().healthShield -= lightningDamage;
                        FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", lightningDamage);
                    }
                }
                else if (!PlayerManager.instance.isInvincible)
                {
                    PlayerManager.instance.TakeDamage(lightningDamage);
                }
            }

            Vector3 spawnPosition = obj.transform.position;
            Instantiate(lightningObj, spawnPosition, Quaternion.identity);
            Destroy(obj);
        }
    }
    private void OnDrawGizmos()
    {
        // Знаходимо всі об'єкти, які мають тег "Lightning"
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Lightning");
        Gizmos.color = Color.green; // напівпрозорий жовтий

        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;
            Gizmos.DrawWireSphere(obj.transform.position, 3f);
        }

    }
    //Lightnings End
}

