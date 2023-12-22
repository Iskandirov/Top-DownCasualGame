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
    public Vector3 startPosition;
    public float lifeTime;

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

            lifeTime = 3f;
        }
        else if (isRandome)
        {
            speed += 0.1f;
            lifeTime = 5f;
        }
        else
        {
            speed += 0.1f;
            lifeTime = 3f;
        }
        //Invoke("DestroyBullet", lifeTime);
    }
    public void DestroyBullet()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        speed = 2;
        distance = 5;
        isAround = false;
        isRandome = false;
        gameObject.SetActive(false);
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !PlayerManager.instance.isInvincible)
        {
            PlayerManager.instance.TakeDamage(damage);
            DestroyBullet();
        }
        else if (collision.CompareTag("Shield"))
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            DestroyBullet();
        }
    }
}
