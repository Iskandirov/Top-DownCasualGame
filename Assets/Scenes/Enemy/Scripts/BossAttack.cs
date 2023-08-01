using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public BossBullet bulletPrefab;

    public float interval = 2f;
    public float defaultInterval = 2f;
    public float randomeInterval = 2f;
    public float roundInterval = 2f;

    private float angle = 0f;
    public float speed = 5f;
    public float aceleration = 1f;

    public List<bool> attackBools;
    public float attackTypeInterval = 5f;
    public int Index = -1;
    public void Start()
    {
        StartCoroutine(ToggleBoolsCoroutine());
        StartCoroutine(TimerCoroutineTypesAttack());
    }
    private IEnumerator ToggleBoolsCoroutine()
    {
        while (true)
        {
            if (Index < attackBools.Count - 1)
            {
                Index++;
            }
            else
            {
                Index = 0;
            }
            attackBools[Index] = true;

            // Виключаємо всі інші були
            for (int i = 0; i < attackBools.Count; i++)
            {
                if (i != Index)
                {
                    attackBools[i] = false;
                }
            }

            // Зачекайте декілька секунд перед наступною зміною
            yield return new WaitForSeconds(attackTypeInterval);
        }
    }
    private IEnumerator TimerCoroutineTypesAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            aceleration = 1;
            AttackTypes();
        }
    } 
    private void AttackTypes()
    {
        if (attackBools[0] == true)
        {
            // Випустити об'єкти з відповідними напрямками
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 45f) * Mathf.Deg2Rad), Mathf.Sin((angle + 45f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 135f) * Mathf.Deg2Rad), Mathf.Sin((angle + 135f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 225f) * Mathf.Deg2Rad), Mathf.Sin((angle + 225f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 325f) * Mathf.Deg2Rad), Mathf.Sin((angle + 325f) * Mathf.Deg2Rad));
            angle += 45f;
            interval = defaultInterval;
        }
        else if (attackBools[1] == true)
        {
            for (int i = 0; i < 10; i++)
            {
                BossBullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
                bullet.GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle) * Mathf.Deg2Rad), Mathf.Sin((angle) * Mathf.Deg2Rad));
                bullet.enemyBody = transform;
                bullet.isAround = true;
                bullet.speed = 1f / (1f + i * 0.15f);
            }
            interval = roundInterval;
        }
        else if (attackBools[2] == true)
        {
            for (int i = 0; i < 30; i++)
            {
                Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((Random.Range(5, 360)) * Mathf.Deg2Rad), Mathf.Sin((Random.Range(5, 360)) * Mathf.Deg2Rad));
            }
            interval = randomeInterval;
        }
        
    }
    private void Update()
    {
        aceleration += 0.1f;
    }
}