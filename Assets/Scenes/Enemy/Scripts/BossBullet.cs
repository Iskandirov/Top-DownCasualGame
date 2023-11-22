using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public string[] targetPrefabNames;
    public bool isAround;
    public bool isRandome;
    public Transform enemyBody;
    public float distance = 20f;
    public float speed = 5;
    public float angle = 5;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAround)
        {
            // Визначаємо колову траєкторію навколо цілі
            Vector3 position = startPosition + new Vector3(Mathf.Cos(Time.time * speed) * distance, Mathf.Sin(Time.time * speed) * distance, 0f);
            distance += 0.3f;

            // Задаємо позицію об'єкту
            transform.position = position;

            Invoke("DestroyBullet", 3f);
        }
        else if (isRandome)
        {
            speed += 0.1f;
            Invoke("DestroyBullet", 5f);
        }
        else
        {
            speed += 0.1f;
            Invoke("DestroyBullet", 3f);
        }
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !collision.GetComponent<Move>().isUntouchible)
        {
            collision.GetComponent<Health>().playerHealthPoint -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("DamageTaken", damage);
            collision.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.GetComponent<Health>().playerHealthPointMax;
            collision.GetComponent<Animator>().SetBool("IsHit", true);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Shield"))
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
    }
}
