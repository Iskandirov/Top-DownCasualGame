using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiwi_Attack : MonoBehaviour
{
    public GameObject root; // Префаб об'єкта, який буде запускатись
    public float launchForce = 10.0f; // Сила запуску
    public float delay;
    float delayMax;
    Transform objTransform;
    public void Start()
    {
        delayMax = delay;
        objTransform = transform;
    }
    public void FixedUpdate()
    {
        delay -= Time.fixedDeltaTime;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Перевіряємо, чи об'єкт в триггер-зоні є гравцем
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (delay <= 0)
            {
                delay = delayMax;
                // Отримуємо напрямок до гравця
                Vector2 directionToPlayer = collision.transform.position - objTransform.position;

                // Створюємо новий об'єкт з використанням префабу
                GameObject newObject = Instantiate(root, objTransform.position, Quaternion.identity);

                // Запускаємо новий об'єкт у напрямку гравця
                Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
                rb.velocity = directionToPlayer.normalized * launchForce;
            }
        }
    }
}
