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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAround)
        {
            // Визначаємо колову траєкторію навколо цілі
            Vector3 position = enemyBody.position + new Vector3(Mathf.Cos(Time.time * speed) * distance, Mathf.Sin(Time.time * speed) * distance, 0f);
            distance += 0.3f;
            
            // Задаємо позицію об'єкту
            transform.position = position;

            // Повертаємо об'єкт обличчям до цілі (необов'язково)
            Vector3 direction = enemyBody.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
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
        foreach (string targetPrefabName in targetPrefabNames)
        {
            if (collision.gameObject.name == targetPrefabName && !collision.isTrigger)
            {
                collision.GetComponent<Health>().playerHealthPoint -= damage;
                collision.GetComponent<Health>().playerHealthPointImg.MinusProgressBar(damage);
                collision.GetComponent<Animator>().SetBool("IsHit", true);
                Destroy(gameObject);
            }
        }
    }
}
