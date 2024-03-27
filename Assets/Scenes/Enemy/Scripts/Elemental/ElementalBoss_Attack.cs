using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss_Attack : MonoBehaviour
{
    public float attackCalldown;
    public float attackCalldownMax;

    public float damage;

    public bool playerInZone;
    public bool enemyInZone;

    Rigidbody2D playerRB;
    PlayerManager player;

    public List<Rigidbody2D> enemy;
    public GameObject VFX_DamageArea;

    public float initialForce = 30f;
    public float duration = 0.5f;
    public float reductionFactor = 1f;

    public List<Collider2D> bodyParts;
    Animator objAniml;
    Shield objShield;
    Transform objTransform;
    void Start()
    {
        player = PlayerManager.instance;
        playerRB = player.GetComponent<Rigidbody2D>();

        objShield = FindObjectOfType<Shield>();

        objAniml = GetComponent<Animator>();

        objTransform = transform;

        attackCalldownMax = attackCalldown;
    }

    void FixedUpdate()
    {
        if (attackCalldown > 0 && !objAniml.GetBool("IsJumpAttack"))
        {
            attackCalldown -= Time.fixedDeltaTime;
        }

        if (attackCalldown <= 0)
        {
            objAniml.SetBool("IsJumpAttack", true);
            attackCalldown = attackCalldownMax;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        AddPlayerToZone(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        AddPlayerToZone(collision);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        RemovePlayerFromZone(collision);
    }
    public void RemovePlayerFromZone(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
        }
        else if (collision.CompareTag("Enemy") && collision.GetComponent<Rigidbody2D>())
        {
            enemyInZone = false;
            enemy.Remove(collision.GetComponent<Rigidbody2D>());
        }
    } 
    public void AddPlayerToZone(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
        else if (collision.CompareTag("Enemy") && collision.GetComponent<Rigidbody2D>())
        {
            enemyInZone = true;
            enemy.Add(collision.GetComponent<Rigidbody2D>());
        }
    }
    public void SpeedFly()
    {
        VFX_DamageArea.SetActive(true);
        //objMove.isFly = true;
        Relocate(player.transform);
    }
    public void Relocate(Transform pos)
    {
        objTransform.position = new Vector2(pos.position.x, pos.position.y + 10);
    }
    public void ToForard()
    {
        VFX_DamageArea.SetActive(false);

        //objMove.isFly = false;
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
                if (pushableObjectRigidbody != null && !player.isInvincible)
                {
                    player.TakeDamage(damage);
                    StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                }
            }
        }

        if (enemyInZone)
        {
            foreach (Rigidbody2D enemyObject in enemy)
            {
                StartCoroutine(ReducePushForce(enemyObject));
            }
        }
        CineMachineCameraShake.instance.Shake(30, .1f);
    }
    private IEnumerator ReducePushForce(Rigidbody2D pushableObjectRigidbody)
    {
        float elapsedTime = 0f;
        float currentForce = initialForce;

        while (elapsedTime < duration)
        {
            Vector2 direction = (pushableObjectRigidbody.transform.position - objTransform.position).normalized;
            pushableObjectRigidbody.velocity = direction * currentForce;

            currentForce -= reductionFactor * initialForce * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Встановлення швидкості та сили відкиду в нуль
        pushableObjectRigidbody.velocity = Vector2.zero;
    }
}
