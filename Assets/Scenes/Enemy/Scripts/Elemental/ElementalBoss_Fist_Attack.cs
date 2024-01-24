using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss_Fist_Attack : MonoBehaviour
{
    public float attackCalldown;
    public float attackCalldownMax;

    public float damage;

    public bool playerInZone;

    Rigidbody2D playerRB;
    Collider2D playerCollider;
    PlayerManager player;

    public List<GameObject> VFX_DamageAreas;
    public List<Collider2D> hitZone;

    public float initialForce = 10f; // початкова сила відкиду
    public float duration = 0.5f; // тривалість відкиду в секундах
    public float reductionFactor = 1f; // коефіцієнт зменшення сили відкиду

    public List<Collider2D> handParts;

    Animator objAnim;
    Forward objMove;
    Shield objShield;
    HealthPoint childrenHealth;
    Collider2D objCollider;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        attackCalldownMax = attackCalldown;
        playerRB = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();

        objShield = FindObjectOfType<Shield>();

        objAnim = GetComponent<Animator>();
        objMove = GetComponent<Forward>();
        objCollider = GetComponent<Collider2D>();
        childrenHealth = GetComponentInChildren<HealthPoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackCalldown > 0 && objAnim.GetBool("IsFistAttack") == false)
        {
            attackCalldown -= Time.fixedDeltaTime;
        }
        if (attackCalldown <= 0 && playerInZone)
        {
            objAnim.SetBool("IsFistAttack", true);
            attackCalldown = attackCalldownMax;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        DeleteFromZone(collision, true);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        DeleteFromZone(collision, true);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        DeleteFromZone(collision,false);
    }
    void DeleteFromZone(Collider2D collision,bool state)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = state;
        }
    }
    public void KickAnim()
    {
        foreach (var zone in VFX_DamageAreas)
        {
            zone.SetActive(true);
        }
        objMove.isFly = true;
    }
    public void ToForardFromFist()
    {
        foreach (var zone in VFX_DamageAreas)
        {
            zone.SetActive(false);
        }
       
        objMove.isFly = false;
        objAnim.SetBool("IsFistAttack", false);
        // Check collision with objects that have a regular collider and the "Shield" tag
        if (playerInZone && objCollider.IsTouching(playerCollider))
        {
            Rigidbody2D pushableObjectRigidbody = playerRB;

            // Check if the collided object has the "Shield" tag
            Shield collidedObject = objShield;

            if (collidedObject != null && collidedObject.CompareTag("Shield"))
            {
                collidedObject.healthShield -= damage;
                FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
                StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                if (collidedObject.basa.stats[2].isTrigger)
                {
                    childrenHealth.healthPoint -= damage / 2;
                    FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage / 2);
                }
                
            }
            else
            {
                // Handle collision with other objects
                if (pushableObjectRigidbody != null && !player.isInvincible)
                {
                    player.TakeDamage(damage);
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
    }
}
