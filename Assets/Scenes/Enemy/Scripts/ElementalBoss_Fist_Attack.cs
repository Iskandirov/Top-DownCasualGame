using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss_Fist_Attack : MonoBehaviour
{
    public float attackCalldown;
    public float attackCalldownMax;

    public float damage;

    public bool playerInZone;

    //public List<ContactPoint2D> contacts;

    public GameObject player;
    public List<GameObject> VFX_DamageAreas;
    public List<Collider2D> hitZone;

    public float initialForce = 10f; // початкова сила відкиду
    public float duration = 0.5f; // тривалість відкиду в секундах
    public float reductionFactor = 1f; // коефіцієнт зменшення сили відкиду

    public List<Collider2D> handParts;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        attackCalldownMax = attackCalldown;
        // VFX_DamageArea.GetComponent<VisualEffect>().
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCalldown > 0 && gameObject.GetComponent<Animator>().GetBool("IsFistAttack") == false)
        {
            attackCalldown -= Time.deltaTime;
        }
        if (attackCalldown <= 0 && playerInZone)
        {
            gameObject.GetComponent<Animator>().SetBool("IsFistAttack", true);
            attackCalldown = attackCalldownMax;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
            //gameObject.GetComponent<CapsuleCollider2D>().GetContacts(contacts);
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
        foreach (var part in handParts)
        {
            part.isTrigger = true;
        }
        foreach (var zone in VFX_DamageAreas)
        {
            zone.SetActive(true);
        }
        gameObject.GetComponent<Forward>().isFly = true;
    }
    public void ToForardFromFist()
    {
        foreach (var zone in VFX_DamageAreas)
        {
            zone.SetActive(false);
        }
        foreach (var part in handParts)
        {
            part.isTrigger = false;
        }
        gameObject.GetComponent<Forward>().isFly = false;
        gameObject.GetComponent<Animator>().SetBool("IsFistAttack", false);
        // Check collision with objects that have a regular collider and the "Shield" tag
        if (playerInZone && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(player.GetComponent<Collider2D>()))
        {
            Rigidbody2D pushableObjectRigidbody = player.GetComponent<Rigidbody2D>();

            // Check if the collided object has the "Shield" tag
            Shield collidedObject = FindObjectOfType<Shield>();

            if (collidedObject != null && collidedObject.CompareTag("Shield"))
            {
                collidedObject.healthShield -= damage;
                StartCoroutine(ReducePushForce(pushableObjectRigidbody));
                if (collidedObject.GetComponent<Shield>().isThreeLevel)
                {
                    gameObject.GetComponentInChildren<HealthBossPart>().healthPoint -= damage / 2;
                }
            }
            else
            {
                // Handle collision with other objects
                if (pushableObjectRigidbody != null)
                {
                    player.GetComponent<Health>().playerHealthPoint -= damage;
                    player.GetComponent<Health>().playerHealthPointImg.fillAmount -= damage / player.GetComponent<Health>().playerHealthPointMax;
                    player.GetComponent<Animator>().SetBool("IsHit", true);
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
