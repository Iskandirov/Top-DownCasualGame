using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class Illusion : MonoBehaviour
{


    GameObject player;
    public Zzap zzap;
    public float x;
    public float y;
    public float xZzap;
    public float yZzap;
    public float lifeTime;
    public bool isFive;
    public float angle;

    public float stepShoot;
    public float attackSpeed;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Move>().gameObject;
        stepShoot = player.GetComponent<Shoot>().stepShoot;
        attackSpeed = player.GetComponent<Shoot>().attackSpeed;
        if (isFive)
        {
            Zzap a = Instantiate(zzap, transform.position, Quaternion.Euler(0, 0, angle));
            a.copie = gameObject;
            a.x = xZzap;
            a.y = yZzap;
            a.lifeTime = lifeTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        transform.position = new Vector2(player.transform.position.x + x, player.transform.position.y + y);
        ShootBullet(gameObject);
        

    }
    public void ShootBullet(GameObject ShootPointObj)
    {
        stepShoot += Time.deltaTime;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (stepShoot >= attackSpeed)
        {
            if (Input.GetMouseButton(0))  // Перевіряємо, чи натиснута ліва кнопка миші
            {
                stepShoot = 0;
                player.GetComponent<Shoot>().shoot = ShootPointObj;
                // Отримуємо позицію курсора у світових координатах
                //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;  // Закріплюємо координату Z на площині

                // Створюємо об'єкт з використанням префабу
                Bullet newObject = Instantiate(player.GetComponent<Shoot>().bullet, ShootPointObj.transform.position, Quaternion.identity);
                newObject.damage = player.GetComponent<Shoot>().damageToGive;
                newObject.isPiers = player.GetComponent<Shoot>().isLevelFive;
                if (player.GetComponent<Shoot>().isLevelTwo)
                {
                    for (int i = 1; i < player.GetComponent<Shoot>().secondBulletCount + 1; i++)
                    {
                        Invoke("CreateBullet", i * 0.3f);
                    }
                }
                // Визначаємо напрямок до курсора
                Vector2 directionBullet = mousePosition - transform.position;

                // Запускаємо об'єкт в заданому напрямку
                Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
                rb.AddForce(directionBullet.normalized * player.GetComponent<Shoot>().launchForce, ForceMode2D.Impulse);
                float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
                newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
            }
        }
    }
    public void CreateBullet()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bullet newObject = Instantiate(player.GetComponent<Shoot>().bullet, transform.position, Quaternion.identity);
        newObject.damage = player.GetComponent<Shoot>().damageToGive;
        newObject.isPiers = player.GetComponent<Shoot>().isLevelFive;

        Vector2 directionBullet = mousePosition - transform.position;

        // Запускаємо об'єкт в заданому напрямку
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        rb.AddForce(directionBullet.normalized * player.GetComponent<Shoot>().launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
        newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
    }
}
