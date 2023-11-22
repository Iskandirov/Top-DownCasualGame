using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipetreeAttack : MonoBehaviour
{
    public GameObject bullet;
    public float stepShoot;
    public float damage;
    public float attackSpeed;
    public float launchForce = 10f;  // Сила запуску об'єкта
    public Transform playerPos;
    public void Start()
    {
        playerPos = FindObjectOfType<Move>().transform;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ShootBullet();
    }

    public void ShootBullet()
    {
        stepShoot += Time.deltaTime;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (stepShoot >= attackSpeed)
        {
            // визначаємо напрямок до гравця
            Vector2 direction = playerPos.position - transform.position;
            direction.Normalize();

            stepShoot = 0;
            // Отримуємо позицію курсора у світових координатах
            //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;  // Закріплюємо координату Z на площині

            // Створюємо об'єкт з використанням префабу
            Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = launchForce * new Vector2(direction.x, direction.y);
            // Запускаємо об'єкт в заданому напрямку
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(direction.normalized * launchForce, ForceMode2D.Impulse);
            float angleShot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
        }
    }
}
