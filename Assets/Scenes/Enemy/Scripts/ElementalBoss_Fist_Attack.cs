using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss_Fist_Attack : MonoBehaviour
{
    public float attackCalldown;
    public float attackCalldownMax;

    public float damage;

    public bool playerInZone;

    public GameObject player;
    Rigidbody2D playerRB;
    Health playerHealth;
    Animator playerAnim;
    Collider2D playerCollider;

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
        player = GameObject.FindWithTag("Player");
        attackCalldownMax = attackCalldown;
        playerRB = player.GetComponent<Rigidbody2D>();
        playerHealth = player.GetComponent<Health>();
        playerAnim = player.GetComponent<Animator>();
        playerCollider = player.GetComponent<Collider2D>();

        objShield = FindObjectOfType<Shield>();

        objAnim = GetComponent<Animator>();
        objMove = GetComponent<Forward>();
        objCollider = GetComponent<Collider2D>();
        childrenHealth = GetComponentInChildren<HealthPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCalldown > 0 && objAnim.GetBool("IsFistAttack") == false)
        {
            attackCalldown -= Time.deltaTime;
        }
        if (attackCalldown <= 0 && playerInZone)
        {
            objAnim.SetBool("IsFistAttack", true);
            attackCalldown = attackCalldownMax;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
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
                StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                if (collidedObject.isThreeLevel)
                {
                    childrenHealth.healthPoint -= damage / 2;
                }
            }
            else
            {
                // Handle collision with other objects
                if (pushableObjectRigidbody != null)
                {
                    playerHealth.playerHealthPoint -= damage;
                    playerHealth.playerHealthPointImg.fillAmount -= damage / playerHealth.playerHealthPointMax;
                    playerAnim.SetBool("IsHit", true);
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
