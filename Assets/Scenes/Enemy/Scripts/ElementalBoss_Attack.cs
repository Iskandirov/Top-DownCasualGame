using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class ElementalBoss_Attack : MonoBehaviour
{
    public float attackCalldown;
    public float attackCalldownMax;

    public float damage;

    public bool playerInZone;
    public bool enemyInZone;

    public GameObject player;
    Rigidbody2D playerRB;
    Health playerHealth;
    Animator playerAnim;

    public List<GameObject> enemy;
    public GameObject VFX_DamageArea;

    public float initialForce = 30f;
    public float duration = 0.5f;
    public float reductionFactor = 1f;

    public List<Collider2D> bodyParts;
    Animator objAniml;
    Forward objMove;
    Shield objShield;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerHealth = player.GetComponent<Health>();
        playerAnim = player.GetComponent<Animator>();

        objShield = FindObjectOfType<Shield>();

        objAniml = GetComponent<Animator>();
        objMove = GetComponent<Forward>();

        attackCalldownMax = attackCalldown;
    }

    void FixedUpdate()
    {
        if (attackCalldown > 0 && !objAniml.GetBool("IsJumpAttack"))
        {
            attackCalldown -= Time.deltaTime;
        }

        if (attackCalldown <= 0)
        {
            objAniml.SetBool("IsJumpAttack", true);
            attackCalldown = attackCalldownMax;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
        else if (collision.CompareTag("Enemy"))
        {
            enemyInZone = true;
            enemy.Add(collision.gameObject);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
        else if (collision.CompareTag("Enemy"))
        {
            enemyInZone = true;
            enemy.Add(collision.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
        }
        else if (collision.CompareTag("Enemy"))
        {
            enemyInZone = false;
            enemy.Remove(collision.gameObject);
        }
    }

    public void SpeedFly()
    {
        VFX_DamageArea.SetActive(true);
        objMove.isFly = true;
        objMove.Relocate(player.transform);
    }

    public void ToForard()
    {
        VFX_DamageArea.SetActive(false);

        objMove.isFly = false;
        objAniml.SetBool("IsJumpAttack", false);

        if (playerInZone)
        {
            Rigidbody2D pushableObjectRigidbody = playerRB;
            Shield collidedObject = objShield;

            if (collidedObject != null && collidedObject.CompareTag("Shield"))
            {
                collidedObject.healthShield -= damage;
                FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
                StartCoroutine(ReducePushForce(pushableObjectRigidbody));
            }
            else
            {
                if (pushableObjectRigidbody != null && !playerHealth.GetComponent<Move>().isUntouchible)
                {
                    playerHealth.playerHealthPoint -= damage;
                    FindObjectOfType<StatsCollector>().FindStatName("DamageTaken", damage);
                    playerHealth.playerHealthPointImg.fullFillImage.fillAmount -= damage / playerHealth.playerHealthPointMax;
                    playerAnim.SetBool("IsHit", true);
                    StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                }
            }
        }

        if (enemyInZone)
        {
            foreach (GameObject enemyObject in enemy)
            {
                Rigidbody2D pushableObjectRigidbody = enemyObject.GetComponent<Rigidbody2D>();
                if (pushableObjectRigidbody != null)
                {
                    StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                }
            }
        }
    }
    private IEnumerator ReducePushForce(Rigidbody2D pushableObjectRigidbody)
    {
        float elapsedTime = 0f;
        float currentForce = initialForce;

        while (elapsedTime < duration)
        {
            Vector2 direction = (pushableObjectRigidbody.transform.position - transform.position).normalized;
            pushableObjectRigidbody.velocity = direction * currentForce;

            currentForce -= reductionFactor * initialForce * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Встановлення швидкості та сили відкиду в нуль
        pushableObjectRigidbody.velocity = Vector2.zero;
        currentForce = 0f;
    }
}
