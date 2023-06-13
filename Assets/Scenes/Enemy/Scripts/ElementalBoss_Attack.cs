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
    public List<GameObject> enemy;
    public GameObject VFX_DamageArea;

    public float initialForce = 30f; // початкова сила відкиду
    public float duration = 0.5f; // тривалість відкиду в секундах
    public float reductionFactor = 1f; // коефіцієнт зменшення сили відкиду

    public List<Collider2D> bodyParts;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        attackCalldownMax = attackCalldown;
       // VFX_DamageArea.GetComponent<VisualEffect>().
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackCalldown > 0 && gameObject.GetComponent<Animator>().GetBool("IsJumpAttack") == false)
        {
            attackCalldown -= Time.deltaTime;
        }
        if (attackCalldown <= 0)
        {
            gameObject.GetComponent<Animator>().SetBool("IsJumpAttack", true);
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
        foreach (var part in bodyParts)
        {
            part.isTrigger = true;
        }
        VFX_DamageArea.SetActive(true);
        gameObject.GetComponent<Forward>().isFly = true;
        gameObject.GetComponent<Forward>().Relocate(player.transform);
    }
    public void ToForard()
    {
        VFX_DamageArea.SetActive(false);
        foreach (var part in bodyParts)
        {
            part.isTrigger = false;
        }
        gameObject.GetComponent<Forward>().isFly = false;
        gameObject.GetComponent<Animator>().SetBool("IsJumpAttack", false);
        //gameObject.GetComponent<Forward>().speed = gameObject.GetComponent<Forward>().speedMax;
        //// перевірка зіткнення з об'єктом, який має звичайний колайдер
        if (playerInZone)
        {
            Rigidbody2D pushableObjectRigidbody = player.GetComponent<Rigidbody2D>();

            // Check if the collided object has the "Shield" tag
            Shield collidedObject = FindObjectOfType<Shield>();

            if (collidedObject != null && collidedObject.CompareTag("Shield"))
            {
                collidedObject.healthShield -= damage;
                StartCoroutine(ReducePushForce(pushableObjectRigidbody));
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
